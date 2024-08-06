using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class CountOnlineUsers : MonoBehaviour
{
    private DatabaseReference databaseReference;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        ListenForOnlineUsers();
    }

    void ListenForOnlineUsers()
    {
        databaseReference.Child("presence").ValueChanged += (sender, args) =>
        {
            List<string> onlineUsers = new List<string>();

            foreach (var childSnapshot in args.Snapshot.Children)
            {
                if ((bool)childSnapshot.Value)
                {
                    onlineUsers.Add(childSnapshot.Key);
                }
            }

            Debug.Log("Online users: " + string.Join(", ", onlineUsers));
        };
    }
}
