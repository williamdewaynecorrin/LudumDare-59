using UnityEngine;
using UnityEngine.UI;

public class UIReticle : MonoBehaviour
{
    public Transform up, right, down, left;
    [Range(0f, 1f)]
    public float recoverspeed = 0.1f;
    public Image hitmarker;
    public CTimer alphatimer;
    public float alphafade = 2.0f;

    private Vector3 storedup, storedright, storeddown, storedleft;

    private float startingalpha;

    void Awake()
    {
        storedup = up.localPosition;
        storedright = right.localPosition;
        storeddown = down.localPosition;
        storedleft = left.localPosition;

        alphatimer.Init();
        alphatimer.time = 0f;

        startingalpha = hitmarker.color.a;
        hitmarker.color=hitmarker.color.SetAlpha(0f);
    }

    private void Update()
    {
        if(hitmarker.color.a > 0f)
        {
            alphatimer.Tick(Time.deltaTime);
            if(alphatimer.TimerReached())
            {
                float newalpha = hitmarker.color.a - Time.deltaTime * alphafade;
                newalpha = Mathf.Clamp01(newalpha);
                hitmarker.color=hitmarker.color.SetAlpha(newalpha);
            }
        }
    }

    private void FixedUpdate()
    {
        up.localPosition = Vector3.Lerp(up.localPosition, storedup, recoverspeed);
        right.localPosition = Vector3.Lerp(right.localPosition, storedright, recoverspeed);
        down.localPosition = Vector3.Lerp(down.localPosition, storeddown, recoverspeed);
        left.localPosition = Vector3.Lerp(left.localPosition, storedleft, recoverspeed);
    }

    public void AddRecoil(float power)
    {
        up.localPosition += up.up * power;
        right.localPosition += right.up * power;
        down.localPosition += down.up * power;
        left.localPosition += left.up * power;
    }

    public void SetHitMarker()
    {
        hitmarker.color=hitmarker.color.SetAlpha(startingalpha);
        alphatimer.Reset();
    }

    public void Show()
    {
        up.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        down.gameObject.SetActive(true);
        left.gameObject.SetActive(true);
    }

    public void Hide()
    {
        up.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        down.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
    }
}
