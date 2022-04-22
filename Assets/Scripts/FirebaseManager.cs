using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;

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
    public TMP_Text lifetimeClicks;
    public GameObject scoreElement;
    public Transform scoreboardContent;


    public GameObject loginPanel;
    public GameObject mainGamePanel;
    public int FBClicks;
    public int testClick;
    public int clicksToPush;

    public Coroutine leaderboardCoroutine;


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
                InvokeRepeating(nameof(IncreaseClicks), 5.0f, 5.0f);
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
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        
        //Start the leaderboard updating
        StartCoroutine(KeepScoreboardUpdating("click", scoreboardContent));

    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    //Function for the save button
    //TODO - Make this happen every x amount of seconds instead of on every click
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
    }

    public void ScoreboardButton(string clickType)
    {
        //StopCoroutine(leaderboardCoroutine);
        leaderboardCoroutine = StartCoroutine(LoadScoreboardData(clickType, scoreboardContent));

    }
    
    public void ClickButton(int clicksToAdd)
    {
        StartCoroutine(IncreaseClicks(clicksToAdd, "click"));
    }


    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningRegisterText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningRegisterText.text = "";
            warningRegisterText.text = "Logged In";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(1f);

            usernameField.text = User.DisplayName;
            //Hide the login panel and open the game panel
            loginPanel.SetActive(!loginPanel.activeInHierarchy);
            mainGamePanel.SetActive(!mainGamePanel.activeInHierarchy);
            //UIManager.instance.UserDataScreen(); // Change to user data UI
            warningRegisterText.text = "";

            //Add 0 clicks to update local variables
            StartCoroutine(IncreaseClicks(0, "click"));

        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;
                var DBTask = DBreference.Child("users").Child(User.UserId).Child("click").SetValueAsync(0);
                var DBTask1 = DBreference.Child("users").Child(User.UserId).Child("weeklyClick").SetValueAsync(0);
                var DBTask2 = DBreference.Child("users").Child(User.UserId).Child("monthlyClick").SetValueAsync(0);
                var DBTask3 = DBreference.Child("users").Child(User.UserId).Child("allTimeClick").SetValueAsync(0);

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        //UIManager.instance.LoginScreen();
                        warningRegisterText.text = "Registered!";

                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }


    //TODO: Find a way of doing this not on every click
    private IEnumerator IncreaseClicks(int clicksToAdd, string timeClick)
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


                //Add clicks done and update UI - 
                //THIS NEEDS CHANGING TO A CHEAPER WAY OF ADDING CLICKS
                click += clicksToAdd;
                StartCoroutine(UpdateClick(click, timeClick));
                lifetimeClicks.text = click.ToString();
                FBClicks = int.Parse(snapshot.Child(timeClick).Value.ToString());
                Debug.Log("FB " + click);
            }

            //This is to update on startup
            if (clicksToAdd == 0)
            {
                lifetimeClicks.text = FBClicks.ToString();
                Debug.Log("Start " + FBClicks);
            }

        
        
    }

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
            //Clicks is now updated
        }
    }

    //Update clicks locally so user personal numbers update on every click (even though its not being pushed to FB)
    public void UpdateClickLocally(int clicksToAdd)
    {
        testClick = clicksToAdd + FBClicks;
        if (testClick > FBClicks)
        {
            testClick = FBClicks;
        }
        //It's easier to just add 1 here rather than messing around with FB stuff
        lifetimeClicks.text = (testClick + 1).ToString();
        
        
        
        Debug.Log("Local " + testClick);

    }

    private IEnumerator LoadUserData()
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

            clickField.text = snapshot.Child("click").Value.ToString();
        }
    }

    private IEnumerator LoadScoreboardData(string clickSpan, Transform scoreCont)
    {
        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild(clickSpan).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreCont.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int click = int.Parse(childSnapshot.Child(clickSpan).Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreCont);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, click);
            }
        }
        
    }

    //Update the scoreboard every 1.5 seconds rather than constantly
    private IEnumerator KeepScoreboardUpdating(string clickType, Transform scoreboardCont)
    {
        StartCoroutine(LoadScoreboardData(clickType, scoreboardCont));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(KeepScoreboardUpdating(clickType, scoreboardCont));
    }
}