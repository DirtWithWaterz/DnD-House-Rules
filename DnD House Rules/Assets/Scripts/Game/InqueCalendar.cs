using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InqueCalendar : MonoBehaviour
{

    public Vector2 currentSunrise; //Vector2(hour, min)
    public Vector2 currentSunset; //Vector2(hour, min)


#region Calendar Definitions

    public string month_t;
    public string weekday_t;


    Dictionary<int, string> monthNames = new Dictionary<int, string>{

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
    Dictionary<int, string> dayNames = new Dictionary<int, string>{

        {0, "Undefined"},
        {1, "Reulian"},
        {2, "Riealian"},
        {3, "Ramuen"},
        {4, "Achian"},
        {5, "Salechien"}
    };
    Dictionary<int, int> monthDayNum = new Dictionary<int, int>{

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
    public int totalMonthdays;
    public int totalWeekdays = 5;
    public int totalDays = 373;
    public int totalHours = 35;
    public int totalMins = 59;
    public int totalSecs = 59;


    public int currentYear = 0;
    public int currentYearday = 1;
    public int currentMonth = 1;
    public int currentMonthday = 1;
    public int currentWeekday = 1;
    public int currentDay = 1;
    public int currentHour = 0;
    public int currentMin = 0;
    public int currentSec = 0;
#endregion

#region Moon Phase Definitions
    [SerializeField] Sprite[] phases = new Sprite[0];
    [SerializeField] Moon[] moons = new Moon[0];
#endregion

#region Season Definitions

    [SerializeField] int seasonOffset = 0; // Number of days to offset the seasons
    public Season[] seasons = new Season[0];

    public int currentSeasonday = 1;

    public int currentSeason;

#endregion

    int timeVar = 0;
    int lastDay = 0;

    CalendarUI calendarUI;

    void Awake(){

        calendarUI = GameObject.Find("Calendar UI").GetComponent<CalendarUI>();
    }

    // Update is called once per frame
    void Update()
    {

        GetCurrentSeason(out Season currentSeason, out Season previousSeason, out Season nextSeason);

        currentSunrise = new Vector2(Mathf.Lerp(previousSeason.rise_hour, currentSeason.rise_hour, currentSeasonday), Mathf.Lerp(previousSeason.rise_minute, currentSeason.rise_minute, currentSeasonday));
        currentSunset = new Vector2(Mathf.Lerp(previousSeason.set_hour, currentSeason.set_hour, currentSeasonday), Mathf.Lerp(previousSeason.set_minute, currentSeason.set_minute, currentSeasonday));

        if(Time.time >= timeVar+1){

            CalculateDateAndTime();

            timeVar = Mathf.FloorToInt(Time.time);
        }
        if(currentDay != lastDay && calendarUI.IsTabOpen(0)){

            foreach(Moon moon in moons){

                // Update the moon's cycle position
                moon.cyclePos = (currentDay + moon.shift) % moon.cycle;

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
            lastDay = currentDay;
        }
    }

#region Calculate date and time
    void CalculateDateAndTime(){

        currentSec =
            currentSec>=
            totalSecs?0:
            currentSec+1;
        
        currentMin =
            currentMin>=
            totalMins&&
            currentSec==0?0:
            currentSec==0?
            currentMin+1:
            currentMin;

        currentHour =
            currentHour>=
            totalHours&&
            currentMin==0&&
            currentSec==0?0:
            currentMin==0&&
            currentSec==0?
            currentHour+1:
            currentHour;

        currentYearday =
            currentYearday>=
            totalDays&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?1:
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentYearday+1:
            currentYearday;
        
        currentDay =
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentDay+1:
            currentDay;

        currentWeekday =
            currentWeekday>=
            totalWeekdays&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?1:
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentWeekday+1:
            currentWeekday;

        totalMonthdays =
            monthDayNum[currentMonth];

        currentMonthday =
            currentMonthday>=
            totalMonthdays&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?1:
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentMonthday+1:
            currentMonthday;
        
        currentMonth =
            currentMonth>=
            totalMonths&&
            currentMonthday==1&&
            currentYearday==1&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?1:
            currentMonthday==1&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentMonth+1:
            currentMonth;
        
        currentYear =
            currentMonth==1&&
            currentYearday==1&&
            currentHour==0&&
            currentMin==0&&
            currentSec==0?
            currentYear+1:
            currentYear;

        weekday_t = dayNames[currentWeekday];
        month_t = monthNames[currentMonth];
    }
#endregion

#region Season stuff

float Lerp(float start, float end, float t)
{
    return start + (end - start) * t;
}

void GetCurrentSeason(out Season currentSeason, out Season previousSeason, out Season nextSeason)
{
    currentSeason = null;
    previousSeason = null;
    nextSeason = null;

    int seasonStart = 1;

    int adjustedYearday = (currentYearday + seasonOffset) % totalDays;
    if (adjustedYearday <= 0) adjustedYearday += totalDays;


    for (int i = 0; i < seasons.Length; i++)
    {
        int seasonEnd = seasonStart + seasons[i].duration - 1;

        if (adjustedYearday >= seasonStart && adjustedYearday <= seasonEnd)
        {
            currentSeason = seasons[i];
            previousSeason = i > 0 ? seasons[i - 1] : seasons[seasons.Length - 1]; // Wrap around if the current season is the first
            nextSeason = i < seasons.Length - 1 ? seasons[i + 1] : seasons[0]; // Wrap around if the current season is the last

            currentSeasonday = adjustedYearday-seasonStart;
            this.currentSeason = i;
            break;
        }

        seasonStart = seasonEnd + 1; // Move start to the next season
    }
}



#endregion
}
