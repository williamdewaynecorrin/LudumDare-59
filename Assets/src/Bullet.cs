using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : PooledObject
{
    [Header("Base Data")]
    public EBulletMode mode = EBulletMode.eRaycast;
    public AudioClipXT sfxhit;
    public BulletTrail trail;

    [Header("Rigidbody Mode")]
    public Rigidbody body;

    [Header("Raycast Mode")]
    public LayerMask raycastmask;

    private SBulletParams data;
    private Vector3 trailoffsetp;
    private Quaternion trailoffsetr;
    private Vector3 trailscale;

    public float AdjustedSpeed
    {
        get
        {
            switch(mode)
            {
                case EBulletMode.eRigidbody:
                    return data.speed;
                case EBulletMode.eRaycast:
                    return data.speed / 100f;
            }

            Debug.LogErrorFormat("Bullet mode '{0}' invalid", mode.ToString());
            return 0f;
        }
    }

    public float AdjustedGravity
    {
        get
        {
            switch (mode)
            {
                case EBulletMode.eRigidbody:
                    return data.gravity;
                case EBulletMode.eRaycast:
                    return data.gravity / 100f;
            }

            Debug.LogErrorFormat("Bullet mode '{0}' invalid", mode.ToString());
            return 0f;
        }
    }

    void Awake()
    {

    }

    public void CreateBullet(SBulletParams data)
    {
        trail.transform.SetParent(this.transform);
        trail.transform.localPosition = trailoffsetp;
        trail.transform.localRotation = trailoffsetr;
        trail.transform.localScale = trailscale;
        trail.TurnOff();

        this.data = data;

        if(mode == EBulletMode.eRigidbody)
        {
            body.linearVelocity = data.bulletdir * AdjustedSpeed;
            body.useGravity = false;
        }
    }

    public override void OnHandle()
    {
        base.OnHandle();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override void OnRecycled()
    {
        base.OnRecycled();
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        switch(mode)
        {
            case EBulletMode.eRaycast:

                Vector3 totalmove = data.bulletdir * AdjustedSpeed + Vector3.down * AdjustedGravity;
                float movemag = totalmove.magnitude;
                Vector3 movedir = totalmove / movemag;

                if(Physics.Raycast(transform.position, movedir, out RaycastHit hit, movemag + 0.01f, raycastmask, QueryTriggerInteraction.Ignore ))
                {
                    OnCollisionInternal(hit.point, hit.normal, hit.collider.gameObject);
                    return;
                }

                transform.position += data.bulletdir * AdjustedSpeed;
                transform.position += Vector3.down * AdjustedGravity;
                break;
            case EBulletMode.eRigidbody:
                body.MovePosition(body.position + Vector3.down * AdjustedGravity);
                break;
        }
    }

    private void OnCollisionInternal(Vector3 hitpoint, Vector3 hitnormal, GameObject hit)
    {
        if(hit.TryGetComponent<BulletHitReaction>(out BulletHitReaction bhit))
        {
            bhit.OnHit(data, hitpoint, hitnormal);
        }
        else
        {
            GameManager.Play3D(sfxhit, hitpoint);
        }

        trail.transform.SetParent(null);
        trail.TurnOn(data.bulletdir * AdjustedSpeed);
        RecycleSelf();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (mode == EBulletMode.eRaycast)
            return;

        OnCollisionInternal(collision.contacts[0].point, collision.contacts[0].normal, collision.collider.gameObject);
    }
}

public enum EBulletMode
{
    eRigidbody = 0,
    eRaycast = 1
}

public struct SBulletParams
{
    public Vector3 bulletdir;
    public uint teamid;
    public float speed;
    public float gravity;
    public float damage;

    public SBulletParams(Vector3 bulletdir, uint teamid, float speed, float gravity, float damage)
    {
        this.bulletdir = bulletdir;
        this.teamid = teamid;
        this.speed = speed;
        this.gravity = gravity;
        this.damage = damage;
    }
}