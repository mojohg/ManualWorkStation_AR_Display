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
    public int elapsed_frames = 0;
    public int frames_till_movement = 1000;

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
        elapsed_frames++;
        
        if(elapsed_frames > frames_till_movement)
        {
            if (x_movement)
            {
                if (current_distance > (distance * 1 / speed))
                {
                    this.transform.position = initial_pos;
                    current_distance = 0;
                }
                transform.Translate(Vector3.up * speed * direction);
            }
            if (y_movement)
            {
                if (current_distance > (distance * 1 / speed))
                {
                    this.transform.position = initial_pos;
                    current_distance = 0;
                }
                transform.Translate(Vector3.right * speed * direction);
            }
            if (z_movement)
            {
                if (current_distance > (distance * 1 / speed))
                {
                    this.transform.position = initial_pos;
                    current_distance = 0;
                }
                transform.Translate(Vector3.forward * speed * direction);
            }

            elapsed_frames = 0;
        }

        current_distance++;
    }
}
