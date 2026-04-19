using UnityEngine;

[System.Serializable]
public class CTimer
{
    public float time;

    [HideInInspector]
    public float maxtime;

    public void Init()
    {
        maxtime = time;
    }

    public bool Tick(float dt)
    {
        time -= dt;
        return TimerReached();
    }

    public bool TimerReached()
    {
        return time <= 0.0f;
    }

    public void Reset()
    {
        time = maxtime;
    }

    public float GetLerpValue()
    {
        return Mathf.Clamp01(1.0f - (time / maxtime));
    }
}
