using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The class MessageHandler contains methods for parsing the messages coming from the IoT adapter and the MES.
/// </summary>

public class MessageHandler_noJson : MonoBehaviour
{
    private GameObject client;
    private GameObject assemblies;
    private GameObject feedback_canvas;
    private GameObject task_finished;
    private GameObject final_assembly_green;
    private GameObject camera;

    public int current_knowledge_level;
    public string current_version;
    private string current_producttype;
    private GameObject current_assembly_GO;

    private List<GameObject> product_versions = new List<GameObject>();
    private List<GameObject> assembly_items = new List<GameObject>();
    private List<GameObject> active_items = new List<GameObject>();
    private List<GameObject> disabled_items = new List<GameObject>();
    
    // Materials
    private Material assembly_info_material_1;
    private Material assembly_info_material_2;
    private Material finished_info_material;
    private Material error_info_material;
    private Material transparent_material;

    // UI
    private GameObject setup_test;
    private GameObject current_point_display;
    private GameObject current_action_display;
    private GameObject max_point_display;
    private int max_points;
    private List<GameObject> uncompleted_steps = new List<GameObject>();
    private GameObject object_presentation;
    private GameObject assembly_presentation;
    private GameObject annotation;
    private float pick_prefab_scale;

    // UI: Miniature assembly
    private GameObject assembly_miniature;
    private GameObject assembly_miniature_holder;
    private List<GameObject> optically_changed_parts = new List<GameObject>();

    void Start()
    {
        assembly_info_material_1 = (Material)Resources.Load("Materials/InformationMaterial1", typeof(Material));
        assembly_info_material_2 = (Material)Resources.Load("Materials/InformationMaterial2", typeof(Material));
        finished_info_material = (Material)Resources.Load("Materials/Green", typeof(Material));
        error_info_material = (Material)Resources.Load("Materials/Red", typeof(Material));
        transparent_material = (Material)Resources.Load("Materials/Transparent", typeof(Material));

        client = GameObject.Find("Client");

        // Find GO
        setup_test = GameObject.Find("Setup_Test");
        assemblies = GameObject.Find("Assemblies");
        object_presentation = GameObject.Find("NextObjects");
        assembly_presentation = GameObject.Find("TotalAssembly");
        task_finished = GameObject.Find("TaskFinished");
        camera = GameObject.Find("MainCamera");

        // Find Elements of Feedback Canvas
        feedback_canvas = GameObject.Find("FeedbackCanvas");
        current_action_display = feedback_canvas.transform.Find("General/ActionInfo").gameObject;
        annotation = feedback_canvas.transform.Find("General/Annotation").gameObject;
        current_point_display = feedback_canvas.transform.Find("Gamification/PointDisplay/CurrentPoints").gameObject;
        max_point_display = feedback_canvas.transform.Find("Gamification/PointDisplay/MaxPoints").gameObject;
    }

    public void InitializeVersion(string version_name)
    {
        // Reset everything
        ResetWorkplace();
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetFeedbackElements();
        if(current_assembly_GO != null)  // Remove previous product display
        {
            Destroy(current_assembly_GO);
        }

        // Set colors of gamification elements
        feedback_canvas.GetComponent<UI_FeedbackHandler>().InitializeQualityRate(80.0f, 60.0f);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().InitializeTimeRate(80.0f, 60.0f);

        // Load new work information
        current_version = version_name;  // Get V3.1
        current_producttype = current_version.Split('.')[0];  // Get V3
        Debug.Log("Load product version " + current_version);
        Debug.Log("Load product type " + current_producttype);
        current_action_display.GetComponent<Text>().text = "Load version " + current_version;

        product_versions = assemblies.GetComponent<AssemblyOrganisation>().main_items_list;

        // Copy current product to highlight and modify it for the assembly instructions
        foreach (GameObject product in product_versions)
        {
            if (product.name == current_version)
            {
                Debug.Log("Instantiate new assembly for " + product.name);
                
                // Check if assembly contains all required scripts
                if (product.GetComponent<AssemblyOrganisation>() == null)
                {
                    product.AddComponent<AssemblyOrganisation>();
                }
                if (product.GetComponent<ObjectInteractions>() == null)
                {
                    product.AddComponent<ObjectInteractions>();
                }
                foreach (Transform item in product.transform)
                {
                    if (item.gameObject.GetComponent<ObjectInteractions>() == null)
                    {
                        item.gameObject.AddComponent<ObjectInteractions>();
                    }
                }

                // Create new assembly to display information without modifying the original assembly object
                current_assembly_GO = Instantiate(
                    original: product,
                    position: product.transform.position,
                    rotation: product.transform.rotation,
                    parent: product.transform.parent);
                current_assembly_GO.SetActive(true);
            }
            product.SetActive(false);  // Deactivate all product versions
        }

        // Set product-specific properties
        pick_prefab_scale = current_assembly_GO.GetComponent<AssemblyOrganisation>().pick_prefab_scale;
        Debug.Log("New pick prefab scale: " + pick_prefab_scale);


        if (current_assembly_GO == null)
            {
                Debug.LogError("Product assembly not found of version " + current_version);
            }

        // Generate miniature product of current assembly
        GenerateMiniature(current_assembly_GO);

        // Deactivate all items of current assembly
        foreach (Transform item in current_assembly_GO.transform)
        {
            item.gameObject.SetActive(false);
        }

        // Acknowledge init and send user information to hardware control
        Debug.Log(setup_test.GetComponent<Admin_PropertySelection>().user_name);
        client.GetComponent<Connection_noJson>().SendInformation("init_username[" + setup_test.GetComponent<Admin_PropertySelection>().user_name + "]level[" + setup_test.GetComponent<Admin_PropertySelection>().user_level + "]");
    }

    public void InitializeSteps(int number_steps)
    {
        Debug.Log("InitializeSteps: " + number_steps.ToString());
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetNumberSteps();
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowNumberSteps(number_steps);
    }

    public void InitializePoints (int number_points)
    {
        max_points = number_points;
        Debug.Log("InitializePoints: " + max_points.ToString());
        max_point_display.GetComponent<Text>().text = max_points.ToString();
        ShowPoints(0);
    }

    public void NewInstructions ()
    {
        // Debug.Log("New work step -> reset support");
        ResetWorkplace();
    }

    public void InitializeCamera(float x, float y, float z, float ortho)
    {
        camera.GetComponent<CameraHandler>().ChangeCameraSettings(x, y, z, ortho);
    }

    public void ParsePerformanceMessage(int total_points, float quality_performance, float time_performance, int total_level, string node_finished, string level_up, string perfect_run, 
        string message_text, int message_color_r, int message_color_g, int message_color_b)
    {
        // Todo: Neue Punkte schicken
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowPoints(total_points);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowQualityRate(quality_performance);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowTimeRate(time_performance);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowLevel(total_level);
        if (node_finished == "True")
        {
            feedback_canvas.GetComponent<UI_FeedbackHandler>().FinishStep();
            // Todo: Ton kurz
        }
        if(level_up == "True")
        {
            feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayLevelup();
        }
        if(perfect_run == "True")
        {
            feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayPerfectRun();
        }
        if(message_text != "")
        {
            feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayPopup(message_text, message_color_r, message_color_g, message_color_b);
        }
    }

    public void PickObject(string item_name, string led_color, int knowledge_level, int default_time)
    {
        // Demo application for events
        if(item_name == "thank_you")
        {
            current_action_display.GetComponent<Text>().text = "Thank You!";
            return;
        }

        // Find and show prefab
        GameObject item_prefab = FindPrefab("Prefabs/Parts/" + current_producttype + "/" + item_name, item_name);
        if (item_prefab == null)
        {
            Debug.LogWarning("Prefab for " + item_name + " not found");
            return;
        }

        if (led_color == "green")  // Correct pick -> show information according to level
        {
            Debug.Log("Show pick instruction for " + item_name);
            if (knowledge_level < 4)  // Show 3D image
            {
                ShowPickPrefab(item_prefab, "Pick item");
            }
            else if (knowledge_level == 4)  // Do not show 3D image
            {
            }
            else
            {
                Debug.LogWarning("Unknown level " + knowledge_level);
            }
            feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);
        }
        else if (led_color == "red")  // Wrong pick -> show error information independently from level
        {
            Debug.Log("Show error pick instruction for " + item_name);
            feedback_canvas.GetComponent<UI_FeedbackHandler>().NotifyWrongAction();
            ResetWorkplace();
            GameObject item = ShowPickPrefab(item_prefab, "Wrong pick");
            item.GetComponent<ObjectInteractions>().ChangeMaterial(error_info_material);
        }        
    }

    public void StoreObject(string item_name, int knowledge_level, int default_time)
    {
        Debug.Log("Show store instruction for " + item_name);
        current_action_display.GetComponent<Text>().text = "Store " + item_name;
        feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);

        // Delete the finished assembly GO if it is part of assembly
        foreach (GameObject finished_item in current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list)
        {
            Destroy(finished_item);
            current_assembly_GO.GetComponent<AssemblyOrganisation>().main_items_list.Remove(finished_item);
        }
        current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list = new List<GameObject>();
    }

    public void PickTool(string tool_name, string led_color, int knowledge_level, int default_time)
    {
        // Find prefab
        GameObject tool_prefab = FindPrefab("Prefabs/Tools/" + tool_name, tool_name);
        if (tool_prefab == null)
        {
            Debug.LogWarning("Prefab for " + tool_name + " not found");
            return;
        }

        // Show pick instructions
        if (led_color == "green")  // Correct pick -> show information according to level
        {
            Debug.Log("Show pick instruction for " + tool_name);
            if (knowledge_level < 4)  // Show 3D image
            {
                ShowPickPrefab(tool_prefab, "Pick tool");
            }
            else if (knowledge_level == 4)  // Do not show 3D image
            {
            }
            else
            {
                Debug.LogWarning("Unknown level " + knowledge_level);
            }
            feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);
        }
        else if (led_color == "red")  // Wrong pick -> show error information independently from level
        {
            Debug.Log("Show error pick instruction for " + tool_name);
            feedback_canvas.GetComponent<UI_FeedbackHandler>().NotifyWrongAction();
            ResetWorkplace();
            GameObject tool = ShowPickPrefab(tool_prefab, "Wrong pick, return tool");
            tool.GetComponent<ObjectInteractions>().ChangeMaterial(error_info_material);
        }
    }

    public void ReturnTool(string tool_name, string led_color, int knowledge_level, int default_time)
    {
        // Find prefab
        GameObject tool_prefab = FindPrefab("Prefabs/Tools/" + tool_name, tool_name);
        if (tool_prefab == null)
        {
            Debug.LogWarning("Prefab for " + tool_name + " not found");
            return;
        }

        // Show pick instructions
        Debug.Log("Show return instruction for " + tool_name);
        if (knowledge_level < 4)  // Show 3D image
        {
            ShowPickPrefab(tool_prefab, "Return tool");
        }
        else if (knowledge_level == 4)  // Do not show 3D image
        {
        }
        else
        {
            Debug.LogWarning("Unknown level " + knowledge_level);
        }
        feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);
    }

    public void ShowAssemblyInfos(string item_name, int knowledge_level, int default_time, string text_annotation)
    {
        if (item_name == "thank_you")
        {
            current_action_display.GetComponent<Text>().text = "Thank You!";
            feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayThankYou();
            return;
        }

        Debug.Log("Show assembly instruction for " + item_name + " in level " + knowledge_level);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);

        if (knowledge_level == 1)
        {
            current_action_display.GetComponent<Text>().text = "Assemble";
            annotation.GetComponent<Text>().text = text_annotation;
            annotation.GetComponent<UI_BackgroundImage>().annotation_change = true;
            ShowAssemblyPosition(assembly_info_material_2, item_name, disable_afterwards: true, change_material: true);
            ShowPositionMiniature(item_name);
        }
        else if (knowledge_level == 2)
        {
            current_action_display.GetComponent<Text>().text = "Assemble";
            GameObject item_go = ShowAssemblyPosition(assembly_info_material_2, item_name, disable_afterwards: true, change_material: true);
            RemoveAssemblyHints(item_go);
            ShowPositionMiniature(item_name);
        }
        else if (knowledge_level == 3)
        {
            current_action_display.GetComponent<Text>().text = "Assemble";
            ShowPositionMiniature(item_name);
        }
        else if (knowledge_level == 4)
        {
        }
        else
        {
            Debug.LogWarning("Unknown level " + knowledge_level);
        }

        GameObject current_go = FindGameobject(item_name, current_assembly_GO.GetComponent<AssemblyOrganisation>().main_items_list);
        current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list.Add(current_go);
    }

    public void ShowMoveInstruction(string action_name, int knowledge_level, int default_time, string text_annotation)
    {
        Debug.Log("Show move instruction for " + action_name);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);

        // Find move position and orientation
        GameObject move_pos = null;
        foreach (Transform item in current_assembly_GO.transform)
        {
            if (item.name == action_name)
            {
                move_pos = item.gameObject;
                move_pos.SetActive(true);
                active_items.Add(move_pos);
            }
        }
        if(move_pos == null)
        {
            Debug.LogError(action_name + " not found in assembly items -> move instruction cannot be shown");
        }

        // Group all finished GO and move them to the new position
        // GameObject existing_assembly  = new GameObject("ExistingAssembly");
        GameObject first_object = current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list[0];
        first_object.SetActive(true);
        current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list.Remove(first_object);

        foreach (GameObject item in current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list)
        {
            item.transform.SetParent(first_object.transform);
            item.SetActive(true);
            item.GetComponent<ObjectInteractions>().RemoveUnnecessaryInformation();
        }
        first_object.transform.position = move_pos.transform.position;
        first_object.transform.rotation = move_pos.transform.rotation;
        first_object.transform.SetParent(move_pos.transform);
        current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list = new List<GameObject>();
        current_assembly_GO.GetComponent<AssemblyOrganisation>().finished_items_list.Add(move_pos);

        // Reload the miniature to include the changes
        GenerateMiniature(current_assembly_GO);

        if (knowledge_level == 1)
        {
            current_action_display.GetComponent<Text>().text = "Move";
            annotation.GetComponent<Text>().text = text_annotation;
            annotation.GetComponent<UI_BackgroundImage>().annotation_change = true;
            ShowAssemblyPosition(assembly_info_material_2, move_pos.name, disable_afterwards: true, change_material: true);
            ShowPositionMiniature(move_pos.name);
        }
        else if (knowledge_level == 2)
        {
            current_action_display.GetComponent<Text>().text = "Move";
            GameObject action_go = ShowAssemblyPosition(assembly_info_material_2, action_name, disable_afterwards: true, change_material: true);
            RemoveAssemblyHints(action_go);
            ShowPositionMiniature(action_name);
        }
        else if (knowledge_level == 3)
        {
            current_action_display.GetComponent<Text>().text = "Move";
            ShowPositionMiniature(action_name);
        }
        else if (knowledge_level == 4)
        {

        }
        else
        {
            Debug.LogWarning("Unknown level " + knowledge_level);
        }
    }

    public void ShowToolUsage(string action_name, int knowledge_level, int default_time, string text_annotation)
    {
        Debug.Log("Show tool usage instruction for " + action_name);
        feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(default_time);

        if (knowledge_level == 1)
        {
            current_action_display.GetComponent<Text>().text = "Assemble with tool";
            annotation.GetComponent<Text>().text = text_annotation;
            annotation.GetComponent<UI_BackgroundImage>().annotation_change = true;
            ShowAssemblyPosition(assembly_info_material_2, action_name, disable_afterwards: true, change_material: true);
            ShowPositionMiniature(action_name);
        }
        else if (knowledge_level == 2)
        {
            current_action_display.GetComponent<Text>().text = "Assemble with tool";
            GameObject action_go = ShowAssemblyPosition(assembly_info_material_2, action_name, disable_afterwards: true, change_material: true);
            RemoveAssemblyHints(action_go);
            ShowPositionMiniature(action_name);
        }
        else if (knowledge_level == 3)
        {
            current_action_display.GetComponent<Text>().text = "Assemble with tool";
            ShowPositionMiniature(action_name);
        }
        else if (knowledge_level == 4)
        {

        }
        else
        {
            Debug.LogWarning("Unknown level " + knowledge_level);
        }        
    }

    private GameObject ShowAssemblyPosition(Material material, string item_name, bool disable_afterwards, bool change_material)
    {
        GameObject current_go = FindGameobject(item_name, current_assembly_GO.GetComponent<AssemblyOrganisation>().main_items_list);
        current_go.gameObject.SetActive(true);
        active_items.Add(current_go.gameObject);
        ShowObjectPosition(current_go.gameObject, material, disable_afterwards, change_material);
        return current_go.gameObject;
    }

    private void RemoveAssemblyHints(GameObject current_item)
    {
        foreach (Transform part in current_item.transform)
        {
            if (part.name.Contains("Animation"))
            {
                part.gameObject.SetActive(false);
                disabled_items.Add(part.gameObject);
            }
            if (part.name.Contains("Text"))
            {
                part.gameObject.SetActive(false);
                disabled_items.Add(part.gameObject);
            }
        }
    }

    private void ShowPositionMiniature(string item_name)
    {
        assembly_miniature.SetActive(true);
        Debug.Log("Show part in miniature: " + item_name);
        GameObject current_mini_part = assembly_miniature.transform.Find(item_name).gameObject;
        ShowObjectPosition(current_mini_part, assembly_info_material_1, disable_afterwards: false, change_material: true);

        // Activate holder if existing
        if(assembly_miniature_holder != null)
        {
            assembly_miniature_holder.SetActive(true);
        }
        else
        {
            Debug.Log("No holder for miniature found");
        }
    }

    public void ShowPoints(int current_points)
    {
        current_point_display.GetComponent<Text>().text = current_points.ToString();
        float ratio = current_points / max_points;

        if (ratio > 0.8f)
        {
            current_point_display.GetComponent<Text>().color = Color.green;
        }
        else if (ratio > 0.4f)
        {
            current_point_display.GetComponent<Text>().color = Color.cyan;
        }
        else
        {
            current_point_display.GetComponent<Text>().color = Color.yellow;
        }
    }

    public void FinishStep()
    {
        uncompleted_steps[0].GetComponent<Image>().color = Color.green;
        uncompleted_steps.RemoveAt(0);
    }

    public void FinishJob()
    {
        current_assembly_GO.GetComponent<ObjectInteractions>().ActivateAllChildren();

        final_assembly_green = Instantiate(
            original: current_assembly_GO,
            position: current_assembly_GO.transform.position,
            rotation: current_assembly_GO.transform.rotation,
            parent: current_assembly_GO.transform.parent);
        Destroy(current_assembly_GO);

        final_assembly_green.GetComponent<ObjectInteractions>().RemoveUnnecessaryInformation();

        final_assembly_green.GetComponent<ObjectInteractions>().ChangeMaterial(finished_info_material);
        task_finished.GetComponent<AudioSource>().Play();
        current_action_display.GetComponent<Text>().text = "Task finished: Answer Questionnaire";

        // client.GetComponent<Connection_noJson>().SendInformation("{finished}");
    }

    private void ShowObjectPosition(GameObject current_object, Material material, bool disable_afterwards, bool change_material)
    {
        current_object.SetActive(true);
        if(change_material)
        {
            if (current_object.GetComponent<ObjectInteractions>() == null)
            {
                current_object.AddComponent<ObjectInteractions>();
            }
            current_object.GetComponent<ObjectInteractions>().ChangeMaterial(material);
            optically_changed_parts.Add(current_object);
        }
        if (disable_afterwards)
        {
            active_items.Add(current_object);
        }

    }

    private void GenerateMiniature(GameObject original_go)
    {
        if (assembly_miniature != null)
        {
            Destroy(assembly_miniature);
        }
        assembly_miniature = Instantiate(original_go, new Vector3(0, 0, 0), original_go.transform.rotation, assembly_presentation.transform);
        assembly_miniature.GetComponent<ObjectInteractions>().RemoveUnnecessaryInformation();
        foreach (Transform part in assembly_miniature.transform)
        {
            if (part.name.Contains("AssemblyHolder"))  // Find assembly holder if existing
            {
                Debug.Log("AssemblyHolder for miniature found");
                assembly_miniature_holder = part.gameObject;
            }
        }
        assembly_miniature.transform.localPosition = new Vector3(0, 0, 0);
        assembly_miniature.transform.localScale = 0.5f * assembly_miniature.transform.localScale;
    }

    public void ResetWorkplace()
    {
        if (object_presentation.transform.childCount > 0)
        {
            foreach (Transform child in object_presentation.transform)
            {
                Destroy(child.gameObject);
            }
            object_presentation.transform.DetachChildren();  // Remove children from parent, otherwise childCount is not working in same frame
        }
        if (active_items.Count() > 0)
        {
            foreach (GameObject item in active_items)
            {
                item.SetActive(false);
            }
        }
        if(optically_changed_parts.Count() > 0)
        {
            foreach (GameObject part in optically_changed_parts.ToList())
            {
                part.GetComponent<ObjectInteractions>().ResetMaterial();
                optically_changed_parts.Remove(part);
            }
        }
        if(assembly_miniature != null)
        {
            assembly_miniature.SetActive(false);
        }
        if(final_assembly_green != null)
        {
            Destroy(final_assembly_green);
        }
        active_items.Clear();
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetNotifications();
        annotation.GetComponent<Text>().text = "";
        annotation.GetComponent<UI_BackgroundImage>().annotation_change = true;
        current_action_display.GetComponent<Text>().text = "";
    }

    public GameObject FindGameobject(string name, List<GameObject> gameobject_list)
    {
        foreach (GameObject obj in gameobject_list)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        Debug.LogWarning("Gameobject " + name + " not found");
        return null;
    }

    private GameObject ShowPickPrefab(GameObject prefab, string display_text)
    {
        // Instantiate prefab and set parent
        GameObject displayed_item = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Vector3 original_scale = displayed_item.transform.localScale;
        displayed_item.transform.parent = object_presentation.transform;
        displayed_item.transform.localRotation = prefab.transform.rotation;
        displayed_item.transform.localScale = original_scale * pick_prefab_scale;

        // Check if several pick options exist
        int number_pick_options = object_presentation.transform.childCount;
        Vector3 offset = new Vector3(0.5f, 0, 0);

        if (number_pick_options == 1)  // Show prefab at first pick position
        {
            displayed_item.transform.localPosition = new Vector3(0, 0, 0);

            current_action_display.GetComponent<Text>().text = display_text;
        }
        else  // Show prefab at subsequent pick position
        {
            Debug.Log("Number of pick options: " + number_pick_options.ToString());
            int movement = number_pick_options - 1;
            displayed_item.transform.localPosition = new Vector3(0, 0, 0) + movement * offset;

            current_action_display.GetComponent<Text>().text = "Pick objects";
        }

        // Add properties
        if (displayed_item.GetComponent<ObjectInteractions>() == null)
        {
            displayed_item.AddComponent<ObjectInteractions>();
        }

        return displayed_item;
    }

    private GameObject FindPrefab(string path_name, string prefab_name)
    {
        GameObject prefab = (GameObject)Resources.Load(path_name, typeof(GameObject));
        if (prefab == null)
        {
            Debug.LogWarning("Prefab not found:" + prefab_name);
        }
        return prefab;
    }
}
