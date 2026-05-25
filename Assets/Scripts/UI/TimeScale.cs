using UnityEngine;

public class TimeScale : MonoBehaviour
{
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void Pause()
    {
        SetTimeScale(0f);
    }

    public void Resume()
    {
        SetTimeScale(1f);
    }
}
