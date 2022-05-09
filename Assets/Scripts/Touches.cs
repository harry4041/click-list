using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Touches : MonoBehaviour
{
    public GameObject MGPanel;
    public GameObject ShopPanel;
    public FirebaseManager FB;
    int touches = 0;

    private void Start()
    {
        touches = 0;
        InvokeRepeating(nameof(SendAndResetTouches), 0f, 5f);
    }
    // Update is called once per frame
    void Update()
    {
        if (MGPanel.active || ShopPanel.active)
        {
            if (Input.touchCount > 0 && FB.canUpdateClicks)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    touches += 1;
                    FB.UpdateClickLocally();
                }

            }
        }

    }

    public void SendAndResetTouches()
    {
            Debug.Log("Sending " + touches + " touches.");
            FB.ClickButton(touches);
            touches = 0; 
    }
}

