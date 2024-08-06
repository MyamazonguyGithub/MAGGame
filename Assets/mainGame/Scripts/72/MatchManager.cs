using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private void Start()
    {
        // Initialize match (e.g., spawn players, start timer)
    }

    public void EndMatch(Player winner, Player loser)
    {
        PhotonNetwork.LeaveRoom();
        TournamentManager.Instance.OnMatchResult(winner, loser);
    }
}
