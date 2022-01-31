using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameElements_Selection : MonoBehaviour {

    private bool show_ui = false;

    private int margins = 10;
    private int box_width = 200;
    private int box_height;
    private int box_x0;

    private bool tmp_point_display = true;
    private bool tmp_clock = true;
    private bool tmp_quality = true;
    private bool tmp_level = true;
    private bool tmp_performance_comparison = true;
    private bool tmp_own_goals = true;

    [HideInInspector] public bool point_display = true;
    [HideInInspector] public bool clock = true;
    [HideInInspector] public bool quality = true;
    [HideInInspector] public bool level = true;
    [HideInInspector] public bool performance_comparison = true;
    [HideInInspector] public bool own_goals = true;

    public GameObject go_point_display;
    public GameObject go_clock;
    public GameObject go_quality;
    public GameObject go_level;
    public GameObject go_performance_comparison;
    public GameObject go_own_goals;

    Regex pattern = new Regex(@"^\w+$");

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
        point_display = true;
        clock = true;
        quality = true;
        level = true;
        performance_comparison = true;
        own_goals = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.AltGr))
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

            // Add elements
            tmp_point_display = GUI.Toggle(new Rect(box_x0 + margins, 40, box_width - 2 * margins, 20), tmp_point_display, "Show Points");
            tmp_clock = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 0, box_width - 2 * margins, 20), tmp_clock, "Show Clock");
            tmp_quality = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 1, box_width - 2 * margins, 20), tmp_quality, "Show Quality");
            tmp_level = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 2, box_width - 2 * margins, 20), tmp_level, "Show Level");
            tmp_performance_comparison = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 3, box_width - 2 * margins, 20), tmp_performance_comparison, "Show Performance");
            // ToDo: Own goals

            Debug.Log(point_display);
            //Debug.Log(tmp_point_display);

            // Apply changes if save button is pressed
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 20 * 6, box_width - 2 * margins, 20), "Save"))
            {
                if (tmp_point_display != point_display)
                {
                    point_display = tmp_point_display;
                    go_point_display.SetActive(point_display);
                }

                if (tmp_clock != clock)  // Activate or deactivate game elements
                {
                    clock = tmp_clock;
                    go_clock.SetActive(clock);
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

                if (tmp_performance_comparison != performance_comparison)
                {
                    performance_comparison = tmp_performance_comparison;
                    // ToDo
                }
            }

            if (GUI.Button(new Rect(box_x0 + margins, 60 + 20 * 8, box_width - 2 * margins, 20), "Reset Game Elements"))
            {
                tmp_point_display = true;
                tmp_clock = true;
                tmp_quality = true;
                tmp_level = true;
                tmp_performance_comparison = true;
                tmp_own_goals = true;

                point_display = true;
                clock = true;
                quality = true;
                level = true;
                performance_comparison = true;
                own_goals = true;

                go_point_display.SetActive(true);
                go_clock.SetActive(true);
                go_quality.SetActive(true);
                go_level.SetActive(true);
            }
        }        
    }
}
