using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] panels;
    public int clickCount;
    public FirebaseManager FB;
    public TMP_Text LBText;

    // Start is called before the first frame update
    public void ToggleShowHideFullPanel(GameObject obj)
    {

        // Get all GameObjects with the Tag Panel
        panels = GameObject.FindGameObjectsWithTag("Panel");
        // For each found Panel - Disable
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        // If the target panel is not active, activate it. 
        obj.SetActive(!obj.activeInHierarchy);
    }

    public void ToggleShowHidePanel(GameObject obj)
    {
        // If the target panel is not active, activate it. 
        obj.SetActive(!obj.activeInHierarchy);
    }

    public void LeaderboardText(string text)
    {
        LBText.text = text;
    }
}
