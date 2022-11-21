using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Move_linear : MonoBehaviour
{
    public float distance = 5;
    public float speed = 0.1f;
    public bool x_movement = false;
    public bool y_movement = false;
    public bool z_movement = false;

    public int direction = 1;
    
    private Vector3 initial_pos;
    private float current_distance = 0;

    // Start is called before the first frame update
    void Start()
    {
        initial_pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(current_distance);
        if (z_movement)
        {
            if (current_distance > (distance * 1/speed))
            {
                this.transform.position = initial_pos;
                current_distance = 0;
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime * direction);
        }

        current_distance++;
    }
}
