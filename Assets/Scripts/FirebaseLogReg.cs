using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class FirebaseLogReg : MonoBehaviour
{

    /*public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public Touches TS;

    [Header("Register")]
    public TMP_Text warningRegisterText;
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;

    [Header("Login")]
    public TMP_InputField usernameField;
    public GameObject loginPanel;
    public GameObject mainGamePanel;
    public TMP_InputField clickField;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;


    public FirebaseManager FB;
    private void Start()
    {
        auth = FB.auth;
        DBreference = FB.DBreference;
    }

    public void RegisterButton2()
    {
        auth = FB.auth;
        DBreference = FB.DBreference;
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void LoginButton()
    {
        auth = FB.auth;
        DBreference = FB.DBreference;
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
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
                var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
                var DBTask0 = DBreference.Child("users").Child(User.UserId).Child("click").SetValueAsync(0);
                var DBTask1 = DBreference.Child("users").Child(User.UserId).Child("clickWeekly").SetValueAsync(0);
                var DBTask2 = DBreference.Child("users").Child(User.UserId).Child("clickMonthly").SetValueAsync(0);
                var DBTask3 = DBreference.Child("users").Child(User.UserId).Child("clickAlltime").SetValueAsync(0);
                var DBTask4 = DBreference.Child("users").Child(User.UserId).Child("lastDaily").SetValueAsync(0);
                var DBTask5 = DBreference.Child("users").Child(User.UserId).Child("lastDailyPB").SetValueAsync(0);
                var DBTask6 = DBreference.Child("users").Child(User.UserId).Child("lastWeekly").SetValueAsync(0);
                var DBTask7 = DBreference.Child("users").Child(User.UserId).Child("lastWeeklyPB").SetValueAsync(0);
                var DBTask8 = DBreference.Child("users").Child(User.UserId).Child("lastMonthly").SetValueAsync(0);
                var DBTask9 = DBreference.Child("users").Child(User.UserId).Child("lastMonthlyPB").SetValueAsync(0);
                var DBTask10 = DBreference.Child("users").Child(User.UserId).Child("dailyMidnight").SetValueAsync("dailyMidnight");
                var DBTask11 = DBreference.Child("users").Child(User.UserId).Child("weeklyMidnight").SetValueAsync("weeklyMidnight");
                var DBTask12 = DBreference.Child("users").Child(User.UserId).Child("monthlyMidnight").SetValueAsync("monthlyMidnight");

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


            usernameField.text = User.DisplayName;

            //Hide the login panel and open the game panel
            mainGamePanel.SetActive(!mainGamePanel.activeInHierarchy);
            yield return new WaitForSeconds(1.5f);
            loginPanel.SetActive(!loginPanel.activeInHierarchy);

            //UIManager.instance.UserDataScreen(); // Change to user data UI
            warningRegisterText.text = "";

            TS.loggedIn = true;
            FB.LoginButton();
        }
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
    */
}
