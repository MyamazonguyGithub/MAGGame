using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class FirebaseAuthenticator : MonoBehaviour
{
    private FirebaseAuth auth;

    async void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        await SignInAnonymously();
    }

    async Task SignInAnonymously()
    {
        try
        {
            AuthResult authResult = await auth.SignInAnonymouslyAsync();
            FirebaseUser newUser = authResult.User;
            Debug.Log("User signed in successfully: " + newUser.UserId);
            // Now you can count and log users
            GetComponent<CountUsers>().CountAndLogRegisteredUsers();
        }
        catch (Exception ex)
        {
            Debug.LogError("Sign-in failed: " + ex.Message);
        }
    }
}
