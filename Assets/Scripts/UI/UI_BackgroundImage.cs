using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackgroundImage : MonoBehaviour
{
    private GameObject background_image;
    public bool annotation_change = false;


    // Start is called before the first frame update
    void Start()
    {
        background_image = this.transform.Find("Background").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(annotation_change)
        {
            if (this.GetComponent<Text>().text == "")
            {
                background_image.SetActive(false);
            }
            else
            {
                background_image.SetActive(true);
            }
        }
    }
}
