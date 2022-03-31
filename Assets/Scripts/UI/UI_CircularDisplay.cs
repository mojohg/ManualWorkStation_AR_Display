using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_CircularDisplay : MonoBehaviour
{
    public Image uiFill;
    public Text uiText;

    private float max;
    private float threshold_y;
    private float threshold_r;

    private void Start()
    {
    }

    public void InitializeCircularDisplay(float threshold_yellow, float threshold_red)
    {
        threshold_y = threshold_yellow;
        threshold_r = threshold_red;
    }

    public void UpdateCircularDisplay(float ratio)
    {
        float percentage = ratio * 100;
        uiText.text = percentage.ToString("0.0") + "%";
        uiFill.fillAmount = ratio;

        // Adjust colors
        if(percentage > threshold_y)
        {
            uiFill.color = Color.green;
        }
        else if (percentage > threshold_r)
        {
            uiFill.color = Color.yellow;
        }
        else
        {
            uiFill.color = Color.red;
        }
    }

    public void ResetCircularDisplay()
    {
        uiText.text = "0%";
        uiFill.fillAmount = 1;
    }
}
