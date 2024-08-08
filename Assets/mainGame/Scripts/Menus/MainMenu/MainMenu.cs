using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    // UI
    public GameObject playerEloGO;
    public GameObject battleButtonGO;
    public GameObject cardsButtonGO;
    public GameObject playerExpGO;
    public GameObject playerLevelSlider;
    public GameObject notifyCards;
    public GameObject settings;
    public GameObject deleteConfirmation;
    public GameObject aboutPopup;
    public GameObject buttonDelete;
    public GameObject buttonAbout;
    public GameObject buttonCredits;
    public GameObject buttonCloseConfirmation;
    public GameObject buttonCloseAbout;
    public GameObject battleEnergyIcon;
    public GameObject battleSwordIcon;
    Image lockIcon;

    // stats
    public TextMeshProUGUI attack;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI speed;

    // notifications
    public TextMeshProUGUI notifyCardsTxt;

    // daily gift
    public GameObject dailyGiftCanvas;
    DailyGift dailyGift;
    GameObject dailyGiftsNotification;

    // ranking 
    public GameObject rankingCanvas;

    // profile
    public GameObject profileCanvas;

    public Fighter fighterInstance;

    void Awake()
    {

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        AudioSource audioSource = audioManager.GetComponent<AudioSource>();
        audioManager.Stop("V_Combat_Theme");
        //If Main Menu theme is already playing keep playing it
        if (!audioSource.isPlaying) audioSource.Play();

        dailyGift = dailyGiftCanvas.GetComponent<DailyGift>();
        dailyGiftsNotification = GameObject.Find("DailyGiftsNotification");
        lockIcon = GameObject.Find("Battle_Lock").GetComponent<Image>();

        Fighter player = PlayerUtils.FindInactiveFighter();
        PlayerUtils.FindInactiveFighterGameObject().SetActive(false);
        MenuUtils.ShowElo(playerEloGO);
        MenuUtils.SetLevelSlider(playerExpGO, playerLevelSlider, player.level, player.experiencePoints);
        MenuUtils.DisplayLevelIcon(player.level, GameObject.Find("Levels"));
        MenuUtils.SetFighterStats(attack, hp, speed);
        cardsButtonGO.GetComponent<Button>().interactable = player.skills.Count > 0;
        notifyCardsTxt = notifyCards.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        SetBattleButton();

        // Hide poppups
        settings.SetActive(false);
        deleteConfirmation.SetActive(false);
        aboutPopup.SetActive(false);
        dailyGiftCanvas.SetActive(false);
        rankingCanvas.SetActive(false);
        profileCanvas.SetActive(false);
        dailyGiftsNotification.SetActive(false);
        if (dailyGift.IsFirstTime())
            dailyGiftsNotification.SetActive(true);

        buttonDelete.GetComponent<Button>().onClick.AddListener(() => ShowDeleteConfirmation());
        buttonCloseConfirmation.GetComponent<Button>().onClick.AddListener(() => CloseSettingsConfirmation());
        buttonAbout.GetComponent<Button>().onClick.AddListener(() => ShowAboutPopup());
        buttonCloseAbout.GetComponent<Button>().onClick.AddListener(() => HideAboutPopup());
        buttonCredits.GetComponent<Button>().onClick.AddListener(() => IShowCredits());


       
    }

    IEnumerator Start()
    {
        if (SceneFlag.sceneName == SceneNames.EntryPoint.ToString() ||
            SceneFlag.sceneName == SceneNames.Combat.ToString() ||
            SceneFlag.sceneName == SceneNames.LevelUp.ToString() ||
                SceneFlag.sceneName == SceneNames.Credits.ToString())
        {
            StartCoroutine(SceneManagerScript.instance.FadeIn());
            yield return new WaitForSeconds(GeneralUtils.GetRealOrSimulationTime(SceneFlag.FADE_DURATION));
        }

        battleButtonGO.GetComponent<Button>().enabled = User.Instance.energy > 0;

        StartCoroutine(RefreshItems());

        SceneFlag.sceneName = SceneNames.MainMenu.ToString();
    }

    IEnumerator RefreshItems()
    {
        do
        {
            if (dailyGift.IsGiftAvailable() || dailyGift.IsFirstTime())
                dailyGiftsNotification.SetActive(true);

            // Notifications
            if (Notifications.isInventoryNotificationsOn)
            {
                notifyCards.SetActive(true);
                notifyCardsTxt.text = Notifications.cardsUnseen.ToString();
            }
            else
                notifyCards.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        while (true);
    }

    #region UI

    public void UpdateStatsUI()
    {
        fighterInstance.IncreaseDamagePerClick(50);
    }

    public void SetBattleButton()
    {
        bool userHasEnergy = User.Instance.energy > 0;
        battleButtonGO.GetComponent<Button>().enabled = userHasEnergy;
        lockIcon.enabled = !userHasEnergy;
        battleEnergyIcon.SetActive(!userHasEnergy);
        battleSwordIcon.SetActive(userHasEnergy);
    }

    public void JoinTournament()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " has joined the lobby.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom(null, 8);
    }

    void CreateRoom(int maxPlayers)
    {
        int roomNum = Random.Range(1, 1000);
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, PublishUserId = true, MaxPlayers = maxPlayers};
        PhotonNetwork.CreateRoom("Room" + roomNum, roomOptions);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " failed to join a room.");
        CreateRoom(8); //Creating room for bracket cup
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Usually, failing to create a room means there is already a room with the same name.
        Debug.Log("Failed to create a room: " + message);
        CreateRoom(8); //Creating room for bracket cup
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " has joined the room " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Cup");
    }

    // on settings button
    public void OpenSettings()
    {
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
    }

    public void ShowDeleteConfirmation()
    {
        deleteConfirmation.SetActive(true);
    }

    public void ShowAboutPopup()
    {
        aboutPopup.SetActive(true);
    }

    public void HideAboutPopup()
    {
        aboutPopup.SetActive(false);
    }

    public void IShowCredits()
    {
        StartCoroutine(ShowCredits());
    }
    #endregion



    public IEnumerator ShowCredits()
    {
        StartCoroutine(SceneManagerScript.instance.FadeOut());
        yield return new WaitForSeconds(GeneralUtils.GetRealOrSimulationTime(SceneFlag.FADE_DURATION));
        SceneManager.LoadScene(SceneNames.Credits.ToString());
    }

    public void CloseSettingsConfirmation()
    {
        deleteConfirmation.SetActive(false);
    }

    public void EnableDailyGiftNotification()
    {
        dailyGiftsNotification.SetActive(true);
        dailyGiftsNotification.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
    }

    public void DisableDailyGiftNotification()
    {
        dailyGiftsNotification.SetActive(false);
    }
}
