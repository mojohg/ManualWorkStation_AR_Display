using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameElements_Selection : MonoBehaviour {

    private bool show_ui = false;

    // UI properties
    private int margins = 10;
    private int box_width = 200;
    private int box_height;
    private int box_x0;

    // Temporary variables for selection process
    private bool tmp_points = true;
    private bool tmp_time = true;
    private bool tmp_quality = true;
    private bool tmp_level = true;
    private bool tmp_popup = true;

    // Variables for selection process
    [HideInInspector] public bool points = true;
    [HideInInspector] public bool time = true;
    [HideInInspector] public bool quality = true;
    [HideInInspector] public bool level = true;
    [HideInInspector] public bool popup = true;

    // GameObjects in scene
    public GameObject go_quality;
    public GameObject go_time;
    public GameObject go_points;
    public GameObject go_popup;
    public GameObject go_level;

    // Collection of elements
    private List<GameObject> gameobjects;

    Regex pattern = new Regex(@"^\w+$");

    void OnEnable()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        points = true;
        time = true;
        quality = true;
        level = true;
        popup = true;
    }

    void Start()
    {
        // Create lists with all elements for reset purposes

        gameobjects.Add(go_quality);
        gameobjects.Add(go_time);
        gameobjects.Add(go_points);
        gameobjects.Add(go_popup);
        gameobjects.Add(go_level);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
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
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Gamification Cockpit");

            // Add element selection
            tmp_points = GUI.Toggle(new Rect(box_x0 + margins, 35, box_width - 2 * margins, 20), tmp_points, "Show Points");
            int i = 0;
            tmp_time = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * i, box_width - 2 * margins, 20), tmp_time, "Show Timer");
            i++;
            tmp_quality = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * i, box_width - 2 * margins, 20), tmp_quality, "Show Quality");
            i++;
            tmp_level = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * i, box_width - 2 * margins, 20), tmp_level, "Show Level");
            i++; 
            tmp_popup = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * i, box_width - 2 * margins, 20), tmp_popup, "Show Popup");
            i++;

            // Apply changes if save button is pressed -> Activate or deactivate game elements
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 20 * 6, box_width - 2 * margins, 20), "Save"))
            {
                if (tmp_points != points)
                {
                    points = tmp_points;
                    go_points.SetActive(points);
                }

                if (tmp_time != time)
                {
                    time = tmp_time;
                    go_time.SetActive(time);
                }

                if (tmp_quality != quality)
                {
                    quality = tmp_quality;
                    go_quality.SetActive(quality);
                }

                if (tmp_level != level)
                {
                    level = tmp_level;
                    go_level.SetActive(level);
                }

                if (tmp_popup != popup)
                {
                    popup = tmp_popup;
                    go_popup.SetActive(popup);
                }
            }

            if (GUI.Button(new Rect(box_x0 + margins, 60 + 20 * 8, box_width - 2 * margins, 20), "Reset Game Elements"))
            {
                foreach(GameObject element in gameobjects)
                {
                    element.SetActive(true);
                }

                tmp_points = true;
                tmp_time = true;
                tmp_quality = true;
                tmp_level = true;
                tmp_popup = true;

                points = true;
                time = true;
                quality = true;
                level = true;
                popup = true;
            }
        }        
    }
}
