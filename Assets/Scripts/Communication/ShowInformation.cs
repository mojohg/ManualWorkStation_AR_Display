using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInformation : MonoBehaviour
{

    public void ShowItem(GameObject item)
    {

    }

    public void ResetInformation()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child);
        }
    }
}
