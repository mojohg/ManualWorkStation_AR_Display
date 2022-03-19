using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Timer : MonoBehaviour
{
    public Image uiFill;
    public Text uiText;

    private int remaining_duration;  // in milliseconds
    private int max_duration;

    private void Start()
    {
    }

    public void StartTimer(int duration_sec)
    {
        /*remainingDuration = duration_sec * 1000;
        Debug.Log("Set timer to : " + remainingDuration.ToString() + " milliseconds");
        StartCoroutine(UpdateTimer(remainingDuration));*/
        max_duration = duration_sec;
        remaining_duration = duration_sec;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()  // int duration
    {
        while (remaining_duration >= 0)
        {
            /*uiText.text = $"{remainingDuration / 1000:00}:{remainingDuration % 1000:00}";
            uiFill.fillAmount = Mathf.InverseLerp(0, duration, remainingDuration);
            remainingDuration = remainingDuration - 10;
            yield return new WaitForSeconds(0.010f);*/

            uiText.text = $"{remaining_duration / 60:00}:{remaining_duration % 60:00}";
            uiFill.fillAmount = Mathf.InverseLerp(0, max_duration, remaining_duration);
            remaining_duration = remaining_duration - 1;
            yield return new WaitForSeconds(1f);
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
