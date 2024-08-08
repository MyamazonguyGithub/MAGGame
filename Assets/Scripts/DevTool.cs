using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTool : MonoBehaviour
{
    public static DevTool Instance;

    [SerializeField] GameObject mainCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            mainCanvas.SetActive(!mainCanvas.activeSelf);
        }
    }

    public void UserIDLog()
    {
        Debug.Log("User ID: " + PhotonNetwork.LocalPlayer.UserId);
    }

    public void ListRooms()
    {
        //Debug.Log($"Rooms: {PhotonNetwork.room}" );
    }
}
