using UnityEngine;

public class GameTimeTracker : MonoBehaviour
{
    public static GameTimeTracker Instance;

    private float sessionStartTime;
    private float totalPlayTime;
    private bool isTracking;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartTracking();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTracking()
    {
        sessionStartTime = Time.time;
        isTracking = true;
    }

    public void StopTracking()
    {
        if (isTracking)
        {
            float sessionTime = Time.time - sessionStartTime;
            totalPlayTime += sessionTime;
            isTracking = false;
        }
    }

    public void PauseTracking()
    {
        if (isTracking)
        {
            float sessionTime = Time.time - sessionStartTime;
            totalPlayTime += sessionTime;
            isTracking = false;
        }
    }

    public void ResumeTracking()
    {
        sessionStartTime = Time.time;
        isTracking = true;
    }

    public float GetCurrentTotalPlayTime()
    {
        float currentTotal = totalPlayTime;
        if (isTracking)
        {
            currentTotal += (Time.time - sessionStartTime);
        }
        return currentTotal;
    }

    public void SetTotalPlayTime(float time)
    {
        totalPlayTime = time;
        sessionStartTime = Time.time;
    }

    public void ResetTimer()
    {
        totalPlayTime = 0f;
        sessionStartTime = Time.time;
        isTracking = true;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseTracking();
        }
        else
        {
            ResumeTracking();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseTracking();
        }
        else
        {
            ResumeTracking();
        }
    }
}