using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(ExecutionOrders.GAME_MANAGER)]
public class GameManager : MonoBehaviour
{
    // -- singleton
    public const uint kPlayerTeam = 0;
    public const uint kEnemyTeam = 1;
    public const string kMenuLevel = "level_menu";
    public const string kGameLevel = "level_game";

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
    [Header("Base Data")]
    public float musictransitionspeed = 2.0f;
    public AudioClip levelmusic;
    public AudioSource musicsource;
    public AudioSource battlemusicsource;
    public AudioSource sfxsource;
    public ObjectPooler textpooler; 
    public ObjectPooler enemybulletpooler;
    public CutsceneCamera cutscenecam;

    [Header("Post Process")]
    public PostProcessProfile postprocess;
    [Range(0f, 1f)]
    public float vignetteonvalue;
    [Range(0f, 1f)]
    public float vignetteoffvalue;
    [Range(0f, 1f)]
    public float vignettelerp = 0.1f;
    public float targetgrain;

    private Vignette vignette;
    private Grain grain;
    private ChromaticAberration chromatic;
    private bool vignetteon = false;

    private static bool wongame = false;

    void Awake()
    {
        instance = this;
        OnFirstInitialize();
        return;

        //// -- use new level's music
        //instance.musicsource.Stop();
        //instance.musicsource.clip = levelmusic;
        //instance.musicsource.time = 0.0f;
        //instance.musicsource.Play();

        //// -- destroy new instance
        //GameObject.Destroy(this.gameObject);
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

        postprocess.TryGetSettings<Vignette>(out vignette);
        vignette.opacity.value = vignetteonvalue;

        postprocess.TryGetSettings<Grain>(out grain);
        grain.intensity.value = 0f;

        postprocess.TryGetSettings<ChromaticAberration>(out chromatic);
        chromatic.intensity.value = 0.0f;
    }

    void FixedUpdate()
    {
        if(vignetteon)
        {
            vignette.opacity.value = Mathf.Lerp(vignette.opacity.value, vignetteonvalue, vignettelerp);
        }
        else
        {
            vignette.opacity.value = Mathf.Lerp(vignette.opacity.value, vignetteoffvalue, vignettelerp);
        }

        float playerhealthratio = Mathf.Clamp01(1.0f - (player.health.health / player.health.MaxHealth)) * 0.8f;
        chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, playerhealthratio, vignettelerp * 2.0f);
        grain.intensity.value = Mathf.Lerp(grain.intensity.value, targetgrain, vignettelerp);
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
        player.SetFrozen(true);
        player.camera.DisableAll();
        instance.cutscenecam.PlayWin();
    }

    public static void LoseGame()
    {
        if(!wongame)
        {
            player.camera.Camera.enabled = false;
            instance.cutscenecam.PlayLoss();
        }
    }

    public static void VignetteOpen()
    {
        instance.vignetteon = false;
    }

    public static void VignetteClose()
    {
        instance.vignetteon = true;
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
