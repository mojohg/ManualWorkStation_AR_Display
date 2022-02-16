using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Popup : MonoBehaviour {

    private TextMeshPro textMesh;
    private Color textColor;
    private Transform cameraTransform;
    private float disappearTime = 1f;
    private float fadeOutSpeed = 3f;
    private float moveYSpeed = 0.2f; 

    void Update()
    {
        PopUpUpdate();
    }

    public void Setup(string message, Color col)
    {
        textMesh = this.GetComponent<TextMeshPro>();
        textMesh.color = col;
        textColor = textMesh.color;
        textMesh.SetText(message);
    }

    private void PopUpUpdate()
    {
        this.transform.eulerAngles = new Vector3(90,-90,0);

        transform.position += new Vector3(0f, moveYSpeed * Time.deltaTime, 0f);

        disappearTime -= Time.deltaTime;
        if (disappearTime < 0f)
        {
            textColor.a -= fadeOutSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
