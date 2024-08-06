using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CupTimer : MonoBehaviour
{
    // UI
    [SerializeField]
    private GameObject buttonCup;

    [SerializeField]
    private GameObject buttonBypass;

    [SerializeField]
    private Image cupIcon;
    [SerializeField]
    private TextMeshProUGUI textCup;
    [SerializeField]
    private TextMeshProUGUI textCupDisabled;
    [SerializeField]
    private GameObject timerGO;
    [SerializeField]
    private TextMeshProUGUI textTimer;
    [SerializeField]
    private Image lockIcon;

    // Weekly timer settings
    [SerializeField]
    private DayOfWeek targetDay = DayOfWeek.Monday;
    [SerializeField]
    private int targetHour = 22; // 10PM
    [SerializeField]
    private int targetMinute = 0;
    [SerializeField]
    private int targetSecond = 0;

    private void Awake()
    {
        SetupUI();
        SetupCup();


        //buttonBypass.GetComponent<Button>().onClick.AddListener(BypassCupTimer);

    }

    private void SetupUI()
    {


        //buttonBypass = GameObject.Find("Button_Bypass");
    }

    private void Update()
    {
        SetupCup();
    }

    private void SetupCup()
    {
        if (IsCupAvailable())
            EnableCup();
        if (UpdateTimer() >= TimeSpan.Zero)
        {
            DisableCup();
            timerGO.SetActive(true);
            TimeSpan timer = UpdateTimer();
            textTimer.text = $"{timer.Days}d {timer.Hours}h {timer.Minutes}m {timer.Seconds}s";
        }
    }

    private void EnableCup()
    {
        buttonCup.GetComponent<Button>().enabled = true;
        cupIcon.enabled = true;
        lockIcon.enabled = false;
        textCup.enabled = true;
        textCupDisabled.enabled = false;
        timerGO.SetActive(false);
    }

    private void DisableCup()
    {
        buttonCup.GetComponent<Button>().enabled = false;
        cupIcon.enabled = false;
        lockIcon.enabled = true;
        textCup.enabled = false;
        textCupDisabled.enabled = true;
        timerGO.SetActive(true);
    }

    private bool IsCupAvailable()
    {
        if (!PlayerPrefs.HasKey("firstCup"))
        {
            SetFirstCup();
            return false;
        }

        if (HasCupCooldownElapsed())
            return true;

        return false;
    }

    private void SetFirstCup()
    {
        PlayerPrefs.SetInt("firstCup", 1);
        PlayerPrefs.Save();
        StartCountdown();
    }

    private bool HasCupCooldownElapsed()
    {
        if (PlayerPrefs.HasKey("cupCountdown"))
        {
            string countdownBinary = PlayerPrefs.GetString("cupCountdown");
            long countdownTicks = Convert.ToInt64(countdownBinary);
            DateTime countdownEndTime = DateTime.FromBinary(countdownTicks);
            return DateTime.Now >= countdownEndTime;
        }
        return false;
    }

    private void StartCountdown()
    {
        DateTime nextEventTime = CalculateNextEventTime();
        PlayerPrefs.SetString("cupCountdown", nextEventTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private DateTime CalculateNextEventTime()
    {
        DateTime now = DateTime.Now;
        DateTime nextEventTime = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, targetSecond);

        // Adjust the date to the next target day
        while (nextEventTime.DayOfWeek != targetDay || nextEventTime <= now)
        {
            nextEventTime = nextEventTime.AddDays(1);
        }

        return nextEventTime;
    }

    private TimeSpan UpdateTimer()
    {
        if (PlayerPrefs.HasKey("cupCountdown"))
        {
            long countdownTicks = Convert.ToInt64(PlayerPrefs.GetString("cupCountdown"));
            DateTime countdownEndTime = DateTime.FromBinary(countdownTicks);
            return countdownEndTime - DateTime.Now;
        }
        return TimeSpan.Zero;
    }

    // Bypass the timer for testing
    private void BypassCupTimer()
    {
        PlayerPrefs.SetString("cupCountdown", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
        SetupCup();
    }
}
