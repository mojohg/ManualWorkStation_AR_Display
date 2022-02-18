using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Timer : MonoBehaviour
{
    public Image uiFill;
    public Text uiText;

    private int remainingDuration;

    private void Start()
    {
    }

    public void StartTimer(int duration)
    {
        ResetTimer();
        remainingDuration = duration;
        StartCoroutine(UpdateTimer(duration));
    }

    private IEnumerator UpdateTimer(int duration)
    {
        while(remainingDuration >= 0)
        {
            uiText.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";
            uiFill.fillAmount = Mathf.InverseLerp(0, duration, remainingDuration);
            remainingDuration--;
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("Timer ended");
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
