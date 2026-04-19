using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(ExecutionOrders.GAME_MANAGER)]
public class GameManager : MonoBehaviour
{
    // -- singleton
    public const uint kPlayerTeam = 0;
    public const uint kEnemyTeam = 1;

    private static GameManager instance = null;
    private static float gSFXVolume = 1.0f;
    private const float kDefault3DMaxVolumeDistance = 10.0f;
    private const float kDefault3DMinVolumeDistance = 100.0f;

    private static float gMusicVolume = 1.0f;
    private static PlayerController player = null;
    private static bool battlemusicactive = false;

    public static PlayerController Player => player;
    public static ObjectPooler TextPooler => instance.textpooler;
    public static ObjectPooler EnemyBulletPooler => instance.enemybulletpooler;

    // -- inspectable
    public float musictransitionspeed = 2.0f;
    public AudioClip levelmusic;
    public AudioSource musicsource;
    public AudioSource battlemusicsource;
    public AudioSource sfxsource;
    public ObjectPooler textpooler; 
    public ObjectPooler enemybulletpooler;

    private static bool wongame = false;

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

    void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerController>();   
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
        if(battlemusicactive)
        {
            battlemusicsource.volume += musictransitionspeed * Time.deltaTime;
            musicsource.volume -= musictransitionspeed * Time.deltaTime;

            if (musicsource.volume == 0.0f)
                musicsource.Pause();
        }
        else
        {
            if (musicsource.volume == 0.0f)
                musicsource.Play();

            battlemusicsource.volume -= musictransitionspeed * Time.deltaTime;
            musicsource.volume += musictransitionspeed * Time.deltaTime;
        }

        battlemusicsource.volume = Mathf.Clamp(battlemusicsource.volume, 0, gMusicVolume);
        musicsource.volume = Mathf.Clamp(musicsource.volume, 0, gMusicVolume);
    }

    public static void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public static void PlayBattleTrack(AudioClip track)
    {
        battlemusicactive = true;
        instance.battlemusicsource.clip = track;
        instance.battlemusicsource.Stop(); 
        instance.battlemusicsource.time = 0.0f;
        instance.battlemusicsource.Play();
    }

    public static void ResumeNormalTrack()
    {
        battlemusicactive = false;
    }

    public static void WinGame()
    {
        wongame = true;
    }

    public static void LoseGame()
    {
        if(!wongame)
        {

        }
    }

    public static AudioSource Play2D(AudioClipXT clip)
    {
        AudioSource source = instance.CreateInternal(clip);
        source.Play();

        return source;
    }

    public static AudioSource Play3D(AudioClipXT clip, Vector3 pos)
    {
        AudioSource source = instance.CreateInternal(clip);
        source.transform.position = pos;
        source.spatialBlend = 1.0f;
        source.minDistance = kDefault3DMaxVolumeDistance;
        source.maxDistance = kDefault3DMinVolumeDistance;
        source.Play();

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
