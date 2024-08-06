using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public static class FirebaseManagerx
{
    private static DatabaseReference databaseReference;

    static FirebaseManagerx()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    public static async Task<string> GetRandomUserName()
    {
        DataSnapshot snapshot = await databaseReference.Child("users").GetValueAsync();
        if (snapshot.Exists)
        {
            var users = snapshot.Children;
            int randomIndex = UnityEngine.Random.Range(0, (int)snapshot.ChildrenCount);
            int currentIndex = 0;

            foreach (var user in users)
            {
                if (currentIndex == randomIndex)
                {
                    return user.Child("username").Value.ToString();
                }
                currentIndex++;
            }
        }
        Debug.LogError("No users found in Firebase.");
        return "DefaultUser";
    }
}
