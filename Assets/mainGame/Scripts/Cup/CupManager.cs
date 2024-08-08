using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CupManager : MonoBehaviour
{
    public static CupManager Instance;

    CupFighter[] cupFighters;
    int fighterCount = 0;

    private void Awake()
    {
        /*if (File.Exists(JsonDataManager.getFilePath(JsonDataManager.CupFileName)))
            JsonDataManager.ReadCupFile();
        else
            CreateCupFile();*/
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        cupFighters = new CupFighter[8];
    }

    private void Start()
    {
        for (int i = 0; i < cupFighters.Length; i++)
        {
            if (cupFighters[i] == null)
            {
                cupFighters[i] = new CupFighter()
                {
                    userID = PhotonNetwork.LocalPlayer.UserId,
                    fighterName = PhotonNetwork.LocalPlayer.NickName
                };
                Debug.Log($"Fighter {PhotonNetwork.LocalPlayer.NickName} was added to the array index {i}");
                fighterCount++;
                Debug.Log($"Fighter count: {fighterCount}");
                break;
            }
        }
        CupUIManager.Instance.UpdateCupUI(cupFighters);
    }

    private void CreateCupFile()
    {
        string cupName = CupDB.CupNames.DIVINE.ToString(); ;
        string round = CupDB.CupRounds.QUARTERS.ToString();
        /*bool isActive = false;
        bool playerStatus = true;*/
        List<CupFighter> participants = GenerateParticipants();
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = GenerateCupInitialInfo();

        //CupFactory.CreateCupInstance(cupName, isActive, playerStatus, round, participants, cupInfo);
        /*JObject cup = JObject.FromObject(Cup.Instance);
        JsonDataManager.SaveData(cup, JsonDataManager.CupFileName);*/
    }

    public void DeleteCupFile()
    {
        string[] filesAndroid = Directory.GetFiles(Path.GetDirectoryName(Application.persistentDataPath));
        foreach (var file in filesAndroid) 
            if(file.Contains("cup"))
                File.Delete(file);

        // Debug.Log(Directory.GetFiles(Application.persistentDataPath)[0]);

        string[] filesPC = Directory.GetFiles(Application.persistentDataPath);
        foreach (var file in filesPC)
            if (file.Contains("cup"))
                File.Delete(file);
    }

    private List<CupFighter> GenerateParticipants()
    {
        Fighter player = PlayerUtils.FindInactiveFighter();

        // there will be 8 fighters per cup (7 + user)
        List<CupFighter> participants = new List<CupFighter>();

        /*CupFighter user = new CupFighter(0.ToString(), player.fighterName, player.species);
        participants.Add(user);

        for (int i = 1; i < 8; i++)
        {
            participants.Add(
                new CupFighter(
                    i.ToString(),
                    RandomNameGenerator.GenerateRandomName(),
                    GeneralUtils.GetRandomSpecies()
                )
            );
        }*/

        return participants;
    }

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> GenerateCupInitialInfo()
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = 
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                { CupDB.CupRounds.QUARTERS.ToString(), new Dictionary<string, Dictionary<string, string>>
                    {
                        { "1", new Dictionary<string, string>
                            {
                                { "matchId", "1"} , // match id
                                { "1", "0"} ,       // seed 1 player
                                { "2", "1"} ,       // seed 2 player
                                { "winner" , ""} ,  // winner id
                                { "loser" , ""}     // loser id
                            }
                        },
                        { "2", new Dictionary<string, string>
                            {
                                { "matchId", "2"} ,
                                { "3", "2"} ,
                                { "4", "3"} ,
                                { "winner" , ""} ,  
                                { "loser" , ""}
                            }
                        },
                        { "3", new Dictionary<string, string>
                            {
                                { "matchId", "3"} ,
                                { "5", "4"} ,
                                { "6", "5"} ,
                                { "winner" , ""} ,  
                                { "loser" , ""}
                            }
                        },
                        { "4", new Dictionary<string, string>
                            {
                                { "matchId", "4"} ,
                                { "7", "6"} ,
                                { "8", "7"} ,
                                { "winner" , ""} ,  
                                { "loser" , ""}
                            }
                        },
                    }
                }
            };

        return cupInfo;
    }
}
