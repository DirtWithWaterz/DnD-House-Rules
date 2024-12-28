using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalendarUI : MonoBehaviour
{

    [SerializeField] TMP_Text CurrentDate;
    [SerializeField] TMP_Text DayOfMonth;
    [SerializeField] TMP_Text Time;
    [SerializeField] TMP_Text DayName;
    [SerializeField] TMP_Text DayOfWeek;
    [SerializeField] TMP_Text MonthName;
    [SerializeField] TMP_Text MonthOfYear;

    [SerializeField] GameObject[] tabline = new GameObject[0];
    [SerializeField] GameObject[] page = new GameObject[0];

    [SerializeField] Transform[] planetPositions = new Transform[0];
    [SerializeField] GameObject planet;
    [SerializeField] TMP_Text SeasonName;

    [SerializeField] TMP_Text Sunrise;
    [SerializeField] TMP_Text Sunset;

    InqueCalendar inqueCalendar;

    // Start is called before the first frame update
    void Start()
    {
        inqueCalendar = GameObject.Find("Calendar UI").GetComponent<InqueCalendar>();

        foreach(GameObject g in tabline){

            g.SetActive(true);
            page[g.transform.parent.GetComponent<CalendarButton>().index].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsTabOpen(1)){

            CurrentDate.text =
                $"{inqueCalendar.currentYear:0000}-{inqueCalendar.currentMonth:00}-{inqueCalendar.currentMonthday:00}";
            
            DayOfMonth.text =
                $"{inqueCalendar.currentMonthday:00}/{inqueCalendar.totalMonthdays:00}";
            
            Time.text =
                $"{inqueCalendar.currentHour:00}:{inqueCalendar.currentMin:00}";

            DayName.text =
                $"{inqueCalendar.weekday_t}";

            DayOfWeek.text =
                $"{inqueCalendar.currentWeekday}/{inqueCalendar.totalWeekdays}";

            MonthName.text =
                $"{inqueCalendar.month_t}";

            MonthOfYear.text =
                $"{inqueCalendar.currentMonth}/{inqueCalendar.totalMonths}";
        }
        if(IsTabOpen(2)){

            Transform selectedPlanetPos = planetPositions[inqueCalendar.currentSeason];

            planet.transform.position = selectedPlanetPos.position;

            SeasonName.text = $"SEASON: {inqueCalendar.seasons[inqueCalendar.currentSeason].name}";

            Sunrise.text = $"{inqueCalendar.currentSunrise.x:00}:{inqueCalendar.currentSunrise.y:00}";
            Sunset.text = $"{inqueCalendar.currentSunset.x:00}:{inqueCalendar.currentSunset.y:00}";
        }
    }

    public void OpenTab(int index){

        foreach(GameObject g in tabline){

            if(g.transform.parent.GetComponent<CalendarButton>().index != index)
                g.SetActive(true);
            page[g.transform.parent.GetComponent<CalendarButton>().index].SetActive(false);
        }

        tabline[index].SetActive(false);
        page[index].SetActive(true);
    }

    public bool IsTabOpen(int index){

        return page[index].activeInHierarchy;
    }
}
