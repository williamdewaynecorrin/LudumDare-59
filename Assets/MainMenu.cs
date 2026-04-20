using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Base Data")]
    public GameObject mainobject;
    public GameObject settingsobject;
    public AudioClipXT sfxclick;
    public AudioSource sfx;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        postprocess.TryGetSettings<Vignette>(out vignette);
        vignette.opacity.value = vignetteonvalue;

        postprocess.TryGetSettings<Grain>(out grain);
        postprocess.TryGetSettings<ChromaticAberration>(out chromatic);

        chromatic.intensity.value = 0.0f;
    }

    void FixedUpdate()
    {
        vignette.opacity.value = Mathf.Lerp(vignette.opacity.value, vignetteoffvalue, vignettelerp);
        grain.intensity.value = Mathf.Lerp(grain.intensity.value, targetgrain, vignettelerp);
    }

    public void StartGame()
    {
        PlayClick();
        SceneManager.LoadScene(GameManager.kGameLevel);
    }

    public void ShowSettings()
    {
        PlayClick();
        mainobject.SetActive(false);
        settingsobject.SetActive(true);
    }

    public void HideSettings()
    {
        PlayClick();
        mainobject.SetActive(true);
        settingsobject.SetActive(false);
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
    }

    private void PlayClick()
    {
        sfx.clip = sfxclick.clip;
        sfx.volume = sfxclick.volumemult;
        sfx.loop = false;
        sfx.Stop();
        sfx.Play();
    }
}
