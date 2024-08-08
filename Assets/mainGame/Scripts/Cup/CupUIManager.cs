using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* CUP STRUCTURE IDS
 * 
 * QUARTERS     SEMIS       FINAL       SEMIS       QUARTERS
 * 
 *    1            9         13          11            5
 *    2           10         14          12            6
 * 
 *    3                                                7
 *    4                                                8
 *    
 * ----------------------------------------------------------
 * 
 * BLOCK STRUCTURE IDS
 * 
 * QUARTERS     SEMIS       FINAL       SEMIS       QUARTERS
 * 
 *    A           E           G           F            C
 *                                              
 *    B                                                D
 *                                                    
 *    
 * ----------------------------------------------------------
 */

public class CupUIManager : MonoBehaviour
{
    public static CupUIManager Instance;

    [SerializeField] CupFighterContainer[] fighterContainers;
    // UI
    Transform labelContainer;
    Transform playersContainer;
    TextMeshProUGUI roundAnnouncer;
    Button buttonCollectRewards;

    // prize
    public Canvas prizeCanvas;
    GameObject cupGoldAndGems;
    GameObject cupSkills;
    GameObject cupGoldPopup;
    GameObject cupGemsPopup;

    TextMeshProUGUI goldOrGemsTitle;
    TextMeshProUGUI goldQuantity;
    TextMeshProUGUI gemsQuantity;

    // UI chest rewards
    public GameObject skillTitle;
    public GameObject skillType;
    public GameObject skillRarity;
    public GameObject skillDescription;
    public GameObject skillIcon;
    public GameObject commonSkill;
    public GameObject rareSkill;
    public GameObject epicSkill;
    public GameObject legendarySkill;
    public GameObject battleBtnContainer;
    public Button readyButton;
    public Button skillInventory;
    public Button skillMainMenu;
    public Button allSkills;
    
    // @here For your approval. Here are the sample Box Characters. 
    // data
    static Fighter player;

    // scripts
    CupManager cupManager;

    // vars
    public string round;
    private Color32 playerHihglight = new Color32(254, 161, 0, 255);
    //const int CUP_COOLDOWN_IN_DAYS = 5;

    private const int CUP_COOLDOWN_IN_SECONDS = 10;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        SetUpUI(); //get references
        IsTournamentOver();
        //HideCupLabels();
        //GetAllUIPlayers();
        //SetUIBasedOnRound();
        //SetUpButtons();

        // Initial setup
        /*cupGoldAndGems.SetActive(false);
        cupSkills.SetActive(false);
        cupGoldPopup.SetActive(false);
        cupGemsPopup.SetActive(false);*/
        //player = PlayerUtils.FindInactiveFighter();
    }

    private IEnumerator Start()
    {
        //ShowCupLabel();
        if (SceneFlag.sceneName == SceneNames.Combat.ToString() || SceneFlag.sceneName == SceneNames.LevelUp.ToString())
        {
            StartCoroutine(SceneManagerScript.instance.FadeIn());
            yield return new WaitForSeconds(GeneralUtils.GetRealOrSimulationTime(1f));
        }

        SceneFlag.sceneName = SceneNames.Cup.ToString();
    }

    private void SetUpUI()
    {
        // cup bracket
        labelContainer = GameObject.Find("LabelContainer").GetComponent<Transform>();
        playersContainer = GameObject.Find("Players").GetComponent<Transform>();
        roundAnnouncer = GameObject.Find("RoundAnnouncerTxt").GetComponent<TextMeshProUGUI>();
        buttonCollectRewards = GameObject.Find("Button_Rewards").GetComponent<Button>();
        cupManager = GetComponent<CupManager>();

        // collect reward popup
        cupSkills = GameObject.Find("Popup_Skill");
        cupGoldAndGems = GameObject.Find("Popup_Currencies");
        cupGoldPopup = GameObject.Find("GoldReward");
        cupGemsPopup = GameObject.Find("GemsReward");
        //goldOrGemsTitle = GameObject.Find("Popup_Currencies_Title").GetComponent<TextMeshProUGUI>();
        //goldQuantity = GameObject.Find("Gold_Quantity").GetComponent<TextMeshProUGUI>();
        //gemsQuantity = GameObject.Find("Gems_Quantity").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCupUI(CupFighter[] cupFighters)
    {
        for (int i = 0; i < cupFighters.Length; i++) //The length must be 8
        {
            if (cupFighters[i] == null)
            {
                fighterContainers[i].tmpFighterName.text = "Empty";
            }
            else
            {
                fighterContainers[i].tmpFighterName.text = cupFighters[i].fighterName;
            }
        }
    }

    private void IsTournamentOver()
    {
        /*if (Cup.Instance.round == CupDB.CupRounds.END.ToString() || !Cup.Instance.playerStatus)
        {
            Debug.Log("Tournament ends.");
            //battleBtnContainer.SetActive(false);
            //buttonCollectRewards.gameObject.SetActive(true);
        }
        else
        {
            //battleBtnContainer.SetActive(true);
            //buttonCollectRewards.gameObject.SetActive(false);
            Debug.Log("Tournament continues.");
            readyButton.gameObject.SetActive(true);
        }*/
    }

    private void SetUIBasedOnRound()
    {
        /*switch (Cup.Instance.round)
        {
            case "QUARTERS":
                SetUIQuarters();
                break;
            case "SEMIS":
                SetUISemis();
                break;
            case "FINALS":
                SetUIFinals();
                break;
            case "END":
                SetUIFinalsEnd();
                break;
        }*/
    }

    private void HideCupLabels()
    {
        for (int i = 0; i < labelContainer.childCount; i++)
            labelContainer.GetChild(i).gameObject.SetActive(false);
    }

    private Transform GetCupLabelByName(string name)
    {
        switch (name)
        {
            case "DIVINE":
                return labelContainer.GetChild(0);
        }

        // default
        return labelContainer.GetChild(0);
    }


    private Sprite GetSpeciePortrait(string species)
    {
        return Resources.Load<Sprite>("CharacterProfilePicture/" + species);
    }

    private Dictionary<string, string> GetRewardType(string round)
    {
        return new Dictionary<string, string>
        {
            {
                CupDB.prizes[(CupDB.CupRounds)Enum.Parse(typeof(CupDB.CupRounds), round)]["reward"],
                CupDB.prizes[(CupDB.CupRounds)Enum.Parse(typeof(CupDB.CupRounds), round)]["value"]
            }
        };
    }

    private void GiveReward(Dictionary<string, string> reward)
    {
        FindObjectOfType<AudioManager>().Play("S_Reward_Received");

        prizeCanvas.enabled = true;

        if (reward.ContainsKey("gold"))
            EnableGoldPopup(reward);
        if (reward.ContainsKey("gems"))
            EnableGemsPopup(reward);
        if (reward.ContainsKey("chest"))
            EnableChestPopup(reward);

        ResetCup();
    }

    //private void ResetCup()
    //{
    //     //PlayerPrefs.SetString("cupCountdown", DateTime.Now.AddDays(CUP_COOLDOWN_IN_DAYS).ToBinary().ToString());

    //    PlayerPrefs.SetString("cupCountdown", DateTime.Now.AddSeconds(CUP_COOLDOWN_IN_SECONDS).ToBinary().ToString());

    //    PlayerPrefs.Save();

    //    cupManager.DeleteCupFile();
    //}

    private void ResetCup()
    {
        // Calculate the next Monday 10 AM
        DateTime nextMonday = DateTime.Today.AddDays(((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7);
        DateTime nextMonday10PM = nextMonday.Date.AddHours(22);

        // Store the next scheduled time
        PlayerPrefs.SetString("cupCountdown", nextMonday10PM.ToBinary().ToString());
        PlayerPrefs.Save();

        // Additional logic to reset or delete cup data as needed
        cupManager.DeleteCupFile();
    }

    private void EnableGoldPopup(Dictionary<string, string> reward)
    {
        cupGoldAndGems.SetActive(true);
        CurrencyHandler.instance.AddGold(int.Parse(reward["gold"]));
        goldOrGemsTitle.text = "GOLD REWARD";
        cupGoldPopup.SetActive(true);
        goldQuantity.text = reward["gold"];
    }

    private void EnableGemsPopup(Dictionary<string, string> reward)
    {
        cupGoldAndGems.SetActive(true);
        CurrencyHandler.instance.AddGems(int.Parse(reward["gems"]));
        goldOrGemsTitle.text = "GEMS REWARD";
        cupGemsPopup.SetActive(true);
        gemsQuantity.text = reward["gems"];
    }

    private void EnableChestPopup(Dictionary<string, string> reward)
    {
        cupSkills.SetActive(true);
        SkillPopUpLogic(reward);
    }

    // Handle chest
    // TODO v2: This function is declared in 4 different places: Cup, shop, levelup & dailygift
    // We could create a static class to reuse all of this logic. 
    // Its a bit of a mess because the functions called inside this function are different on each class
    private void SkillPopUpLogic(Dictionary<string, string> reward)
    {
        SkillCollection.SkillRarity skillRarityAwarded = GetRandomSkillRarityBasedOnChest(reward);
        Skill skillInstance = GetAwardedSkill(skillRarityAwarded);
        Fighter player = PlayerUtils.FindInactiveFighter();
        player.skills.Add(skillInstance);
        player.SaveFighter();
        Notifications.TurnOnNotification();
        Notifications.IncreaseCardsUnseen();

        ShowSkillData(skillInstance);
        ShowSkillIcon(skillInstance);
    }

    private void ShowSkillData(Skill skill)
    {
        skillTitle.GetComponent<TextMeshProUGUI>().text = skill.skillName;
        skillType.GetComponent<TextMeshProUGUI>().text = skill.category;
        skillRarity.GetComponent<TextMeshProUGUI>().text = skill.rarity;
        skillDescription.GetComponent<TextMeshProUGUI>().text = skill.description;
    }

    private bool HasSkillAlready(OrderedDictionary skill)
    {
        return player.skills.Any(playerSkill => playerSkill.skillName == skill["name"].ToString());
    }
    private Skill GetAwardedSkill(SkillCollection.SkillRarity skillRarityAwarded)
    {
        //List of OrderedDictionaries
        //Filter the ones that are from another rarity and the ones the player already has
        var skills = SkillCollection.skills
            .Where(skill => (string)skill["skillRarity"] == skillRarityAwarded.ToString())
            .Where(skill => !HasSkillAlready(skill))
            .ToList();

        Debug.Log(SkillCollection.skills
            .Where(skill => !HasSkillAlready(skill)).ToList().Count + " " + skillRarityAwarded);

        //If player has all skill for the current rarity get skills from a rarity above. 
        //Does not matter that they might not belong to the current chest
        if (!skills.Any())
        {
            Debug.Log("User has all skills for the given rarity.");

            //Cast enum to int

            int skillRarityIndex = (int)skillRarityAwarded;

            //If value for the next index in the enum exists return that rarity. Otherwise return the first value of the enum (COMMON)
            SkillCollection.SkillRarity newRarity = (Enum.IsDefined(typeof(SkillCollection.SkillRarity), (SkillCollection.SkillRarity)skillRarityIndex++) && skillRarityIndex < 4)
            ? (SkillCollection.SkillRarity)skillRarityIndex++
            : (SkillCollection.SkillRarity)0;

            //Recursive call with the new rarity
            return GetAwardedSkill(newRarity);
        }

        int skillIndex = UnityEngine.Random.Range(0, skills.Count());

        //OrderedDictionary
        var awardedSkill = skills[skillIndex];

        return new Skill(awardedSkill["name"].ToString(), awardedSkill["description"].ToString(),
                awardedSkill["skillRarity"].ToString(), awardedSkill["category"].ToString(), awardedSkill["icon"].ToString());
    }

    private SkillCollection.SkillRarity GetRandomSkillRarityBasedOnChest(Dictionary<string, string> reward)
    {
        Dictionary<SkillCollection.SkillRarity, float> skillRarityProbabilitiesForChest =
            Chest.shopChests[(Chest.ShopChestTypes)Enum.Parse
            (typeof(Chest.ShopChestTypes), reward["chest"].ToUpper())];

        float diceRoll = UnityEngine.Random.Range(0f, 100);

        foreach (KeyValuePair<SkillCollection.SkillRarity, float> skill in skillRarityProbabilitiesForChest)
        {
            if (skill.Value >= diceRoll)
                return skill.Key;

            diceRoll -= skill.Value;
        }

        Debug.LogError("Error");
        //Fallback
        return SkillCollection.SkillRarity.COMMON;
    }

    private void ShowSkillIcon(Skill skill)
    {
        //ShowFrame
        commonSkill.SetActive(SkillCollection.SkillRarity.COMMON == GeneralUtils.StringToSkillRarityEnum(skill.rarity));
        rareSkill.SetActive(SkillCollection.SkillRarity.RARE == GeneralUtils.StringToSkillRarityEnum(skill.rarity));
        epicSkill.SetActive(SkillCollection.SkillRarity.EPIC == GeneralUtils.StringToSkillRarityEnum(skill.rarity));
        legendarySkill.SetActive(SkillCollection.SkillRarity.LEGENDARY == GeneralUtils.StringToSkillRarityEnum(skill.rarity));

        //Show icon
        Sprite icon = Resources.Load<Sprite>("Icons/IconsSkills/" + skill.icon);
        skillIcon.GetComponent<Image>().sprite = icon;
    }
}
 