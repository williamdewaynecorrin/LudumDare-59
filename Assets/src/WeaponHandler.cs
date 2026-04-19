using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandler : MonoBehaviour
{
    [Header("Base")]
    public Transform rootbase;
    public Transform hipfirebase;
    public Transform adsbase;
    [Range(0f, 1f)]
    public float adsspeed = 0.2f;
    [Range(0f, 1f)]
    public float lowerspeed = 0.3f;

    [Header("Animation")]
    public Animator fpsarmsanim;
    public string reloadanim = "Reload";
    public string fireanim = "Fire";
    public float reticlepower = 10.0f;
    public ParticleSystem muzzleflash;
    public float adsmuzzlescale = 0.5f;
    public float hipfiremuzzlescale = 1.0f;

    [Header("Firing")]
    public ObjectPooler bulletpooler;
    public Transform muzzle;
    [Range(0f, 1f)]
    public float adsaccuracy = 0.1f;
    [Range(0f, 1f)]
    public float hipfireaccuracy = 0.5f;
    public int clipsize = 32;
    public float bulletspeed = 1.0f;
    public float bulletgravity = 0.1f;
    public float damage = 15.0f;
    public CTimer firetime;
    public AudioClipXT sfxfire;

    [Header("UI")]
    public UIReticle uireticle;
    public Color uiammofillmaxcolor;
    public Color uiammofillmincolor;
    public Image uiammofill;
    public TMP_Text uiammoamt;

    private bool isreloading = false;
    private int currentclip;
    private bool isads = false;

    void Awake()
    {
        currentclip = clipsize;
        firetime.Init();
        firetime.time = 0.0f;
        muzzleflash.Stop();
    }

    void Update()
    {
        if(!isreloading)
        {
            if(firetime.time > 0.0f)
                firetime.Tick(Time.deltaTime);

            if (Input.GetMouseButton(0) && CanFire())
            {
                if (firetime.TimerReached())
                {
                    Fire();
                    firetime.Reset();
                }
            }
            else if(Input.GetMouseButtonDown(0))
            {
                TryReload();
            }

            if (Input.GetMouseButton(1))
            {
                if (!isads)
                    uireticle.Hide();

                isads = true;
            }
            else
            {
                if (isads)
                    uireticle.Show();

                isads = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }
        }
    }

    void FixedUpdate()
    {
        if (isads)
        {
            rootbase.transform.localPosition = Vector3.Lerp(rootbase.transform.localPosition, adsbase.localPosition, adsspeed);
        }
        else
        {
            rootbase.transform.localPosition = Vector3.Lerp(rootbase.transform.localPosition, hipfirebase.localPosition, lowerspeed);
        }
    }

    private bool CanFire()
    {
        return currentclip > 0;
    }

    private void TryReload()
    {
        if (!fpsarmsanim.AnimatorIsInState(reloadanim))
        {
            isreloading = true;
            isads = false;
            fpsarmsanim.PlayAnimationState(reloadanim);
        }
    }

    public void Fire()
    {
        --currentclip;

        Bullet bullet = bulletpooler.Handle() as Bullet;
        bullet.transform.position = muzzle.position;
        bullet.transform.rotation = muzzle.rotation;

        SBulletParams bparams = new SBulletParams(CalculateBulletDirection(), GameManager.kPlayerTeam, bulletspeed, bulletgravity, damage);
        bullet.CreateBullet(bparams);

        fpsarmsanim.PlayAnimationState(fireanim, resetifsame: true);

        uireticle.AddRecoil(reticlepower);
        UpdateWeaponUI();

        muzzleflash.transform.localScale = (isads ? adsmuzzlescale : hipfiremuzzlescale) * Vector3.one;
        muzzleflash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleflash.Play(true);
        GameManager.Play2D(sfxfire);
    }

    private Vector3 CalculateBulletDirection()
    {
        Vector3 forward = muzzle.forward;

        float accuracy = isads ? adsaccuracy : hipfireaccuracy;
        Vector3 offset = muzzle.up * Random.Range(-1f, 1f) + muzzle.right * Random.Range(-1f, 1f);
        offset = offset.normalized * accuracy;

        return (forward + offset).normalized;
    }

    public void OnReloadFinished()
    {
        isreloading = false;
        currentclip = clipsize;
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        float fillratio = (float)currentclip / (float)clipsize;

        // -- use HSV instead i guess (its gray at 0.5 with rgb)
        Color.RGBToHSV(uiammofillmincolor, out float hmin, out float smin, out float vmin);
        Color.RGBToHSV(uiammofillmaxcolor, out float hmax, out float smax, out float vmax);
        float hval = Mathf.Lerp(hmin, hmax, fillratio);
        Color lerpcolor = Color.HSVToRGB(hval, smax, vmax);

        uiammoamt.text = currentclip.ToString();
        uiammoamt.color = lerpcolor;

        uiammofill.fillAmount = fillratio;
        uiammofill.color = lerpcolor;
    }
}
