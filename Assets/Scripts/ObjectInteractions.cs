using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// The class ObjectInteractions contains basic methods to change object properties of GameObjects and to highlight specific parts.
/// </summary>

public class ObjectInteractions : MonoBehaviour {

    private int number_children = 0;
    private GameObject client;
    [HideInInspector] public GameObject arrow;
    private GameObject arrow_prefab;
    private int rotation_help_limit = 40;
    private Material current_material;
    private Shader standardShader;

    void Start()
    {
        client = GameObject.Find("Client");
        arrow_prefab = (GameObject)Resources.Load("Prefabs/General/Arrow_1", typeof(GameObject));
        standardShader = Shader.Find("Standard");
        StoreCurrentProperties();
    }

    public void ChangeMaterial(Material material)
    {
        if (this.GetComponent<MeshRenderer>() != null)
        {
            this.GetComponent<MeshRenderer>().material = material;
        }
        
        if (this.transform.childCount == 0)  // Check for child objects in assemblies
        {
            return;
        }

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<ObjectInteractions>() != null)
            {
                child.gameObject.GetComponent<ObjectInteractions>().ChangeMaterial(material);
            }            
        }
    }

    public void ResetShader()
    {
        if (this.GetComponent<Renderer>() != null)
        {
            Renderer currentRenderer = this.GetComponent<Renderer>();
            foreach (Material mat in currentRenderer.materials)
            {
                mat.shader = standardShader;
            }               
        }

        number_children = this.transform.childCount;
        if (number_children > 0)
        {
            foreach (Transform child in this.transform)
            {
                if (child.GetComponent<ObjectInteractions>() != null && child.GetComponent<Renderer>() != null)
                {
                    child.gameObject.GetComponent<ObjectInteractions>().ResetShader();
                }
            }
        }
    }

    public void ShowObjectBoundaries(Vector4 outline_color, float outline_width)
    {
        if (this.GetComponent<Renderer>() != null)
        {
            this.GetComponent<ObjectInteractions>().HighlightContour(outline_color, outline_width);
        }
        
        number_children = this.transform.childCount;  // Check for child objects in assemblies
        if (number_children > 0)
        {
            foreach (Transform child in this.transform)
            {
                if (child.gameObject.GetComponent<ObjectInteractions>() != null && child.GetComponent<Renderer>() != null)
                {
                    child.gameObject.GetComponent<ObjectInteractions>().ShowObjectBoundaries(outline_color, outline_width);
                }                
            }
        }
    }

    private void HighlightContour(Vector4 outline_color, float outline_width)
    {
        Shader shaderOutline = Shader.Find("Unlit/Outline");
        Renderer currentRenderer = this.GetComponent<Renderer>();
        foreach(Material mat in currentRenderer.materials)
        {
            mat.shader = shaderOutline;
            mat.SetVector("_OutlineColor", outline_color);
            mat.SetFloat("_OutlineWidth", outline_width);
            mat.renderQueue = 3001;
        }
        
    }

    public void TransferUserInteraction(CommunicationClass user_interaction_response)
    {
        if (client != null)
        {
            // client.GetComponent<Server>().SendUserInteractions(user_interaction_response);
        }        
    }

    public void ShowDirection(Quaternion goal_rotation, float interpol_ratio, GameObject controller)
    {
        //if (this.gameObject.GetComponent<SpecialProperties>() != null)  // Do not show an arrow in case of rotation symmetry
        //{
        //    if (this.gameObject.GetComponent<SpecialProperties>().rotationSymmetry == true)
        //    {
        //        return;
        //    }
        //}

        if (arrow_prefab == null)
        {
            Debug.LogWarning("Arrow prefab not found.");
        }

        if (Quaternion.Angle(this.transform.rotation, goal_rotation) > rotation_help_limit)
        {
            if (arrow == null)
            {
                //arrow = Instantiate(arrow_prefab, this.transform.position, this.transform.rotation);
                //arrow.transform.SetParent(this.transform);
                arrow = Instantiate(arrow_prefab, controller.transform.position, controller.transform.rotation);
                arrow.transform.SetParent(controller.transform);
            }
            arrow.transform.rotation = Quaternion.Slerp(this.transform.rotation, goal_rotation, interpol_ratio);
        }
        else
        {
            UnshowDirection();
        }
    }

    public void UnshowDirection()
    {
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }

    public GameObject FindChild(string name)
    {
        GameObject found_child = null;
        if (this.transform.childCount > 0)
        {
            foreach (Transform child in this.transform)
            {
                if (child.name == name)
                {
                    found_child = child.gameObject;
                }
                else if (child.transform.childCount > 0)
                {
                    if (child.GetComponent<ObjectInteractions>() != null)
                    {
                        found_child = child.GetComponent<ObjectInteractions>().FindChild(name);
                    }
                }
            }
        }        
        return found_child;
    }

    public void StoreCurrentProperties()
    {
        if(this.GetComponent<MeshRenderer>() != null)
        {
            current_material = this.GetComponent<MeshRenderer>().material;
        }        
    }

    public void SetPreviousProperties()
    {
        this.GetComponent<MeshRenderer>().material = current_material;
    }
}
