using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotation_x;
    public float rotation_y; 
    public float rotation_z = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(rotation_x, rotation_y, rotation_z * Time.deltaTime); //rotates 50 degrees per second around z axis
    }
}
