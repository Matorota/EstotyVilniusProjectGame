using System;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    private int _pauseCount;

    private void Awake()
    {
        Application.targetFrameRate = 60; // reminder
    }

    public void Pause()
    {
        _pauseCount++;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _pauseCount = Mathf.Max(0, _pauseCount - 1);
        if (_pauseCount == 0)
            Time.timeScale = 1f;
    }
}
