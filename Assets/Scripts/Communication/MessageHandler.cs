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
    private GameObject object_presentation;
    //public GameObject training_finished;

    public int current_knowledge_level;
    public string current_version;

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
    //private List<GameObject> assembly_items;
    private List<GameObject> turn_versions;
    private List<GameObject> turn_operations;
    //private List<GameObject> active_items = new List<GameObject>();
    //private List<GameObject> training_pages = new List<GameObject>();
    //private Material assembly_info_material_1;
    //private Material assembly_info_material_2;
    //private Material finished_info_material;
    //private Material toolpoint_material;
    //private Material invisible_material;
    //private CommunicationClass message = new CommunicationClass();
    
    private AudioSource source_wrong;

    // UI
    private GameObject current_point_display;
    private GameObject max_point_display;
    private int max_points;
    private List<GameObject> uncompleted_steps = new List<GameObject>();
    private GameObject prefab_bar;

    void Start()
    {
        //assembly_info_material_1 = (Material)Resources.Load("InformationMaterial1", typeof(Material));
        //assembly_info_material_2 = (Material)Resources.Load("InformationMaterial2", typeof(Material));
        //toolpoint_material = (Material)Resources.Load("InformationMaterialToolpoints", typeof(Material));
        //finished_info_material = (Material)Resources.Load("LedGreen", typeof(Material));
        //invisible_material = (Material)Resources.Load("Transparent", typeof(Material));
        source_wrong = this.transform.Find("Audio_Wrong").GetComponent<AudioSource>();
        //levelup = GameObject.Find("LevelUp");
        //feedback_system = GameObject.Find("UserFeedback_Canvas");

        // Find GO
        assemblies = GameObject.Find("Assemblies");
        product_turns = GameObject.Find("ProductTurns");
        product_holder = GameObject.Find("ProductHolder");
        feedback_canvas = GameObject.Find("Canvas");
        object_presentation = GameObject.Find("NextObjects");
        current_point_display = feedback_canvas.transform.Find("PointDisplay/CurrentPoints").gameObject;
        max_point_display = feedback_canvas.transform.Find("PointDisplay/MaxPoints").gameObject;

        // Load prefabs
        prefab_bar = (GameObject)Resources.Load("Prefabs/UI/bar", typeof(GameObject));
    }

    public void InitializeVersion(string version_name)
    {
        current_version = version_name;
        Debug.Log("Load product version " + current_version);
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
                        //assembly_items = product.GetComponent<AssemblyOrganisation>().main_items_list;
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

    public void InitializeSteps(int number_steps)  // TODO
    {
        Debug.Log("InitializeSteps: " + number_steps.ToString());
        GameObject progressBar = feedback_canvas.transform.Find("StepDisplay").gameObject;
        GameObject new_bar;

        for (int i = 0; i < number_steps; i++)
        {
            new_bar = Instantiate(prefab_bar, progressBar.transform);
            new_bar.name = "Bar_" + i;
            uncompleted_steps.Add(new_bar);
        }
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
        /*if(message.total_level == 4)
        {
            training_finished.GetComponent<User_TrainingFinished>().ShowFinished();
            return;
        }
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
        current_knowledge_level = knowledge_level;

        // Find prefab
        GameObject item_prefab = FindPrefab("Prefabs/Parts/" + current_version + "/" + item_name, item_name);
        if (item_prefab == null)
        {
            Debug.LogWarning("Prefab for " + item_name + " not found");
            return;
        }

        // Show object at specified position
        GameObject displayed_item = Instantiate(item_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        displayed_item.transform.parent = object_presentation.transform;
        displayed_item.transform.localPosition = new Vector3(0, 0, 0);
        displayed_item.transform.localScale = new Vector3(1, 1, 1);
        if (displayed_item.GetComponent<ObjectInteractions>() == null)
        {
            displayed_item.AddComponent<ObjectInteractions>();
        }

        if (led_color == "red")  // Wrong pick
        {
            source_wrong.Play();

        }
        else if (led_color == "green")  // Correct pick
        {
        }
    }

    public void PickTool(int tool_name,string led_color, int knowledge_level, int default_time)  // TODO
    {
        /*current_knowledge_level = knowledge_level;

        tool_holder_name = "Tool_" + tool_level + "_" + tool_number;
        GameObject tool_holder = GameObject.Find(tool_holder_name);
        if (tool_holder == null)
        {
            Debug.LogWarning(tool_holder_name + " not found");
            return;
        }
        GameObject tool = tool_holder.transform.GetChild(0).gameObject;
        if (tool == null)
        {
            Debug.LogWarning(" Tool not found");
            return;
        }
        if (tool.GetComponent<ObjectInteractions>() == null)
        {
            tool.AddComponent<ObjectInteractions>();
        }

        if(current_knowledge_level == 0)
        {
            IEnumerable<GameObject> pages = training_pages.Where(obj => obj.name == "PickTool");
            foreach (GameObject page in pages)
            {
                page.SetActive(true);
            }
            left_controller.GetComponent<HighlightController>().HighlightTrigger();
            right_controller.GetComponent<HighlightController>().HighlightTrigger();
            tool.GetComponent<ObjectInteractions>().ShowObjectBoundaries(Color.green, 1.06f);
        }
        else if (current_knowledge_level == 1)
        {
            tool.GetComponent<ObjectInteractions>().ShowObjectBoundaries(Color.green, 1.06f);
        }
        else
        {
            ShowToolLeds(tool_level, tool_number, led_color, knowledge_level, default_time);
        }*/
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

    public void ShowInstructions(int knowledge_level, GameObject obj)  // TODO
    {
        /*current_knowledge_level = knowledge_level;
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

    private void ShowObjectPosition(GameObject current_object, Material material)  // TODO
    {
        /*if (current_object.GetComponent<ToolObject>() != null)
        {
            foreach (GameObject point in current_object.GetComponent<ToolObject>().tool_objects_highlight)  // Highlight toolpoints
            {
                if (point.GetComponent<ObjectInteractions>() != null)
                {
                    point.GetComponent<ObjectInteractions>().ChangeMaterial(toolpoint_material);
                    active_items.Add(point);
                }                
            }
            if (current_object.GetComponent<ObjectInteractions>() != null)  // Highlight item which interacts with tool (e.g. screw)
            {
                current_object.GetComponent<ObjectInteractions>().ChangeMaterial(toolpoint_material);
            }
        }
        else  // Highlight object
        {
            if (current_object.GetComponent<ObjectInteractions>() != null)
            {
                current_object.GetComponent<ObjectInteractions>().ChangeMaterial(material);
                active_items.Add(current_object);
            }
            else
            {
                Debug.LogWarning("ObjectInteractions in Gameobject " + current_object.name + " not found");
            } 
        } */
    }

    public void ResetWorkplace() // TODO
    {
        foreach (Transform child in object_presentation.transform)
        {
            Destroy(child.gameObject);
        }

        // Reset assemblies
        /*foreach (Transform obj in active_product_version.transform)  
        {
            obj.gameObject.SetActive(false);
        }

        // Reset shader for all objects
        if (active_items != null)
        {
            foreach (GameObject obj in active_items)
            {
                obj.GetComponent<ObjectInteractions>().ChangeMaterial(invisible_material);
            }
        }
        active_items = new List<GameObject>();
        
        ResetTrainingPages();
        ResetStorage();
        ResetLeds();*/
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
