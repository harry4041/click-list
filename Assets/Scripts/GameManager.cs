using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject weeklyLeaderboard;
    public GameObject monthlyLeaderboard;
    public GameObject allTimeLeaderboard;

    private void Start()
    {
        weeklyLeaderboard.SetActive(false);
        monthlyLeaderboard.SetActive(false);
        allTimeLeaderboard.SetActive(false);
    }
}
