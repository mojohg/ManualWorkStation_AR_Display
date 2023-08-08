using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class CameraHandler : MonoBehaviour
{
    private CameraSetupProperties setup_data = new CameraSetupProperties();

        // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initial camera position: " + this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCameraSettings(float camera_pos_x, float camera_pos_y, float camera_pos_z, float orthographic_size)
    {
        // Debug.Log("Initial camera position: " + this.transform.position);
        this.transform.position = new Vector3(camera_pos_x, camera_pos_y, camera_pos_z);
        this.GetComponent<Camera>().orthographicSize = orthographic_size;
        // Debug.Log("New camera position: " + this.transform.position);
    }
}
