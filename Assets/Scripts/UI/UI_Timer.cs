using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Timer : MonoBehaviour
{
    public Image uiFill;
    public Text uiText;

    private int remainingDuration;  // in milliseconds

    private void Start()
    {
    }

    public void StartTimer(int duration_sec)
    {
        ResetTimer();
        remainingDuration = duration_sec * 1000;
        Debug.Log("Set timer: " + remainingDuration.ToString());
        StartCoroutine(UpdateTimer(remainingDuration));
    }

    private IEnumerator UpdateTimer(int duration)
    {
        while (remainingDuration >= 0)
        {
            Debug.Log("Count: " + remainingDuration.ToString());
            //uiText.text = $"{remaining_sec / 60:00}:{remaining_sec % 60:00}";
            uiText.text = $"{remainingDuration / 1000:00}:{remainingDuration % 1000:00}";
            uiFill.fillAmount = Mathf.InverseLerp(0, duration, remainingDuration);
            remainingDuration = remainingDuration - 10;
            yield return new WaitForSeconds(0.010f);
        }
        OnEnd();
    }

    private void OnEnd()
    {
        this.GetComponent<Image>().color = Color.red;
    }

    public void ResetTimer()
    {
        this.GetComponent<Image>().color = new Color(255,182,0,255);
    }
}
