using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
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
    [HideInInspector] public int userLevel = 5;
    [HideInInspector] public bool gamification = true;
    [HideInInspector] public string username = "Username";

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            show_ui = !show_ui;
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
            tmp_gamification = GUI.Toggle(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), tmp_gamification, "Gamification Elements");
            i++;
            GUI.Label(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Static Level Selection");
            i++;
            tmp_level = GUI.SelectionGrid(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 120), tmp_level, selectionLevels_5, 1);
            i += 6;

            // Apply changes if save button is pressed
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Save"))
            {
                if (tmp_username != username)
                {
                    username = tmp_username;
                    client.GetComponent<Connection>().SendInformation("username[" + username + "]");
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

                if (tmp_level != userLevel)
                {
                    userLevel = tmp_level;
                    client.GetComponent<Connection>().SendInformation("{level:" + userLevel + "}");
                }
            }

            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 0, box_width - 2 * margins, 20), "Reset Order"))
            {
                client.GetComponent<Connection>().SendInformation("{resetOrder}");
            }
        }        
    }
}
