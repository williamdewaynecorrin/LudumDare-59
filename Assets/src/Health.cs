using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public Image uifill;
    public GameObject uifillroot;
    public UnityEvent ondeath;

    private float maxhealth;
    private bool isdead = false;
    public bool IsDead => isdead;

    void Awake()
    {
        maxhealth = health;
        isdead = false;

        if (uifill != null)
        {
            uifill.fillAmount = 1.0f;

            if(uifillroot != null)
                uifillroot.SetActive(false);
        }
    }

    public void DealDamage(float damage)
    {
        if (isdead)
            return;

        health -= damage;

        if(uifill != null)
        {
            if (uifillroot != null)
                uifillroot.SetActive(true);

            uifill.fillAmount = (float)health / (float)maxhealth;
        }

        if(health <= 0.0f)
        {
            isdead = true;
            ondeath?.Invoke();
        }
    }

    public void OnHit(SBulletParams parms, Vector3 hitpoint, Vector3 hitnormal)
    {
        DealDamage(parms.damage);

        if (parms.teamid == GameManager.kPlayerTeam)
            GameManager.Player.handler.uireticle.SetHitMarker();
    }
}
