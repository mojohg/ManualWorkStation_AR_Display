using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// The class ObjectInteractions contains basic methods to change object properties of GameObjects and to highlight specific parts.
/// </summary>

public class ObjectInteractions : MonoBehaviour {

    private int number_children = 0;
    [HideInInspector] public GameObject arrow;
    public Material initial_material;
    private Material current_material;
    private Shader standardShader;
    private Material mat_grey;

    void Awake()
    {
        mat_grey = (Material)Resources.Load("Materials/StandardGrey", typeof(Material));

        if (this.gameObject.GetComponent<Renderer>() != null)  // Store own material
        {
            initial_material = this.GetComponent<MeshRenderer>().material;
        }
        else if (this.transform.childCount > 0)  // Use material of first child if possible
        {
            Transform child = this.transform.GetChild(0);
            if (child.gameObject.GetComponent<Renderer>() != null)
            {
                initial_material = child.GetComponent<MeshRenderer>().material;
            }
            else  // Use standard material
            {
                initial_material = mat_grey;
            }
        }
        else  // Use standard material
        {
            initial_material = mat_grey;
        }
    }

    
    void Start()
    {
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
            else
            {
                child.gameObject.AddComponent<ObjectInteractions>();
                child.gameObject.GetComponent<ObjectInteractions>().ChangeMaterial(material);
            }
        }
    }

    public void ActivateAllChildren()
    {
        if (this.transform.childCount == 0)  // Check for child objects in assemblies
        {
            return;
        }

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<ObjectInteractions>() != null)
            {
                child.gameObject.SetActive(true);
                child.gameObject.GetComponent<ObjectInteractions>().ActivateAllChildren();
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

    public void ResetMaterial()
    {
        ChangeMaterial(initial_material);     
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
