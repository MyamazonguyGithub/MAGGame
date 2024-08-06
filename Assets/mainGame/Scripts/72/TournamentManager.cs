using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class TournamentManager : MonoBehaviourPunCallbacks
{
    public static TournamentManager Instance;

    public int TotalParticipants;
    public int CurrentRound = 1;
    private List<Player> participants = new List<Player>();
    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                FetchTotalParticipants();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    private void FetchTotalParticipants()
    {
        databaseReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                TotalParticipants = (int)snapshot.ChildrenCount;
                Debug.Log($"Total Participants: {TotalParticipants}");

                // You can start the tournament or create rooms based on TotalParticipants here
            }
            else
            {
                Debug.LogError("Failed to fetch total participants from Firebase.");
            }
        });
    }

    public void StartTournament(List<Player> registeredParticipants)
    {
        participants = registeredParticipants;
        CreateRoomForRound(CurrentRound);
    }

    private void CreateRoomForRound(int round)
    {
        string roomName = $"Round{round}_{System.Guid.NewGuid().ToString()}";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // Assuming 1v1 matches

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartMatch();
        }
    }

    private void StartMatch()
    {
        // Logic to start the match (e.g., load the match scene)
        PhotonNetwork.LoadLevel("MatchScene");
    }

    public void OnMatchResult(Player winner, Player loser)
    {
        participants.Remove(loser);

        if (participants.Count == 1)
        {
            EndTournament(winner);
        }
        else
        {
            CurrentRound++;
            CreateRoomForRound(CurrentRound);
        }
    }

    private void EndTournament(Player winner)
    {
        Debug.Log($"Tournament Winner: {winner.NickName}");
        // Logic to handle the end of the tournament
    }
}
