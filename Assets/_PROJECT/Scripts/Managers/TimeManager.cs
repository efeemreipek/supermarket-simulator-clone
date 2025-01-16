using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private int startDay = 1;
    [SerializeField] private int startHour = 7;
    [SerializeField] private float minuteTime = 1f;

    private int hour, minute, day;
    private float timer = 0f;
    private string timeString = string.Empty;
    private string dayString = string.Empty;

    public static event Action OnDayChanged;

    private void Start()
    {
        hour = startHour;
        day = startDay;

        dayString = $"DAY {day.ToString("D2")}";
        timeString = $"{hour.ToString("D2")}:{minute.ToString("D2")}";
        UIManager.Instance.UpdateDayTimeText(dayString, timeString);
    }
    private void Update()
    {
        if(!PauseManager.Instance.IsGameStarted || PauseManager.Instance.IsPaused) return;

        if(timer >= minuteTime)
        {
            minute++;
            if(minute >= 60)
            {
                minute = 0;
                hour++;
                if(hour >= 20)
                {
                    hour = startHour;
                    day++;
                    OnDayChanged?.Invoke();

                    dayString = $"DAY {day.ToString("D2")}";
                }
            }

            timer = 0f;

            timeString = $"{hour.ToString("D2")}:{minute.ToString("D2")}";
            UIManager.Instance.UpdateDayTimeText(dayString, timeString);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
