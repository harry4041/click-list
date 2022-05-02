using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Touches : MonoBehaviour
{
    public GameObject MGPanel;
    public GameObject ShopPanel;
    public FirebaseManager FB;

    // Update is called once per frame
    void Update()
    {
        if (MGPanel.active || ShopPanel.active)
        {
            if (Input.touchCount > 0)
            {
                int touches = 0;
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    touches += 1;
                    FB.ClickButton(1);
                    FB.UpdateClickLocally(touches);
                }

            }
        }

    }
}

