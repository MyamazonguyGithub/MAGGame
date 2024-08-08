using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Realtime;

namespace Photon.Pun.Demo.Asteroids
{
    public class FirebaseManager : MonoBehaviourPunCallbacks
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
        public TMP_Text warningLoginText;
        public TMP_Text confirmLoginText;

        //Register variables
        [Header("Register")]
        public InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;

        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        public Text IGN;


        public static FirebaseManager Instance;


        void Awake()
        {   // Singleton pattern implementation
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

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


            roomListEntries = new Dictionary<string, GameObject>();


        }

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth");
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }
        public void ClearLoginFeilds()
        {
            emailLoginField.text = "";
            passwordLoginField.text = "";
        }
        public void ClearRegisterFeilds()
        {
            usernameRegisterField.text = "";
            emailRegisterField.text = "";
            passwordRegisterField.text = "";
            passwordRegisterVerifyField.text = "";
        }

        //Function for the login button
        public void LoginButton()
        {
            //Call the login coroutine passing the email and password
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }

        public void GuestLogin()
        {
            if(emailLoginField.text == "")
            {
                warningLoginText.text = "Please insert your name.";
            }
            else
            {
                //User = LoginTask.Result.User;
                //Debug.Log("pangalan: " + LoginTask.Result.User);
                //Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";

                //yield return new WaitForSeconds(2);

                GameData.IGN = emailLoginField.text;
                IGN.text = emailLoginField.text;
                PhotonNetwork.ConnectUsingSettings();
            }
            
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogError("Disconnected from the network: " + cause.ToString());
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.LocalPlayer.NickName = GameData.IGN;
            IGoToScene("EntryPoint");
            //Debug.Log("Name: " + User.DisplayName);
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
        //Function for the register button
        public void RegisterButton()
        {
            //Call the register coroutine passing the email, password, and username
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }
        //Function for the sign out button
        public void SignOutButton()
        {
            auth.SignOut();
            UIManager.instance.LoginScreen();
            ClearRegisterFeilds();
            ClearLoginFeilds();
        }
   
        private IEnumerator Login(string _email, string _password)
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
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
                warningLoginText.text = message;
            }
            else
            {
                //User is now logged in
                //Now get the result
                User = LoginTask.Result.User;
                Debug.Log("pangalan: " + LoginTask.Result.User);
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";

                yield return new WaitForSeconds(2);

                GameData.IGN = User.DisplayName;
                IGN.text = User.DisplayName;
                IGoToScene("EntryPoint");
                Debug.Log("Name: " + User.DisplayName);
                confirmLoginText.text = "";
                ClearLoginFeilds();
                ClearRegisterFeilds();
            }



        }

        private IEnumerator Register(string _email, string _password, string _username)
        {
            if (_username == "")
            {
                //If the username field is blank show a warning
                warningRegisterText.text = "Missing Username";
            }
            else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
            {
                //If the password does not match show a warning
                warningRegisterText.text = "Password Does Not Match!";
            }
            else
            {
                //Call the Firebase auth signin function passing the email and password
                Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
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
                    User = RegisterTask.Result.User;

                    if (User != null)
                    {
                        // Create a user profile and set the username
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        // Call the Firebase auth update user profile function passing the profile with the username
                        Task profileTask = User.UpdateUserProfileAsync(profile);

                        // Wait until the task completes
                        yield return new WaitUntil(() => profileTask.IsCompleted);

                        if (profileTask.Exception != null)
                        {
                            // If there are errors setting the username, handle them
                            Debug.LogWarning($"Failed to set username with {profileTask.Exception}");
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            // Registration and username setting successful
                            Debug.Log("Registration Successful. Welcome " + User.DisplayName);

                            // Update UI with registered username using MenuUtils
                            //GameObject playerNameGO = GameObject.Find("YourPlayerNameGameObject"); // Replace with actual GameObject name
                            //if (playerNameGO != null)
                            //{
                            //    MenuUtils.SetName(playerNameGO, _username);
                            //}
                            //else
                            //{
                            //    Debug.LogError("PlayerName GameObject not found!");
                            //}

                            // Optionally, update additional user data in the database if needed
                            yield return StartCoroutine(UpdateUsernameDatabase(_username));

                            // Clear warnings and input fields
                            warningRegisterText.text = "";
                            ClearRegisterFeilds();
                            ClearLoginFeilds();

                            // Proceed to desired scene after registration
                            UIManager.instance.LoginScreen();
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
            Task ProfileTask = User.UpdateUserProfileAsync(profile);
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
            Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

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

        private IEnumerator UpdateXp(int _xp)
        {
            //Set the currently logged in user xp
            Task DBTask = DBreference.Child("users").Child(User.UserId).Child("xp").SetValueAsync(_xp);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }

        private IEnumerator UpdateKills(int _kills)
        {
            //Set the currently logged in user kills
            Task DBTask = DBreference.Child("users").Child(User.UserId).Child("kills").SetValueAsync(_kills);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Kills are now updated
            }
        }

        private IEnumerator UpdateDeaths(int _deaths)
        {
            //Set the currently logged in user deaths
            Task DBTask = DBreference.Child("users").Child(User.UserId).Child("deaths").SetValueAsync(_deaths);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Deaths are now updated
            }
        }

        public void IGoToScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

    }
}