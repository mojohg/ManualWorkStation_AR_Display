using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInteractions : MonoBehaviour
{
    bool i = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(i == false)
            {
                this.GetComponent<ObjectHandling>().ChangeMaterial("red");
                i = !i;
            }
            else
            {
                this.GetComponent<ObjectHandling>().ChangeMaterial("green");
                i = !i;
            }
        }
    }
}
