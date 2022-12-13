using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpecialEffects : MonoBehaviour
{
    public GameObject text;  // Text is optional
    public bool keep_text = false;

    // Start is called before the first frame update
    void Start()
    {
        if (text != null)
        {
            text.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSpecialEffect()
    {
        // TODO: Start Special Effect

        if (text != null)
        {
            text.SetActive(true);
        }
        if(this.gameObject.activeInHierarchy)
        {
            this.GetComponent<AudioSource>().Play();
        }        
    }

    public void OnParticleSystemStopped()
    {
        if (text != null && keep_text == false)
        {
            text.SetActive(false);
        }
    }
}
