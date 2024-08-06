
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CupManagerv2 : MonoBehaviour
{
    private int TotalParticipants ;

    private void Awake()
    {
        if (File.Exists(JsonDataManager.getFilePath(JsonDataManager.CupFileName)))
            JsonDataManager.ReadCupFile();
        else
            CreateCupFile();


        TotalParticipants = FetchTotalParticipants();
    }

    private int FetchTotalParticipants()
    {
        // Example: Fetch total number of participants dynamically (replace with your logic)
        int totalParticipants = 0;

        // Example: Count authenticated users or registered participants
      //  totalParticipants = UserManager.GetAuthenticatedUserCount();

        // Alternatively, count registered participants if applicable
        // totalParticipants = RegistrationManager.GetRegisteredParticipantCount();

        // Ensure to return a valid count based on your application logic
        return totalParticipants;
    }

    private void CreateCupFile()
    {
        string cupName = CupDB.CupNames.DIVINE.ToString();
        string round = CupDB.CupRounds.QUARTERS.ToString();
        bool isActive = false;
        bool playerStatus = true;
        List<CupFighter> participants = GenerateParticipants();
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = GenerateCupInitialInfo();

        CupFactory.CreateCupInstance(cupName, isActive, playerStatus, round, participants, cupInfo);
        JObject cup = JObject.FromObject(Cup.Instance);
        JsonDataManager.SaveData(cup, JsonDataManager.CupFileName);
    }

    public void DeleteCupFile()
    {
        string[] filesAndroid = Directory.GetFiles(Path.GetDirectoryName(Application.persistentDataPath));
        foreach (var file in filesAndroid)
        {
            if (file.Contains("cup"))
                File.Delete(file);
        }

        string[] filesPC = Directory.GetFiles(Application.persistentDataPath);
        foreach (var file in filesPC)
        {
            if (file.Contains("cup"))
                File.Delete(file);
        }
    }

    private List<CupFighter> GenerateParticipants()
    {
        Fighter player = PlayerUtils.FindInactiveFighter();

        List<CupFighter> participants = new List<CupFighter>();

        CupFighter user = new CupFighter(0.ToString(), player.fighterName, player.species);
        participants.Add(user);

        for (int i = 1; i < TotalParticipants; i++)
        {
            participants.Add(
                new CupFighter(
                    i.ToString(),
                    RandomNameGenerator.GenerateRandomName(),
                    GeneralUtils.GetRandomSpecies()
                )
            );
        }

        return participants;
    }

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> GenerateCupInitialInfo()
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        // Generate quarter-finals
        cupInfo.Add(CupDB.CupRounds.QUARTERS.ToString(), GenerateRoundInfo(TotalParticipants / 4));

        // Generate semi-finals
        cupInfo.Add(CupDB.CupRounds.SEMIS.ToString(), GenerateRoundInfo(TotalParticipants / 8));

        // Generate finals
        cupInfo.Add(CupDB.CupRounds.FINALS.ToString(), GenerateRoundInfo(2));

        return cupInfo;
    }

    private Dictionary<string, Dictionary<string, string>> GenerateRoundInfo(int numberOfMatches)
    {
        Dictionary<string, Dictionary<string, string>> roundInfo = new Dictionary<string, Dictionary<string, string>>();

        for (int i = 1; i <= numberOfMatches; i++)
        {
            roundInfo.Add(i.ToString(), new Dictionary<string, string>
            {
                { "matchId", i.ToString() },
                { "1", "" },
                { "2", "" },
                { "winner", "" },
                { "loser", "" }
            });
        }

        return roundInfo;
    }

    public void SimulateQuarters(bool hasPlayerWon)
    {
        SimulateRound(CupDB.CupRounds.QUARTERS.ToString(), TotalParticipants / 4, hasPlayerWon);
        Cup.Instance.round = CupDB.CupRounds.SEMIS.ToString();
        GenerateCupSemisInfo();
    }

    private void SimulateRound(string roundName, int numberOfMatches, bool hasPlayerWon)
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = Cup.Instance.cupInfo;

        for (int i = 1; i <= numberOfMatches; i++)
        {
            List<string> match = new List<string>
            {
                cupInfo[roundName][i.ToString()]["1"],
                cupInfo[roundName][i.ToString()]["2"]
            };

            if (match.Contains("0") && hasPlayerWon)
            {
                cupInfo[roundName][i.ToString()]["winner"] = "0";
                cupInfo[roundName][i.ToString()]["loser"] = match[0] == "0" ? match[1] : match[0];
            }
            else
            {
                int random = UnityEngine.Random.Range(0, match.Count);
                cupInfo[roundName][i.ToString()]["winner"] = match[random];
                cupInfo[roundName][i.ToString()]["loser"] = match[random == 0 ? 1 : 0];
            }
        }

        Cup.Instance.cupInfo = cupInfo;
        Cup.Instance.SaveCup();
    }

    private void GenerateCupSemisInfo()
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = Cup.Instance.cupInfo;
        List<CupFighter> participants = GenerateParticipantsBasedOnRound(CupDB.CupRounds.QUARTERS.ToString());

        int matchId = 1;
        for (int i = 0; i < participants.Count; i += 2)
        {
            cupInfo[CupDB.CupRounds.SEMIS.ToString()][matchId.ToString()]["1"] = participants[i].id;
            cupInfo[CupDB.CupRounds.SEMIS.ToString()][matchId.ToString()]["2"] = participants[i + 1].id;
            matchId++;
        }

        Cup.Instance.cupInfo = cupInfo;
        Cup.Instance.SaveCup();
    }

    private List<CupFighter> GenerateParticipantsBasedOnRound(string roundName)
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = Cup.Instance.cupInfo;
        List<CupFighter> participants = Cup.Instance.participants;
        List<CupFighter> roundParticipants = new List<CupFighter>();

        foreach (var match in cupInfo[roundName])
        {
            if (!string.IsNullOrEmpty(match.Value["winner"]))
            {
                roundParticipants.Add(participants.Find(p => p.id == match.Value["winner"]));
            }
        }

        return roundParticipants;
    }

    public void SimulateSemis(bool hasPlayerWon)
    {
        SimulateRound(CupDB.CupRounds.SEMIS.ToString(), TotalParticipants / 8, hasPlayerWon);
        Cup.Instance.round = CupDB.CupRounds.FINALS.ToString();
        GenerateCupFinalsInfo();
    }

    private void GenerateCupFinalsInfo()
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = Cup.Instance.cupInfo;
        List<CupFighter> participants = GenerateParticipantsBasedOnRound(CupDB.CupRounds.SEMIS.ToString());

        cupInfo[CupDB.CupRounds.FINALS.ToString()]["1"]["1"] = participants[0].id;
        cupInfo[CupDB.CupRounds.FINALS.ToString()]["1"]["2"] = participants[1].id;

        Cup.Instance.cupInfo = cupInfo;
        Cup.Instance.SaveCup();
    }

    public void SimulateFinals(bool hasPlayerWon)
    {
        SimulateRound(CupDB.CupRounds.FINALS.ToString(), 1, hasPlayerWon);

        // Determine winner and save results
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> cupInfo = Cup.Instance.cupInfo;
        string winnerId = cupInfo[CupDB.CupRounds.FINALS.ToString()]["1"]["winner"];
        string loserId = cupInfo[CupDB.CupRounds.FINALS.ToString()]["1"]["loser"];

        if (winnerId == "0")
        {
            // Player won the finals
            Debug.Log("Player won the cup!");
            ProfileData.SaveCups(); // Save cup won on profile
        }
        else
        {
            // Player lost the finals
            Debug.Log("Player lost the cup.");
        }

        Cup.Instance.round = CupDB.CupRounds.END.ToString();
        Cup.Instance.SaveCup();
    }
}
