using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Admin_PropertySelection : MonoBehaviour {

    private bool show_ui = false;
    private GameObject client;
    private GameObject feedback_canvas;

    private int margins = 10;
    private int box_width = 200;
    private int box_height;
    private int box_x0;

    private bool tmp_gamification = true;
    private int tmp_level = 5;
    private string tmp_username = "Username";

    [HideInInspector] public string[] selectionLevels_5 = { "Level 0", "Level 1", "Level 2", "Level 3", "Level 4", "Auto" };
    [HideInInspector] public int user_level = 5;
    [HideInInspector] public bool gamification = true;
    [HideInInspector] public string user_name = "No-User";

    private GameObject username_go;


    private bool setup_mode = false;
    private bool prev_setup_mode = false;
    private GameObject assemblies;
    GameObject firstActiveGameObject;

    // Regex pattern = new Regex(@"^\w+$");

    void OnEnable()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    void Start()
    {
        client = GameObject.Find("Client");
        feedback_canvas = GameObject.Find("FeedbackCanvas");
        username_go = GameObject.Find("Username");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            show_ui = !show_ui;

            // Find active assembly
            assemblies = GameObject.Find("Assemblies");
            for (int n = 0; n < assemblies.transform.childCount; n++)
            {
                if (assemblies.transform.GetChild(n).gameObject.activeSelf == true)
                {
                    firstActiveGameObject = assemblies.transform.GetChild(n).gameObject;
                }
            }
        }
    }

    void OnGUI()
    {
        if(show_ui)
        {
            // Make a background box
            box_x0 = Screen.width - box_width - margins;
            box_height = Screen.height - 2 * margins;
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Admin Interface");

            // Add elements
            tmp_username = GUI.TextField(new Rect(box_x0 + margins, 35, box_width - 2 * margins, 20), tmp_username, 16);
            int i = 0;
            tmp_gamification = GUI.Toggle(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), tmp_gamification, "Gamification Enabled");
            i++;
            GUI.Label(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Static Level Selection");
            i++;
            tmp_level = GUI.SelectionGrid(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 120), tmp_level, selectionLevels_5, 1);
            i += 6;

            // Apply changes if save button is pressed
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Save"))
            {
                if (tmp_username != user_name)
                {
                    user_name = tmp_username;
                    client.GetComponent<Connection_noJson>().SendInformation("username[" + user_name + "]");
                    username_go.GetComponent<Text>().text = user_name;
                }

                if (tmp_gamification != gamification)  // Activate or deactivate game elements
                {
                    gamification = tmp_gamification;
                    if(gamification)
                    {
                        feedback_canvas.GetComponent<UI_FeedbackHandler>().EnableGamification();
                    }
                    else
                    {
                        feedback_canvas.GetComponent<UI_FeedbackHandler>().DisableGamification();
                    }
                }

                if (tmp_level != user_level)
                {
                    user_level = tmp_level;
                    client.GetComponent<Connection_noJson>().SendInformation("[level:" + user_level + "]");
                }
            }

            // Setup mode to allow changed at the MWS without analyzing the sensor signals
            setup_mode = GUI.Toggle(new Rect(box_x0 + margins, 400 + 25 * 0, box_width - 2 * margins, 20), setup_mode, "Setup-Mode");
            if (prev_setup_mode != setup_mode)
            {
                prev_setup_mode = setup_mode;
                if (setup_mode)
                {
                    client.GetComponent<Connection_noJson>().SendInformation("{setup}");
                    firstActiveGameObject.SetActive(true);
                    foreach (Transform item in firstActiveGameObject.transform)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                else
                {
                    client.GetComponent<Connection_noJson>().SendInformation("{ready}");
                    firstActiveGameObject.SetActive(false);
                    foreach (Transform item in firstActiveGameObject.transform)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }

            if (GUI.Button(new Rect(box_x0 + margins, 400 + 25 * 2, box_width - 2 * margins, 20), "Reset Order"))
            {
                client.GetComponent<Connection_noJson>().SendInformation("[resetOrder]");
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetFeedbackElements();
            }
        }        
    }
}
