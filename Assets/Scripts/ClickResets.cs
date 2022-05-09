using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;


//click
//clickWeekly
//clickMonthly
//clickAlltime

public class ClickResets : MonoBehaviour
{
	///THIS WHOLE THING NEEDS TO BE HANDLED BY A CLOUD FUNCTION
	///THIS WHOLE THING IS NOW HANDLED BY A CLOUD FUNCTION AND WILL NOT RUN
	
	int dayOfWeekInt;

	int lastDayLogged;
	int lastMonthLogged;
	int lastWeekDayLogged;

	public FirebaseManager FB;
	public GameManager GM;

	void Start()
	{

		//Last times logged in
		lastDayLogged = 1;  //GET FROM FB - if no value, then set value. Also dont login until got
		lastMonthLogged = 1;  //GET FROM FB - if no value, then set value. Also dont login until got
		lastWeekDayLogged = 1;  //GET FROM FB - if no value, then set value. Also dont login until got
		dayOfWeekInt = (int)GM.currentTime.DayOfWeek;
	}

    public void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("########");
			Debug.Log(SecondsLeftOfDay());
			Debug.Log(SecondsLeftOfWeek());
			Debug.Log(SecondsLeftOfMonth());
			Debug.Log("########");

		}
	}

	//Put into function
	public void thisFunc()
	{
		if (lastDayLogged != GM.currentTime.Day)
		{
			//FB reset Daily clicks
			FB.dailyClicks.text = 0.ToString();
		}
		if (lastWeekDayLogged > dayOfWeekInt) //If the day of the week is less than the day they were last on, it must be a new week...right?
		{
			//FB reset Weekly clicks
			FB.weeklyClicks.text = 0.ToString();
		}
		if (lastMonthLogged != GM.currentTime.Month)
		{
			//FB reset Monthly clicks
			FB.monthlyClicks.text = 0.ToString();
		}


		StartCoroutine(FB.SetFBValue("lastDayLogged", GM.currentTime.Day));
		StartCoroutine(FB.SetFBValue("lastMonthLogged", GM.currentTime.Month));
		StartCoroutine(FB.SetFBValue("lastWeekDayLogged", (int)GM.currentTime.DayOfWeek));


		Invoke("thisFunc", SecondsLeftOfDay());
		Invoke("thisFunc", SecondsLeftOfWeek());
		Invoke("thisFunc", SecondsLeftOfMonth());
		

	}

	public int SecondsLeftOfDay()
	{
		int hoursToSeconds = ((24 - (GM.currentTime.Hour + 1)) * 60) * 60; //+1 because it's within the hour
		int minuteToSeconds = (60 - GM.currentTime.Minute) * 60;
		return hoursToSeconds + minuteToSeconds;
	}

	public int SecondsLeftOfWeek()
	{
		int daySecondsLeft = SecondsLeftOfDay();
		int daysOfWeekLeft = 7 - dayOfWeekInt; //Tuesday is 2 - so 7 - 2 = 5 full days left
		int secondsOfWeekLeft = daySecondsLeft + (daysOfWeekLeft * 24 * 60 * 60);
		return secondsOfWeekLeft;
	}

	public int SecondsLeftOfMonth()
	{
		int daySecondsLeft = SecondsLeftOfDay();
		if (GM.currentTime.Month == 1 || GM.currentTime.Month == 3 || GM.currentTime.Month == 5 || GM.currentTime.Month == 7 || GM.currentTime.Month == 8 || GM.currentTime.Month == 10 || GM.currentTime.Month == 12)
		{
			//31 days
			int fullDaysLeft = 31 - (GM.currentTime.Day - 1); //-1 because you dont include the current day
			int secondsOfMonthLeft = (fullDaysLeft * 24 * 60 * 60) + daySecondsLeft; //days left x 24 hours x 60 minutes x 60 seconds + daySecondsLeft
			return secondsOfMonthLeft;
		}
		else if (GM.currentTime.Month == 4 || GM.currentTime.Month == 6 || GM.currentTime.Month == 9 || GM.currentTime.Month == 11)
		{
			int fullDaysLeft = 30 - (GM.currentTime.Day - 1); //-1 because you dont include the current day
			int secondsOfMonthLeft = (fullDaysLeft * 24 * 60 * 60) + daySecondsLeft; //days left x 24 hours x 60 minutes x 60 seconds + daySecondsLeft
			return secondsOfMonthLeft;
		}
		else
		{
			if (DateTime.IsLeapYear(GM.currentTime.Year))
			{
				int fullDaysLeft = 29 - (GM.currentTime.Day - 1); //-1 because you dont include the current day
				int secondsOfMonthLeft = (fullDaysLeft * 24 * 60 * 60) + daySecondsLeft; //days left x 24 hours x 60 minutes x 60 seconds + daySecondsLeft
				return secondsOfMonthLeft;
			}
			else
			{
				int fullDaysLeft = 28 - (GM.currentTime.Day - 1); //-1 because you dont include the current day
				int secondsOfMonthLeft = (fullDaysLeft * 24 * 60 * 60) + daySecondsLeft; //days left x 24 hours x 60 minutes x 60 seconds + daySecondsLeft
				return secondsOfMonthLeft;
			}
		}


	}
}
