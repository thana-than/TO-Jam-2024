using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TimeTracker
{
    [SerializeField] double m_time = 0;
    [SerializeField] double m_previousUpdateTime = 0;
    public double time => m_time;
    public double previousUpdateTime => m_previousUpdateTime;

    public TimeScale timeScale;

    public enum TimeScale
    {
        deltaTime,
        unscaledDeltaTime,
    }

    public TimeTracker(double startTime = 0, TimeScale timeScale = TimeScale.deltaTime)
    {
        SetTime(startTime);
        this.timeScale = timeScale;
    }

    public void Restart()
    {
        SetTime(0);
    }

    public void SetTime(double time)
    {
        this.m_time = time;
        m_previousUpdateTime = -1;
    }

    public void Update()
    {
        m_previousUpdateTime = m_time;

        switch (timeScale)
        {
            case TimeScale.deltaTime:
                m_time += Time.deltaTime;
                break;

            case TimeScale.unscaledDeltaTime:
                m_time += Time.unscaledDeltaTime;
                break;
        }
    }

    // public override string ToString()
    // {
    //     return Localization.Instance.SecondsToTimeString(time);
    // }

    // public void SetTextToTime(ref TMP_Text textAsset)
    // {
    //     Localization.Instance.SetTextToTime(ref textAsset, time);
    // }

    public static implicit operator float(TimeTracker tracker)
    {
        return (float)tracker.time;
    }

    public static implicit operator double(TimeTracker tracker)
    {
        return tracker.time;
    }
}
