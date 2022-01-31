using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Admin_PropertySelection : MonoBehaviour {

    private bool show_ui = false;
    private GameObject camera;
    private GameObject basic_environment;
    private GameObject client;

    private int margins = 10;
    private int box_width = 100;
    private int box_height;
    private int box_x0;

    private string[] selectionViews = { "PIP", "Spectator", "User" };  // Picture in Picture
    [HideInInspector] public string[] selectionLevels_5 = { "Level 0", "Level 1", "Level 2", "Level 3", "Level 4", "Auto" };

    private int tmp_level = 5;
    private bool tmp_gravity = false;
    private bool tmp_gamification = true;
    private bool tmp_standing = true;
    private string tmp_username = "Username";
    private int tmp_view = 0;

    private int selectedView = 0;
    [HideInInspector] public int userLevel = 5;
    [HideInInspector] public bool gravity = false;
    [HideInInspector] public bool gamification = true;
    public bool standing = true;
    public string username = "Username";

    private GameObject spectator_camera_user;
    private GameObject spectator_camera_full;
    private GameObject player_camera;

    Regex pattern = new Regex(@"^\w+$");

    void OnEnable()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyProperties();
    }

    void Start()
    {
        client = GameObject.Find("Client");
        ApplyProperties();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Settings");

            // Add elements
            tmp_username = GUI.TextField(new Rect(box_x0 + margins, 40, box_width - 2 * margins, 20), tmp_username, 16);
            tmp_gamification = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 1, box_width - 2 * margins, 20), tmp_gamification, "Gamification");
            tmp_standing = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 2, box_width - 2 * margins, 20), tmp_standing, "Standing");
            tmp_gravity = GUI.Toggle(new Rect(box_x0 + margins, 60 + 20 * 3, box_width - 2 * margins, 20), tmp_gravity, "Gravity");
            tmp_level = GUI.SelectionGrid(new Rect(box_x0 + margins, 160, box_width - 2 * margins, 120), tmp_level, selectionLevels_5, 1);

            // Apply changes if save button is pressed
            if (GUI.Button(new Rect(box_x0 + margins, 300, box_width - 2 * margins, 20), "Save"))
            {
                if (tmp_username != username)
                {
                    if(pattern.IsMatch(tmp_username))
                    {
                        username = tmp_username;
                        // Todo: Set user
                        //userProfileRequest.action_type = "setUser";
                        //userProfileRequest.user_name = username;
                        //server.GetComponent<Server>().SendUserInteractions(userProfileRequest);
                    }
                    else
                    {
                        tmp_username = "";
                    }
                }

                if (tmp_gamification != gamification)  // Activate or deactivate game elements
                {
                    gamification = tmp_gamification;
                    //ToDo
                    // server.GetComponent<GamificationHandler>().Control_Gamification(gamification);
                }

                if (tmp_level != userLevel)
                {
                    userLevel = tmp_level;
                    //userProfileRequest.action_type = "setLevel";
                    //server.GetComponent<Server>().SendUserInteractions(userProfileRequest);
                }
            }

            tmp_view = GUI.SelectionGrid(new Rect(box_x0 + margins, 350, box_width - 2 * margins, 60), tmp_view, selectionViews, 1);
            if(tmp_view != selectedView)
            {
                selectedView = tmp_view;

                switch(selectedView)
                {
                    case 0:  // Picture in Picture
                        spectator_camera_user.SetActive(true);
                        spectator_camera_full.SetActive(true);

                        spectator_camera_full.GetComponent<Camera>().depth = 0;
                        spectator_camera_user.GetComponent<Camera>().depth = 1;
                        player_camera.GetComponent<Camera>().depth = -1;
                        break;
                    case 1:  // Spectator view
                        spectator_camera_user.SetActive(false);
                        break;
                    case 2:  // User view
                        spectator_camera_user.SetActive(false);
                        spectator_camera_full.SetActive(false);
                        break;
                }
            }

            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 0, box_width - 2 * margins, 20), "Reset"))
            {
                if(SceneManager.GetActiveScene().name.Contains("ManualWorkstation"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    // ToDo
                    //userProfileRequest.action_type = "resetOrder";
                    //this.GetComponent<Server>().SendUserInteractions(userProfileRequest);
                }
            }
            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 1, box_width - 2 * margins, 20), "Overview"))
            {
                SceneManager.LoadScene(0);
            }
            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 2, box_width - 2 * margins, 20), "Controller"))
            {
                SceneManager.LoadScene(1);
            }
            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 3, box_width - 2 * margins, 20), "Training"))
            {
                SceneManager.LoadScene(2);
            }
            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 4, box_width - 2 * margins, 20), "Hide UI"))
            {
                show_ui = false;
            }

            if (GUI.Button(new Rect(box_x0 + margins, 430 + 25 * 6, box_width - 2 * margins, 20), "Quit"))
            {
                Application.Quit();
                Debug.Log("Quit is ignored in the editor");
            }
        }        
    }

        public void ApplyProperties()
    /*Update new scene according to properties of admin-ui*/
    {
        //this.gameObject.GetComponent<GamificationHandler>().Control_Gamification(gamification);
    }
}
