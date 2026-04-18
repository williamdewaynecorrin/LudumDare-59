using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // -- singleton
    private static GameManager instance = null;
    private static float gSFXVolume = 1.0f;
    private const float kDefault3DMaxVolumeDistance = 10.0f;
    private const float kDefault3DMinVolumeDistance = 100.0f;

    private static float gMusicVolume = 1.0f;

    // -- inspectable
    public AudioClip levelmusic;
    public AudioSource musicsource;
    public AudioSource sfxsource;

    void Awake()
    {
        if(instance == null)
        {
            OnFirstInitialize();
            instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
            return;
        }

        // -- use new level's music
        instance.musicsource.Stop();
        instance.musicsource.clip = levelmusic;
        instance.musicsource.time = 0.0f;
        instance.musicsource.Play();

        // -- destroy new instance
        GameObject.Destroy(this.gameObject);
    }

    private void OnFirstInitialize()
    {
        musicsource.loop = true;
        musicsource.clip = levelmusic;
        musicsource.Play();

        sfxsource.loop = false;
    }

    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    public static void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public static AudioSource Play2D(AudioClipXT clip)
    {
        AudioSource source = instance.CreateInternal(clip);
        return source;
    }

    public static AudioSource Play3D(AudioClipXT clip, Vector3 pos)
    {
        AudioSource source = instance.CreateInternal(clip);
        source.transform.position = pos;
        source.minDistance = kDefault3DMaxVolumeDistance;
        source.maxDistance = kDefault3DMinVolumeDistance;

        return source;
    }

    private AudioSource CreateInternal(AudioClipXT clip)
    {
        GameObject newaudio = new GameObject(clip.clip.name);
        AudioSource source = newaudio.AddComponent<AudioSource>();
        source.clip = clip.clip;
        source.volume = gSFXVolume * clip.volumemult;
        source.pitch = clip.pitchvariance.PickValue();

        GameObject.Destroy(newaudio, clip.clip.length);
        return source;
    }
}
