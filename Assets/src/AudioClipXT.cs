using UnityEngine;

[System.Serializable]
public class AudioClipXT
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volumemult = 1.0f;
    public MinMaxFloat pitchvariance;
}
