using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class CameraHandler : MonoBehaviour
{
    public string setup_info_path = @"\AR-Settings\" + "ar_camera_setup";
    private CameraSetupProperties setup_data = new CameraSetupProperties();

        // Start is called before the first frame update
    void Start()
    {
        try
        {
            setup_data = JsonConvert.DeserializeObject<CameraSetupProperties>(File.ReadAllText(setup_info_path));
            this.transform.position = new Vector3(setup_data.camera_pos_x, setup_data.camera_pos_y, setup_data.camera_pos_z);
            this.GetComponent<Camera>().orthographicSize = setup_data.orthographic_size;
        }
        catch (Exception)
        {
            Debug.Log("File for camera setup not found -> use standard properties");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
