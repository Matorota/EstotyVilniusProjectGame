using UnityEngine;

public class TimeScale : MonoBehaviour
{
    private int _pauseCount;

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
