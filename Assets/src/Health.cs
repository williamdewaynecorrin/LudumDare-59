using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Range(GameManager.kPlayerTeam, GameManager.kEnemyTeam)]
    public uint teamid = GameManager.kEnemyTeam;
    public float health = 100f;
    public Image uifill;
    public GameObject uifillroot;
    public UnityEvent<Vector3, Vector3> ondamaged;
    public UnityEvent ondeath;
    public bool hasrecoverytime = false;
    public float recoverytime = 2.0f;

    private float maxhealth;
    private bool isdead = false;
    private float cachedhittime;

    public float MaxHealth => maxhealth;
    public bool IsDead => isdead;

    void Awake()
    {
        cachedhittime = -recoverytime;
        maxhealth = health;
        isdead = false;

        if (uifill != null)
        {
            uifill.fillAmount = 1.0f;

            if(uifillroot != null)
                uifillroot.SetActive(false);
        }
    }

    public void DealDamage(float damage, Vector3 hitpoint, Vector3 hitnormal)
    {
        if (isdead)
            return;
        if (hasrecoverytime && cachedhittime + recoverytime > Time.time)
            return;

        cachedhittime = Time.time;
        health -= damage;
        UpdateUI();

        if(health <= 0.0f)
        {
            isdead = true;
            ondeath?.Invoke();
        }
        else
        {
            ondamaged?.Invoke(hitpoint, hitnormal);
        }
    }

    public void Heal(float amt)
    {
        health += amt;
        health = Mathf.Clamp(health, 0f, maxhealth);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (uifill != null)
        {
            if (uifillroot != null)
                uifillroot.SetActive(true);

            uifill.fillAmount = (float)health / (float)maxhealth;
        }
    }

    public void OnHit(SBulletParams parms, Vector3 hitpoint, Vector3 hitnormal)
    {
        DealDamage(parms.damage, hitpoint, hitnormal);

        if (parms.teamid == GameManager.kPlayerTeam)
            GameManager.Player.handler.uireticle.SetHitMarker();
    }
}
