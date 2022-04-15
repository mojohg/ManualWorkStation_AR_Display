using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using System;

public class Setup_Camera : MonoBehaviour {

    private bool show_ui = false;

    private int margins = 10;
    private int box_width = 200;
    private int box_height;
    private int box_x0;

    private float speed = 0.01f;

    private CameraSetupProperties new_camera_settings = new CameraSetupProperties();

    private bool setup_mode = false;
    private bool prev_setup_mode = false;

    public GameObject main_camera;
    private GameObject client;
    private GameObject assembly;

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
        assembly = GameObject.Find("V3.3");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
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
            GUI.Box(new Rect(box_x0, margins, box_width, box_height), "Adjust camera settings");

            // Check if required GO are available
            if (main_camera == null)
            {
                main_camera = GameObject.Find("MainCamera");
            }

            // Add elements
            setup_mode = GUI.Toggle(new Rect(box_x0 + margins, 35, box_width - 2 * margins, 20), setup_mode, "Setup-Mode");
            if (prev_setup_mode != setup_mode)
            {
                prev_setup_mode = setup_mode;
                if (setup_mode)
                {
                    client.GetComponent<Connection_noJson>().SendInformation("{setup}");
                    assembly.SetActive(true);
                    foreach (Transform item in assembly.transform)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                else
                {
                    client.GetComponent<Connection_noJson>().SendInformation("{ready}");
                    assembly.SetActive(false);
                    foreach (Transform item in assembly.transform)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            int i = 1;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Move left"))
            {
                main_camera.transform.position += transform.forward * speed;
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Move right"))
            {
                main_camera.transform.position += - transform.forward * speed;
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Move up"))
            {
                main_camera.transform.position += transform.right * speed;
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Move down"))
            {
                main_camera.transform.position += -transform.right * speed;
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Zoom in"))
            {
                //main_camera.transform.position += - transform.up * speed;
                main_camera.GetComponent<Camera>().orthographicSize -= 0.1f;
            }
            i++;
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Zoom out"))
            {
                //main_camera.transform.position += transform.up * speed;
                main_camera.GetComponent<Camera>().orthographicSize += 0.1f;
            }
            i++;
            
            if (GUI.Button(new Rect(box_x0 + margins, 60 + 25 * i, box_width - 2 * margins, 20), "Store settings"))
            {
                new_camera_settings.camera_pos_x = main_camera.transform.position.x;
                new_camera_settings.camera_pos_y = main_camera.transform.position.y;
                new_camera_settings.camera_pos_z = main_camera.transform.position.z;
                new_camera_settings.orthographic_size = main_camera.GetComponent<Camera>().orthographicSize;

                // Send camera information to client for storage
                client.GetComponent<Connection_noJson>().SendInformation("camera-x[" + main_camera.transform.position.x + "]");
                client.GetComponent<Connection_noJson>().SendInformation("camera-y[" + main_camera.transform.position.y + "]");
                client.GetComponent<Connection_noJson>().SendInformation("camera-z[" + main_camera.transform.position.z + "]");
                client.GetComponent<Connection_noJson>().SendInformation("camera-orthographic[" + main_camera.GetComponent<Camera>().orthographicSize + "]");
            }
        }
    }
}
