using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class CountUsers : MonoBehaviour
{
    private DatabaseReference databaseReference;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async void CountAndLogRegisteredUsers()
    {
        try
        {
            DataSnapshot snapshot = await databaseReference.Child("users").GetValueAsync();
            int userCount = (int)snapshot.ChildrenCount;
            Debug.Log("Total registered users: " + userCount);

            List<string> emailAddresses = new List<string>();
            foreach (DataSnapshot userSnapshot in snapshot.Children)
            {
                if (userSnapshot.HasChild("email") && userSnapshot.HasChild("username"))
                {
                    string email = userSnapshot.Child("email").Value.ToString();
                    string username = userSnapshot.Child("username").Value.ToString();
                    emailAddresses.Add(email);
                    Debug.Log("User email: " + email + ", username: " + username);
                }
            }

            // Optionally, log all email addresses together
            Debug.Log("All registered user emails: " + string.Join(", ", emailAddresses));
        }
        catch (Exception ex)
        {
            Debug.LogError("Error fetching data: " + ex.Message);
        }
    }
}
