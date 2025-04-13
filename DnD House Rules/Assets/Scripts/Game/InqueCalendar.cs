using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InqueCalendar : NetworkBehaviour
{

    public Vector2 currentSunrise; //Vector2(hour, min)
    public Vector2 currentSunset; //Vector2(hour, min)


#region Calendar Definitions

    public NetworkVariable<FixedString64Bytes> month_t = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString64Bytes> weekday_t = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public Dictionary<int, string> monthNames = new Dictionary<int, string>{

        {0, "Undefined"},
        {1, "Kiriel"},
        {2, "Aniel"},
        {3, "Jundail"},
        {4, "Bariel"},
        {5, "Maliel"},
        {6, "Aline"},
        {7, "Ramut"},
        {8, "Uzziel"}
    };
    public Dictionary<int, string> dayNames = new Dictionary<int, string>{

        {0, "Undefined"},
        {1, "Reulian"},
        {2, "Riealian"},
        {3, "Ramuen"},
        {4, "Achian"},
        {5, "Salechien"}
    };
    public Dictionary<int, int> monthDayNum = new Dictionary<int, int>{

        {0, 0},
        {1, 46},
        {2, 46},
        {3, 46},
        {4, 46},
        {5, 46},
        {6, 50},
        {7, 46},
        {8, 46}
    };

    public int totalMonths = 8;
    public NetworkVariable<int> totalMonthdays = new NetworkVariable<int>(0);
    public int totalWeekdays = 5;
    public int totalDays = 373;
    public int totalHours = 35;
    public int totalMins = 59;
    public int totalSecs = 59;


    public NetworkVariable<int> currentYear = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentYearday = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentMonth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentMonthday = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentWeekday = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentDay = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentHour = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentMin = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> currentSec = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
#endregion

#region Moon Phase Definitions
    [SerializeField] Sprite[] phases = new Sprite[0];
    [SerializeField] Moon[] moons = new Moon[0];
#endregion

#region Season Definitions

    public int seasonOffset = 0; // Number of days to offset the seasons
    public Season[] seasons = new Season[0];

    public NetworkVariable<int> currentSeasonday = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> currentSeason = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

#endregion

    int timeVar = 0;
    int lastDay = 0;

    CalendarUI calendarUI;

    User user;

    IEnumerator Start(){

        yield return new WaitUntil(() => GameManager.Singleton.userDatas != null);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Game");
        for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

            if(GameManager.Singleton.userDatas[i].id == NetworkManager.LocalClientId)
                user = GameObject.Find(GameManager.Singleton.userDatas[i].username.ToString()).GetComponent<User>();
        }
        calendarUI = user.GetComponentInChildren<CalendarUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(user == null)
            return;
        if(!user.isInitialized.Value)
            return;
        
        GetCurrentSeason(out Season currentSeason, out Season previousSeason, out Season nextSeason);

        float normalizedSeasonday = Mathf.Clamp01((float)currentSeasonday.Value / (float)currentSeason.duration);
        currentSunrise = new Vector2(Mathf.Lerp((float)currentSeason.rise_hour, (float)nextSeason.rise_hour, normalizedSeasonday), Mathf.Lerp((float)currentSeason.rise_minute, (float)nextSeason.rise_minute, normalizedSeasonday));
        currentSunset = new Vector2(Mathf.Lerp((float)currentSeason.set_hour, (float)nextSeason.set_hour, normalizedSeasonday), Mathf.Lerp((float)currentSeason.set_minute, (float)nextSeason.set_minute, normalizedSeasonday));
        // Debug.Log($"Mathf.Clamp01({currentSeasonday.Value} / {currentSeason.duration}) = {normalizedSeasonday}");
        if(Time.time >= timeVar+1){

            if(IsHost)
                CalculateDateAndTimeRpc();

            timeVar = Mathf.FloorToInt(Time.time);
        }
        if(currentDay.Value != lastDay && calendarUI.IsTabOpen(0)){

            foreach(Moon moon in moons){

                // Update the moon's cycle position
                moon.cyclePos = (currentDay.Value + moon.shift) % moon.cycle;

                // Calculate the current phase within the 0-11 range
                moon.currentPhase = Mathf.FloorToInt((float)moon.cyclePos / moon.cycle * phases.Length);

                // Ensure currentPhase is clamped within the range of available sprites
                moon.currentPhase = Mathf.Clamp(moon.currentPhase, 0, phases.Length - 1);

                // Set the corresponding sprite
                GameObject gO = GameObject.Find(moon.name);
                Image rend = gO.GetComponent<Image>();
                rend.sprite = phases[moon.currentPhase];
                rend.color = moon.color;
            }

            // Update lastDay to the current day
            lastDay = currentDay.Value;
        }
    }

#region Calculate date and time

    [Rpc(SendTo.Server)]
    void CalculateDateAndTimeRpc(){

        currentSec.Value =
            currentSec.Value>=
            totalSecs?0:
            currentSec.Value+1;
        
        currentMin.Value =
            currentMin.Value>=
            totalMins&&
            currentSec.Value==0?0:
            currentSec.Value==0?
            currentMin.Value+1:
            currentMin.Value;

        currentHour.Value =
            currentHour.Value>=
            totalHours&&
            currentMin.Value==0&&
            currentSec.Value==0?0:
            currentMin.Value==0&&
            currentSec.Value==0?
            currentHour.Value+1:
            currentHour.Value;

        currentYearday.Value =
            currentYearday.Value>=
            totalDays&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?1:
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentYearday.Value+1:
            currentYearday.Value;
        
        currentDay.Value =
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentDay.Value+1:
            currentDay.Value;

        currentWeekday.Value =
            currentWeekday.Value>=
            totalWeekdays&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?1:
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentWeekday.Value+1:
            currentWeekday.Value;

        totalMonthdays.Value =
            monthDayNum[currentMonth.Value];

        currentMonthday.Value =
            currentMonthday.Value>=
            totalMonthdays.Value&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?1:
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentMonthday.Value+1:
            currentMonthday.Value;
        
        currentMonth.Value =
            currentMonth.Value>=
            totalMonths&&
            currentMonthday.Value==1&&
            currentYearday.Value==1&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?1:
            currentMonthday.Value==1&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentMonth.Value+1:
            currentMonth.Value;
        
        currentYear.Value =
            currentMonth.Value==1&&
            currentYearday.Value==1&&
            currentHour.Value==0&&
            currentMin.Value==0&&
            currentSec.Value==0?
            currentYear.Value+1:
            currentYear.Value;

        weekday_t.Value = dayNames[currentWeekday.Value];
        month_t.Value = monthNames[currentMonth.Value];
    }
#endregion

#region Season stuff

    float Lerp(float start, float end, float t)
    {
        return start + (end - start) * t;
    }

    public void GetCurrentSeason(out Season currentSeason, out Season previousSeason, out Season nextSeason)
    {
        currentSeason = null;
        previousSeason = null;
        nextSeason = null;

        int seasonStart = 1;

        int adjustedYearday = (currentYearday.Value + seasonOffset) % totalDays;
        if (adjustedYearday <= 0) adjustedYearday += totalDays;


        for (int i = 0; i < seasons.Length; i++)
        {
            int seasonEnd = seasonStart + seasons[i].duration - 1;

            if (adjustedYearday >= seasonStart && adjustedYearday <= seasonEnd)
            {
                currentSeason = seasons[i];
                previousSeason = i > 0 ? seasons[i - 1] : seasons[seasons.Length - 1]; // Wrap around if the current season is the first
                nextSeason = i < seasons.Length - 1 ? seasons[i + 1] : seasons[0]; // Wrap around if the current season is the last

                SetCurrentSeasonRpc(adjustedYearday, seasonStart, i);
                break;
            }

            seasonStart = seasonEnd + 1; // Move start to the next season
        }
    }
    [Rpc(SendTo.Server)]
    void SetCurrentSeasonRpc(int adjustedYearday, int seasonStart, int val){

        currentSeasonday.Value = adjustedYearday-seasonStart;
        currentSeason.Value = val;
    }
#endregion
}
