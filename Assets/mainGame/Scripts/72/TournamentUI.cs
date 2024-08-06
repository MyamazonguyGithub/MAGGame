using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentUI : MonoBehaviour
{
    public Button registerButton;
    public Button startTournamentButton;

    private List<Player> registeredPlayers = new List<Player>();

    private void Start()
    {
        registerButton.onClick.AddListener(RegisterPlayer);
        startTournamentButton.onClick.AddListener(StartTournament);
    }

    private void RegisterPlayer()
    {
        if (!registeredPlayers.Contains(PhotonNetwork.LocalPlayer))
        {
            registeredPlayers.Add(PhotonNetwork.LocalPlayer);
        }
    }

    private void StartTournament()
    {
        if (registeredPlayers.Count >= 2)
        {
            TournamentManager.Instance.StartTournament(registeredPlayers);
        }
        else
        {
            Debug.Log("Not enough players to start the tournament.");
        }
    }
}
