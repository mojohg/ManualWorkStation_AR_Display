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

    private int sellevel = 0;
    private int prev_sellevel = 0;
    private string[] levels = new string[] { "Level 0", "Level 1", "Level 2", "Level 3", "Level 4" };

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
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Test feedback elements");
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
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Set max points"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().SetMaxPoints(100);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Show current points"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowPoints(50);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Show quality rate"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().InitializeQualityRate(80.0f, 60.0f);
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowQualityRate(0.77777777777777f);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Show time rate"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().InitializeTimeRate(80.0f, 60.0f);
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowTimeRate(0.5f);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Finish step"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().FinishStep(true);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "LevelUp"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayLevelup();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "PerfectRun"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayPerfectRun();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "NiceRun"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayNiceRun();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "FinishedRun"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayFinishedRun();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Point Popup"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayPopup("Yeah +2", 0, 255, 0);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Great Time"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayGoodTime();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Time Success"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayTimeSuccess();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Repeated Time Success"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayRepeatedTimeSuccess();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Training finished"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().DisplayTrainingFinished();
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Wrong action"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().NotifyWrongAction();
            }
            i++; if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Start timer"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().StartTimer(60);
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Reset game elements"))
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ResetFeedbackElements();
            }
            i++;
            sellevel = GUI.SelectionGrid(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 40), sellevel, levels, 3);
            i += 2;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Reset Scene"))
            {
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.name);
            }

            // Execute logic of non-button elements
            if (sellevel != prev_sellevel)
            {
                feedback_canvas.GetComponent<UI_FeedbackHandler>().ShowLevel(sellevel);
                prev_sellevel = sellevel;
            }
        }
    }
}
