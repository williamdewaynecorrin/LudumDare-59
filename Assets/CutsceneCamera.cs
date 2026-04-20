using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneCamera : MonoBehaviour
{
    public Camera cam;
    public AudioListener listener;
    public Animator anim;
    public Transform wintarget;
    public Transform losetarget;
    public float targetfov = 75f;
    [Range(0f, 1f)]
    public float cutscenelerp = 0.1f;

    private Transform target;

    private bool alreadyplayed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam.enabled = false;
        listener.enabled = false;
        anim.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(cam.enabled)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, cutscenelerp);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, cutscenelerp);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetfov, cutscenelerp);
        }
    }

    public void PlayWin()
    {
        if (alreadyplayed)
            return;

        transform.position = GameManager.Player.camera.transform.position;
        transform.rotation = GameManager.Player.camera.transform.rotation;
        cam.enabled = true;
        listener.enabled = true;
        cam.fieldOfView = GameManager.Player.camera.Camera.fieldOfView;
        target = wintarget;
        anim.enabled = true;
        alreadyplayed = true;

        StartCoroutine(OnWinRoutine());
    }

    public void PlayLoss()
    {
        if (alreadyplayed)
            return;

        transform.position = GameManager.Player.camera.transform.position;
        transform.rotation = GameManager.Player.camera.transform.rotation;
        cam.enabled = true;
        listener.enabled = true;
        cam.fieldOfView = GameManager.Player.camera.Camera.fieldOfView;
        target = losetarget;
        alreadyplayed = true;

        StartCoroutine(OnLoseRoutine());
    }

    private IEnumerator OnWinRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        GameManager.VignetteClose();
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(GameManager.kMenuLevel);
    }

    private IEnumerator OnLoseRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.VignetteClose();
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(GameManager.kMenuLevel);
    }
}
