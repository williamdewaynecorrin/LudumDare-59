using UnityEngine;

[System.Serializable]
public abstract class MinMax<T>
{
    public T min;
    public T max;

    protected T lastpickedvalue;
    public T LastPickedValue => lastpickedvalue;

    public abstract T PickValue();
}

[System.Serializable]
public class MinMaxFloat : MinMax<float>
{
    public override float PickValue()
    {
        lastpickedvalue = Random.Range(min, max);
        return lastpickedvalue;
    }
}

[System.Serializable]
public class MinMaxInt : MinMax<int>
{
    public override int PickValue()
    {
        lastpickedvalue = Random.Range(min, max);
        return lastpickedvalue;
    }
}
