using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Collections.Generic;

//click
//clickWeekly
//clickMonthly
//clickAlltime
public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    //public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField clickField;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    public GameObject pbElement;
    public Transform pbContent;

    [Header("LocalData")]
    public TMP_Text lifetimeClicks;
    public TMP_Text dailyClicks;
    public TMP_Text weeklyClicks;
    public TMP_Text monthlyClicks;
    public TMP_Text dailyPB;
    public TMP_Text weeklyPB;
    public TMP_Text monthlyPB;
    public int lifetimeInt;
    public int dailyInt;
    public int weeklyInt;
    public int monthlyInt;
    public bool canUpdateClicks = false;

    [Header("PersonalStatsPage")]
    public TMP_Text yesterdaysClicks;
    public TMP_Text lastWeeksClicks;
    public TMP_Text lastMonthsClicks;
    public TMP_Text dailyPB2;
    public TMP_Text weeklyPB2;
    public TMP_Text monthlyPB2;


    public bool midnight = false;
    public bool midnightWeekly = false;
    public bool midnightMonthly = false;

    [Header("Everything else")]
    public GameObject loginPanel;
    public GameObject mainGamePanel;

    public int FBClicks;
    public int testClick;
    public int clicksToPush;

    public Coroutine lb;
    public Coroutine lbUpdate;
    public Coroutine leaderboardCoroutine;

    public GameManager GM;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);


            }
        });


    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //The Login button is the first thing the player presses, therefore it's used to load all data
    public void LoginButton()
    {
        //Start the leaderboard updating
        lb = StartCoroutine(KeepScoreboardUpdating("click", "lastDaily", scoreboardContent, pbContent));

        //Update the user numbers then do it all locally
        StartCoroutine(UpdateLocalClicksOnStart());
    }

    public void ScoreboardButton(string clickType)
    {
        string pbType;
        if (lb != null)
        {
            StopCoroutine(lb);
            StopCoroutine(lbUpdate);
        }
        
        if(clickType == "click")
        {
            pbType = "lastDaily";
            lb = StartCoroutine(KeepScoreboardUpdating(clickType, pbType, scoreboardContent, pbContent));
        } 
        else if(clickType == "clickWeekly")
        {
            pbType = "lastWeekly";
            lb = StartCoroutine(KeepScoreboardUpdating(clickType, pbType, scoreboardContent, pbContent));

        }
        else if (clickType == "clickMonthly")
        {
            pbType = "lastMonthly";
            lb = StartCoroutine(KeepScoreboardUpdating(clickType, pbType, scoreboardContent, pbContent));

        }
        else if (clickType == "clickAlltime")
        {
            pbType = "clickAlltime";
            lb = StartCoroutine(KeepScoreboardUpdating(clickType, pbType, scoreboardContent, pbContent));

        }
        
    }

    public void ClickButton(int clicksToAdd)
    {
        StartCoroutine(IncreaseClicks("click", clicksToAdd, dailyClicks));
        StartCoroutine(IncreaseClicks("clickWeekly", clicksToAdd, weeklyClicks));
        StartCoroutine(IncreaseClicks("clickMonthly", clicksToAdd, monthlyClicks));
        StartCoroutine(IncreaseClicks("clickAlltime", clicksToAdd));
    }

    //TODO: Find a way of doing this not on every click to make it cheaper
    private IEnumerator IncreaseClicks(string timeClick, int clicksToAdd, TMP_Text localComparision = null)
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            clickField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Set 'click' to FB variable of user click
            int click = int.Parse(snapshot.Child(timeClick).Value.ToString());

            //Add clicks and add to FB
            click += clicksToAdd;

            StartCoroutine(UpdateClick(click, timeClick));

            if (localComparision != null)
            {
                localComparision.text = click.ToString();
            }


        }
    }


    //Push clicks to FB
    private IEnumerator UpdateClick(int _click, string child)
    {
        //Set the currently logged in user clicks
        var DBTask = DBreference.Child("users").Child(User.UserId).Child(child).SetValueAsync(_click);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
        }
    }

    //Update clicks locally so user personal numbers update on every click (even though its not being pushed to FB)
    public IEnumerator UpdateLocalClicksOnStart()
    {
        yield return new WaitForSeconds(2);
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //THIS FEELS EXPENSIVE!!!
            //Get pretty much every value from FB
            int daily = int.Parse(snapshot.Child("click").Value.ToString());
            int weekly = int.Parse(snapshot.Child("clickWeekly").Value.ToString());
            int monthly = int.Parse(snapshot.Child("clickMonthly").Value.ToString());
            int allTime = int.Parse(snapshot.Child("clickAlltime").Value.ToString());
            int lastDaily = int.Parse(snapshot.Child("lastDaily").Value.ToString());
            int lastWeekly = int.Parse(snapshot.Child("lastWeekly").Value.ToString());
            int lastMonthly = int.Parse(snapshot.Child("lastMonthly").Value.ToString());
            int lastDailyPB = int.Parse(snapshot.Child("lastDailyPB").Value.ToString());
            int lastWeeklyPB = int.Parse(snapshot.Child("lastWeeklyPB").Value.ToString());
            int lastMonthlyPB = int.Parse(snapshot.Child("lastMonthlyPB").Value.ToString());

            //Populate leaderboards on both main page and personal stats
            dailyClicks.text = daily.ToString();
            weeklyClicks.text = weekly.ToString();
            monthlyClicks.text = monthly.ToString();
            lifetimeClicks.text = allTime.ToString();
            yesterdaysClicks.text = lastDaily.ToString();
            lastWeeksClicks.text = lastWeekly.ToString();
            lastMonthsClicks.text = lastMonthly.ToString();
            dailyPB2.text = lastDailyPB.ToString();
            weeklyPB2.text = lastWeeklyPB.ToString();
            monthlyPB2.text = lastMonthlyPB.ToString();

            StartCoroutine(PersonalBest("lastDailyPB", dailyPB));
            StartCoroutine(PersonalBest("lastMonthlyPB", monthlyPB));
            StartCoroutine(PersonalBest("lastWeeklyPB", weeklyPB));


            canUpdateClicks = true;
        }
    }

    public void UpdateClickLocally()
    {
        if (canUpdateClicks)
        {
            dailyInt = (int.Parse(dailyClicks.text));
            weeklyInt = (int.Parse(weeklyClicks.text));
            monthlyInt = (int.Parse(monthlyClicks.text));
            lifetimeInt = (int.Parse(lifetimeClicks.text));

            dailyClicks.text = (dailyInt + 1).ToString();
            weeklyClicks.text = (weeklyInt + 1).ToString();
            monthlyClicks.text = (monthlyInt + 1).ToString();
            lifetimeClicks.text = (lifetimeInt + 1).ToString();
        }
    }

    public IEnumerator PersonalBest(string pb, TMP_Text pbText)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            clickField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            pbText.text = snapshot.Child(pb).Value.ToString();
        }
    }

    private IEnumerator LoadScoreboardData(string clickSpan, string pbSpan, Transform scoreCont, Transform pBcont)
    {
        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild(clickSpan).GetValueAsync();
        var DBTask2 = DBreference.Child("users").OrderByChild(pbSpan).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            DataSnapshot snapshot2 = DBTask2.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreCont.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in pBcont.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID and shows top 10
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>().Take(10))
            {
                
                    string username = childSnapshot.Child("username").Value.ToString();
                    int click = int.Parse(childSnapshot.Child(clickSpan).Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreCont);
                    scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, click);
            }
            foreach (DataSnapshot childSnapshot in snapshot2.Children.Reverse<DataSnapshot>().Take(1))
            {

                string username = childSnapshot.Child("username").Value.ToString();
                int pBclick = int.Parse(childSnapshot.Child(pbSpan).Value.ToString());

                //Instantiate new scoreboard elements
                GameObject PBElement = Instantiate(scoreElement, pBcont);
                PBElement.GetComponent<ScoreElement>().NewScoreElement(username, pBclick);
            }
        }

    }

    //Update the scoreboard every 1.5 seconds rather than constantly
    private IEnumerator KeepScoreboardUpdating(string clickType, string pBspan, Transform scoreboardCont, Transform pBcont)
    {
        Debug.Log("Scoreboard updating");
        StartCoroutine(LoadScoreboardData(clickType, pBspan, scoreboardCont, pBcont));
        yield return new WaitForSeconds(1.5f);
        lbUpdate = StartCoroutine(KeepScoreboardUpdating(clickType, pBspan, scoreboardCont, pBcont));
    }
}


