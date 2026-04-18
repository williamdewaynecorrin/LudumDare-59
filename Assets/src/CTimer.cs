using UnityEngine;

[System.Serializable]
public class CTimer
{
    public float time;
    private float maxtime;

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
}
