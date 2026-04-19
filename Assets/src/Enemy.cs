using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;
    public CharacterController character;
    public float damageamount = 25f;
    public float mingroundedcastdist = 0.2f;
    public Transform graphicsroot;
    public float speed = 1.0f;
    public float gravity = 0.1f;
    public LayerMask groundmask;
    public LayerMask playermask;
    public SphereCollider sphere;
    public Animator anim;
    public string chaseanim = "Chase";
    public string deathanim = "Death";
    public string spawnanim = "Spawn";
    public AudioSource selfsource;
    [Range(0f, 1f)]
    public float rotationlerp = 0.2f;

    public AudioClipXT sfxspawn;
    public AudioClipXT sfxdeathstart;
    public AudioClipXT sfxdeathend;

    private bool grounded = false;
    private Vector3 currentgravity;
    private PlayerController target;
    private EnemyEncounter spawner;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (health.IsDead || target == null)
            return;

        // -- movement
        Vector3 toplayer = target.transform.position - transform.position;
        toplayer.Normalize();
        Vector3 movement = toplayer * speed;

        if (anim.AnimatorIsInState(spawnanim))
        {
            LookAtTarget(toplayer);
            return;
        }

        // -- gravity + grounding
        bool wasgrounded = grounded;
        grounded = false;

        float castdistance = Mathf.Max(mingroundedcastdist, currentgravity.magnitude);
        if (currentgravity.y <= 0f && Physics.SphereCast(transform.position + sphere.center, sphere.radius, Vector3.down, out RaycastHit groundhit, castdistance, groundmask, QueryTriggerInteraction.Ignore))
        {
            if (!wasgrounded)
                OnGroundLand(groundhit);

            character.Move(Vector3.down * groundhit.distance);
            grounded = true;
            currentgravity = Vector3.zero;
        }
        else
        {
            if (wasgrounded)
                OnGroundLeave();

            currentgravity += Vector3.down * gravity;
        }

        // -- hitting player
        if (Physics.SphereCast(transform.position + sphere.center, sphere.radius, toplayer, out RaycastHit playerhit, speed, playermask, QueryTriggerInteraction.Ignore))
        {
            target.health.DealDamage(damageamount);
        }

        CollisionFlags moveflags = character.Move(movement);
        CollisionFlags gravityflags = character.Move(currentgravity);

        LookAtTarget(toplayer);
    }

    private void LookAtTarget(Vector3 toplayer)
    {
        // -- rotate model towards last look dir
        graphicsroot.localRotation = Quaternion.Slerp(graphicsroot.localRotation, Quaternion.LookRotation(toplayer, Vector3.up), rotationlerp);
    }

    private IEnumerator SpawnRoutine()
    {
        anim.PlayAnimationState(spawnanim);
        target = GameManager.Player;
        yield return new WaitForSeconds(2.0f);
    }

    public void OnDeath()
    {
        anim.PlayAnimationState(deathanim);
        GameManager.Play3D(sfxdeathstart, transform.position);

        if (spawner != null)
            spawner.EnemyDeath(this);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        float frames = 30f;
        float voldec = selfsource.volume / frames;
        float pitchinc = 0.1f;
        for (int i = 0; i < frames; ++i)
        {
            selfsource.pitch += pitchinc;
            selfsource.volume -= voldec;
            yield return new WaitForFixedUpdate();
        }

        selfsource.volume = 0f;

        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(this.gameObject);
        yield break;
    }

    public void SetSpawner(EnemyEncounter encounter)
    {
        spawner = encounter;
    }

    private void OnGroundLand(RaycastHit hit)
    {
        //Teleport(hit.point + hit.normal * mingroundedcastdist * 0.5f);
        //jumped = false;
        //GameManager.Play3D(sfxland, transform.position);
    }

    private void OnGroundLeave()
    {

    }

    public void Teleport(Vector3 location)
    {
        character.enabled = false;
        transform.position = location;
        character.enabled = true;
    }
}
