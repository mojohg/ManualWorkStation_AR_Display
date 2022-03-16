using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using UnityEngine.SceneManagement;

/// <summary>
/// The class MessageHandler contains methods for parsing the messages coming from the IoT adapter and the MES.
/// </summary>

public class MessageHandler : MonoBehaviour
{
    private GameObject assemblies;
    private GameObject product_turns;
    private GameObject product_holder;
    private GameObject feedback_canvas;
    //public GameObject training_finished;

    public int current_knowledge_level;
    public string current_version;
    private string current_producttype;

    //private string box_name;
    //private string tool_holder_name;
    //private string led_name;
    //private string version_name;
    //private string name_box;
    //private GameObject current_led;
    //private GameObject server;
    //private GameObject boxes;    
    //private GameObject current_storage_area;
    private GameObject active_product_version;
    //private GameObject current_prefab;
    //private GameObject feedback_system;
    //private GameObject levelup;
    //private List<GameObject> tools = new List<GameObject>();
    private List<GameObject> holder_versions;
    private List<GameObject> product_versions;
    private List<GameObject> assembly_items;
    private List<GameObject> turn_versions;
    private List<GameObject> turn_operations;
    private List<GameObject> active_items = new List<GameObject>();
    //private List<GameObject> training_pages = new List<GameObject>();
    private Material assembly_info_material_1;
    private Material assembly_info_material_2;
    //private Material finished_info_material;
    //private Material toolpoint_material;
    //private Material invisible_material;
    //private CommunicationClass message = new CommunicationClass();

    // UI
    private GameObject current_point_display;
    private GameObject current_action_display;
    private GameObject max_point_display;
    private int max_points;
    private List<GameObject> uncompleted_steps = new List<GameObject>();
    private GameObject prefab_bar;
    private GameObject object_presentation;
    private GameObject assembly_presentation;

    // UI: Miniature assembly
    private GameObject total_assembly_miniature;
    private List<GameObject> optically_changed_parts = new List<GameObject>();

    // Properties of Assemblies
    private float sizing_factor_v3 = 1.5f;

    void Start()
    {
        assembly_info_material_1 = (Material)Resources.Load("Materials/InformationMaterial1", typeof(Material));
        assembly_info_material_2 = (Material)Resources.Load("InformationMaterial2", typeof(Material));
        //toolpoint_material = (Material)Resources.Load("InformationMaterialToolpoints", typeof(Material));
        //finished_info_material = (Material)Resources.Load("LedGreen", typeof(Material));
        //invisible_material = (Material)Resources.Load("Transparent", typeof(Material));
        //levelup = GameObject.Find("LevelUp");
        //feedback_system = GameObject.Find("UserFeedback_Canvas");

        // Find GO
        assemblies = GameObject.Find("Assemblies");
        product_turns = GameObject.Find("ProductTurns");
        product_holder = GameObject.Find("ProductHolder");
        feedback_canvas = GameObject.Find("FeedbackCanvas");
        object_presentation = GameObject.Find("NextObjects");
        assembly_presentation = GameObject.Find("TotalAssembly");
        current_point_display = feedback_canvas.transform.Find("PointDisplay/CurrentPoints").gameObject;
        max_point_display = feedback_canvas.transform.Find("PointDisplay/MaxPoints").gameObject;
        current_action_display = feedback_canvas.transform.Find("ActionInfo").gameObject;
    }

    public void InitializeVersion(string version_name)
    {
        current_version = version_name;  // Get V3.1
        current_producttype = current_version.Split('.')[0];  // Get V3
        Debug.Log("Load product version " + current_version);
        Debug.Log("Load product type " + current_producttype);
        current_action_display.GetComponent<Text>().text = "Load version " + current_version;

        product_versions = assemblies.GetComponent<AssemblyOrganisation>().main_items_list;
        holder_versions = product_holder.GetComponent<AssemblyOrganisation>().main_items_list;
        turn_versions = product_turns.GetComponent<AssemblyOrganisation>().main_items_list;
        try  // Load product holder
        {
            foreach (GameObject holder in holder_versions)
            {
                if (holder.name == current_version)
                {
                    holder.SetActive(true);
                }
                else
                {
                    holder.SetActive(false);
                }
            }
        }
        catch
        {
            Debug.LogWarning("No product holder specified for version " + current_version);
        }

        try // Load product cad
        {
            foreach (GameObject product in product_versions)
            {
                if (product.name == current_version)
                {
                    product.SetActive(true);

                    try  // Show miniature product
                    {
                        total_assembly_miniature = Instantiate(product, new Vector3(0, 0, 0), product.transform.rotation, assembly_presentation.transform);
                        foreach (Transform part in total_assembly_miniature.transform)
                        {
                            if(part.name.Contains("Toolpoint"))
                            {
                                Destroy(part.gameObject);
                            }
                        }
                        total_assembly_miniature.transform.localPosition = new Vector3(0, 0, 0);
                        total_assembly_miniature.transform.localScale = 0.5f * total_assembly_miniature.transform.localScale;
                    }
                    catch
                    {
                        Debug.LogWarning("Product version could not be displayed " + current_version);
                    }

                    assembly_items = product.GetComponent<AssemblyOrganisation>().main_items_list;  // Find GO of assembly
                    foreach (GameObject item in assembly_items)
                    {
                        item.SetActive(false);  // Deactivate GO that they are not visible
                    }
                    active_product_version = product;
                }
                else
                {
                    product.SetActive(false);
                }
            }
        }
        catch
        {
            Debug.LogWarning("Product assembly not found of version " + current_version);
        }

        try  // Load turn operations
        {
            foreach (GameObject ver in turn_versions)
            {
                if (ver.name == current_version)
                {
                    ver.SetActive(true);
                    turn_operations = ver.GetComponent<AssemblyOrganisation>().main_items_list;
                }
                else
                {
                    ver.SetActive(false);
                }
            }
        }
        catch
        {
            Debug.LogWarning("Product turn operations not found of version " + current_version);
        }
    }

    public void InitializeSteps(int number_steps)
    {
        Debug.Log("InitializeSteps: " + number_steps.ToString());
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
        Debug.Log("New work step -> reset support");
        ResetWorkplace();
    }

    public void ParsePerformanceMessage(PerformanceProperties message)  // TODO
    {
        Debug.Log("ParsePerformanceMessage");
        /*
        if(feedback_system == null)
        {
            feedback_system = GameObject.Find("UserFeedback_Canvas");
        }
        if(feedback_system != null)
        {
            feedback_system.GetComponent<User_UI_Feedback>().ShowPoints(message.total_points);
            feedback_system.GetComponent<User_UI_Feedback>().ShowPerformance(message.performance, 0f, 1f);
            feedback_system.GetComponent<User_UI_Feedback>().ShowLevel(message.total_level);

            if (message.message_text != "")
            {
                feedback_system.GetComponent<User_UI_Feedback>().DisplayPointPopup(message.message_text,
                message.message_color.r, message.message_color.g, message.message_color.b);
            }

            if (message.node_finished)
            {
                feedback_system.GetComponent<User_UI_Feedback>().FinishStep();
            }
        }
        if(message.level_up)
        {
            if(levelup == null)
            {
                levelup = GameObject.Find("LevelUp");
            }
            if(levelup != null)
            {
                levelup.GetComponent<User_Levelup>().ShowLevelUp();
            }            
        }*/
    }

    public void PickObject(string item_name, string led_color, int knowledge_level, int default_time)  // TODO: Level System
    {
        Debug.Log("Show pick instruction for " + item_name);
        current_action_display.GetComponent<Text>().text = "Pick Item";
        current_knowledge_level = knowledge_level;

        // Find and show prefab
        GameObject item_prefab = FindPrefab("Prefabs/Parts/" + current_producttype + "/" + item_name, item_name);
        if (item_prefab == null)
        {
            Debug.LogWarning("Prefab for " + item_name + " not found");
            return;
        }
        ShowPickPrefab(item_prefab);

        if (led_color == "red")  // Wrong pick
        {
            // source_wrong.Play();  ToDo: Use element from feedback GO

        }
        else if (led_color == "green")  // Correct pick
        {
        }
    }

    public void PickTool(string tool_name, string led_color, int knowledge_level, int default_time)  // TODO Level System & Wrong Pick
    {
        Debug.Log("Show pick instruction for " + tool_name);
        current_action_display.GetComponent<Text>().text = "Pick Tool";
        current_knowledge_level = knowledge_level;

        // Find prefab
        GameObject tool_prefab = FindPrefab("Prefabs/Tools/" + tool_name, tool_name);
        if (tool_prefab == null)
        {
            Debug.LogWarning("Prefab for " + tool_name + " not found");
            return;
        }

        if (led_color == "red")  // Wrong pick
        {
            // source_wrong.Play();  ToDo: Use element from feedback GO
            // TODO: Show tool with cross

        }
        else if (led_color == "green")  // Correct pick
        {
            ShowPickPrefab(tool_prefab);
        }
    }

    public void ReturnTool(int tool_level, int tool_number, string led_color, int knowledge_level, int default_time)  // TODO
    {
        /*current_knowledge_level = knowledge_level;

        if (current_knowledge_level == 0)
        {
            IEnumerable<GameObject> pages = training_pages.Where(obj => obj.name == "ReturnTool");
            foreach (GameObject page in pages)
            {
                page.SetActive(true);
            }
            left_controller.GetComponent<HighlightController>().HighlightTrigger();
            right_controller.GetComponent<HighlightController>().HighlightTrigger();
            ShowToolLeds(tool_level, tool_number, led_color, knowledge_level, default_time);
        }
        else
        {
            ShowToolLeds(tool_level, tool_number, led_color, knowledge_level, default_time);
        }*/
    }

    public void ShowAssemblyPosition(string item_name, int knowledge_level, int default_time)  // TODO: Level System
    {
        Debug.Log("Show assembly instruction for " + item_name);
        total_assembly_miniature.SetActive(true);
        current_action_display.GetComponent<Text>().text = "Assemble";

        // Highlight assembly position
        foreach (GameObject item in assembly_items)
        {
            if (item.name == item_name)
            {
                item.SetActive(true);
                active_items.Add(item);
                ShowObjectPosition(item, assembly_info_material_1, disable_afterwards:true);
                break;
            }
        }

        // Highlight position in miniature
        GameObject current_mini_part = total_assembly_miniature.transform.Find(item_name).gameObject;
        ShowObjectPosition(current_mini_part, assembly_info_material_1, disable_afterwards:false);
    }

    public void ShowToolUsage(string action_name, int knowledge_level, int default_time)  // TODO: Add animations
    {
        Debug.Log("Show tool usage instruction for " + action_name);
        total_assembly_miniature.SetActive(true);
        current_action_display.GetComponent<Text>().text = "Assemble with tool";

        // Highlight toolpoint
        foreach (GameObject item in assembly_items)
        {
            if (item.name == action_name)
            {
                item.SetActive(true);
                active_items.Add(item);
                ShowObjectPosition(item, assembly_info_material_1, disable_afterwards: true);
                break;
            }
        }
    }

    public void ShowInstructions(int knowledge_level, GameObject obj)  // TODO
    {
        /*total_assembly_miniature.SetActive(true);
        current_knowledge_level = knowledge_level;
        Transform toolpoint = obj.transform.Find("Toolpoint(Clone)");
        switch (knowledge_level)
        {
            case 0:
                if (toolpoint != null)
                {
                    if (toolpoint.gameObject.GetComponent<ObjectInteractions>() == null)
                    {
                        toolpoint.gameObject.AddComponent<ObjectInteractions>();  // Show toolpoint
                    }
                }
                ShowObjectPosition(obj, assembly_info_material_1);
                break;

            case 1:
                if (toolpoint != null)
                {
                    if (toolpoint.gameObject.GetComponent<ObjectInteractions>() == null)
                    {
                        toolpoint.gameObject.AddComponent<ObjectInteractions>();  // Show toolpoint
                    }
                }
                ShowObjectPosition(obj, assembly_info_material_1);
                break;

            case 2:
                if (toolpoint != null)
                {
                    if (toolpoint.gameObject.GetComponent<ObjectInteractions>() == null)
                    {
                        ShowObjectPosition(obj, assembly_info_material_1);  // Highlight components which interact with tool
                    }
                }
                else
                {
                    ShowObjectPosition(obj, assembly_info_material_2);  // Highlight assembly position
                }                
                break;

            case 3:
                if (toolpoint != null)
                {
                    if (toolpoint.gameObject.GetComponent<ObjectInteractions>() == null)
                    {
                        ShowObjectPosition(obj, assembly_info_material_1);  // Highlight components which interact with tool
                    }
                }
                else
                {
                    ShowObjectPosition(obj, assembly_info_material_2);  // Highlight assembly position
                }
                if (obj.GetComponent<AssembleObjects>() != null)
                {
                    obj.GetComponent<AssembleObjects>().CheckSupportNecessity();
                }
                break;
            case 4:
                if (obj.GetComponent<AssembleObjects>() != null)
                {
                    obj.GetComponent<AssembleObjects>().CheckSupportNecessity();
                }
                break;
        }*/
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

    public IEnumerator FinishJob()  // TODO
    {
        /*var generated_objects = GameObject.FindGameObjectsWithTag("GeneratedObject").ToList();
        var assembled_objects = GameObject.FindGameObjectsWithTag("AssembledObject").ToList();
        var all_objects = generated_objects.Union(assembled_objects).ToList();

        this.transform.Find("Audio_Finished").GetComponent<AudioSource>().Play();
        foreach (GameObject obj in all_objects)  // Show that operation is finished
        {
            obj.GetComponent<ObjectInteractions>().ChangeMaterial(finished_info_material);
        }
        yield return new WaitForSeconds(3);

        
        foreach (GameObject obj in all_objects)  // Remove finished product
        {
            Destroy(obj);
        }

        message.action_type = "finishOrder";
        server.GetComponent<Server>().SendUserInteractions(message);*/
        return null;
    }

    private void ShowObjectPosition(GameObject current_object, Material material, bool disable_afterwards)
    {
        if (current_object.GetComponent<ObjectInteractions>() != null)
        {
            current_object.GetComponent<ObjectInteractions>().ChangeMaterial(material);
            if(disable_afterwards)
            {
                active_items.Add(current_object);
            }
            else
            {
                optically_changed_parts.Add(current_object);
            }
        }
        else
        {
            current_object.AddComponent<ObjectInteractions>();
            current_object.GetComponent<ObjectInteractions>().ChangeMaterial(material);
            active_items.Add(current_object);
        } 
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
        total_assembly_miniature.SetActive(false);
        active_items.Clear();
        feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetNotifications();
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

    private void ShowPickPrefab(GameObject prefab)
    {
        // Instantiate prefab and set parent
        GameObject displayed_item = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Vector3 original_scale = displayed_item.transform.localScale;
        displayed_item.transform.parent = object_presentation.transform;
        displayed_item.transform.localScale = original_scale * sizing_factor_v3;

        // Check if several pick options exist
        int number_pick_options = object_presentation.transform.childCount;
        Vector3 offset = new Vector3(0, 0.5f, 0);

        if (number_pick_options == 1)  // Show prefab at first pick position
        {
            displayed_item.transform.localPosition = new Vector3(0, 0, 0);
        }
        else  // Show prefab at subsequent pick position
        {
            Debug.Log("Number of pick options: " + number_pick_options.ToString());
            int movement = number_pick_options - 1;
            displayed_item.transform.localPosition = new Vector3(0, 0, 0) + movement * offset;
        }

        // Add properties
        if (displayed_item.GetComponent<ObjectInteractions>() == null)
        {
            displayed_item.AddComponent<ObjectInteractions>();
        }
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
