using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandling : MonoBehaviour
{
    private Material mat_red;
    private Material mat_green;
    private Material mat_blue;


    // Start is called before the first frame update
    void Start()
    {
        mat_red = (Material)Resources.Load("Materials/Red", typeof(Material));
        mat_green = (Material)Resources.Load("Materials/Green", typeof(Material));
        mat_blue = (Material)Resources.Load("Materials/Blue", typeof(Material));

    }

    public void ChangeMaterial(string color)
    {
        if (this.GetComponent<MeshRenderer>() != null)
        {
            if(color == "red")
            {
                this.GetComponent<MeshRenderer>().material = mat_red;
            }
            else if (color == "green")
            {
                this.GetComponent<MeshRenderer>().material = mat_green;
            }
            else if (color == "blue")
            {
                this.GetComponent<MeshRenderer>().material = mat_blue;
            }
            
        }
    }
}
