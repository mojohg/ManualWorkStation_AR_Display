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

    public GameObject feedback_canvas;
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
        if (Input.GetKeyDown(KeyCode.T))
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
            if (feedback_canvas == null)
            {
                feedback_canvas = GameObject.Find("Canvas");
            }

            // Add elements

            if (GUI.Button(new Rect(box_x0 + margins, 35, box_width - 2 * margins, 20), "Load progress bar"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowNumberSteps(10);
            }
            int i = 0;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Finish step"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().FinishStep();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "LevelUp"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayLevelup();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Point Info"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayPopup("Yeah +2", 0, 255, 0);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Training finished"))
            {

            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Assembly feedback"))
            {

            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Reset Scene"))
            {
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.name);
            }
        }        
    }
}
