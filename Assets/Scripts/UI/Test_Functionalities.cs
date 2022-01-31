using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Test_Functionalities : MonoBehaviour {

    private bool show_ui = false;

    private int margins = 10;
    private int box_width = 200;
    private int box_height;
    private int box_x0;

    private GameObject client;

    Regex pattern = new Regex(@"^\w+$");

    public GameObject user_canvas;
    public GameObject popup;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Test");

            // Add elements

            if (GUI.Button(new Rect(box_x0 + margins, 40, box_width - 2 * margins, 20), "Load progress bar"))
            {
                client.GetComponent<MessageHandler>().InitializeSteps(10);
            }
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * 0, box_width - 2 * margins, 20), "LevelUp"))
            {
                if (user_canvas == null)
                {
                    user_canvas = GameObject.Find("Canvas");
                }
                if (popup == null)
                {
                    popup = GameObject.Find("PointPopupParent");
                }

                if (user_canvas != null && popup != null)
                {
                    user_canvas.GetComponent<User_UI_Feedback>().DisplayPointPopup("-2", 1f, 0f, 0f);
                }
            }
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * 1, box_width - 2 * margins, 20), "Point Info"))
            {
                
            }
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * 2, box_width - 2 * margins, 20), "Training finished"))
            {
                
            }
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * 3, box_width - 2 * margins, 20), "Assembly feedback"))
            {
                
            }
        }        
    }
}
