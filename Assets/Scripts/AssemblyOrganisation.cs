using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class AsseblyOrganisation is used to store the possible product variants.
/// </summary>

public class AssemblyOrganisation : MonoBehaviour
{
    private int numberVersions;
    public List<GameObject> main_items_list;
    public List<GameObject> finished_items_list;
    public float pick_prefab_scale = 1;


    // Use this for initialization
    void Start ()
    {
        // Get list of assembly versions
        numberVersions = this.transform.childCount;

        if (numberVersions > 0)
        {
            foreach (Transform child in this.transform)
            {
                main_items_list.Add(child.gameObject);
                // child.gameObject.SetActive(false);
            }
        }
    }
}
