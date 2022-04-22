using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text clickText;

    public void NewScoreElement(string _username, int _click)
    {
        usernameText.text = _username;
        clickText.text = _click.ToString();
    }

}