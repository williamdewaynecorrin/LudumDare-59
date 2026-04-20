using UnityEngine;

[DefaultExecutionOrder(ExecutionOrders.PLAYER_CONTROLLER)]
public class PlayerController : MonoBehaviour
{
    // -- inspectable members
    [Header("Base Data")]
    public new PlayerCamera camera;
    public WeaponHandler handler;
    public float acceleration = 0.1f;
    public float maxspeed = 0.5f;
    public float maxsprintspeed = 0.5f;
    [Range(0f, 1f)]
    public float stopfriction = 0.9f;
    public Health health;
    public AudioClipXT sfxhurt;
    public AudioClipXT sfxdeath;
    public UIItems uiitems;

    [Header("Gravity/Jumping")]
    public float gravitystrength = 0.1f;
    public float jumpstrength = 1.0f;
    public float mingroundedcastdist = 0.1f;
    public CTimer coyotetimer;
    public LayerMask groundmask;
    public float knockbackstrength = 1.0f;
    public float knockbackfriction = 0.9f;

    [Header("Character Graphics - Third Person")]
    public Transform graphicsroot;
    [Range(0f, 1f)]
    public float rotationlerp = 0.2f;
    public Animator animatorbody;
    public DynamicAnimation dynamicanim;
    public string bodyidle = "Idle";
    public string bodywalk = "Walk";

    [Header("Character Graphics - First Person")]
    public Animator animatorhands;
    public string handsidle = "Idle";

    [Header("SFX")]
    public AudioClipXT sfxjump;
    public AudioClipXT sfxland;
    public AudioClipXT sfxstep;
    public float traveldistanceforstep = 4.0f;

    // -- private members
    private CharacterController character = null;
    private Vector2 movementinput = Vector2.zero;
    private Vector3 currentmovement = Vector3.zero;
    private Vector3 currentgravity = Vector3.zero;
    private bool grounded = false;
    private GameObject lastgroundobject = null;
    private bool coyotetimeavailable = false;
    private bool jumped = false;
    private float distancetravelled;
    private bool issprinting = false;
    private Vector3 currentknockback = Vector3.zero;
    private bool frozen = false;

    public bool CanJump => !jumped && (grounded || coyotetimeavailable);
    public bool CanSprint => grounded;
    public bool IsSprinting => issprinting;

    void Awake()
    {
        character = GetComponent<CharacterController>();
        coyotetimer.Init();

        animatorbody.PlayAnimationState(bodyidle);
        animatorhands.PlayAnimationState(handsidle);
    }

    void Update()
    {
        if (frozen)
            return;

        // -- movement
        float xin = Input.GetKey(KeyCode.A) ? -1.0f : 0.0f;
        xin += Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;
        float yin = Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        yin += Input.GetKey(KeyCode.S) ? -1.0f : 0.0f;

        movementinput = new Vector2(xin, yin);
        bool sprintinput = Input.GetKey(KeyCode.LeftShift);

        if (!sprintinput)
            issprinting = false;

        if (movementinput != Vector2.zero)
        {
            if (!animatorbody.AnimatorIsInState(bodywalk))
                animatorbody.PlayAnimationState(bodywalk);

            if ((sprintinput && CanSprint) || issprinting)
            {
                dynamicanim.SetState(EDynamicAnimState.eSprint);
            }
            else
            {
                dynamicanim.SetState(EDynamicAnimState.eWalk);
            }
        }
        else
        {
            if (!animatorbody.AnimatorIsInState(bodyidle))
            {
                dynamicanim.SetState(EDynamicAnimState.eIdle);
                animatorbody.PlayAnimationState(bodyidle);
            }
        }

        // -- jumping
        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            Jump();
        }

        // -- coyote time / grounded
        if (grounded)
        {
            coyotetimer.Reset();
            coyotetimeavailable = true;

            if (!issprinting && sprintinput && CanSprint)
            {
                issprinting = true;
            }
        }
        else
        {
            if(coyotetimer.TimerReached())
            {
                coyotetimeavailable = false;
            }
            else
            {
                coyotetimer.Tick(Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (frozen)
            return;

        // -- movement
        Vector3 forward = camera.ForwardMovement();
        Vector3 strafe = camera.StrafeMovement();

        if (movementinput != Vector2.zero)
        {
            Vector3 framemovement = forward * acceleration * movementinput.y + strafe * acceleration * movementinput.x;
            currentmovement += framemovement;

            float clamp = issprinting ? maxsprintspeed : maxspeed;
            currentmovement = Vector3.ClampMagnitude(currentmovement, clamp);
        }
        else
        {
            currentmovement *= stopfriction;
        }

        // -- gravity + grounding
        bool wasgrounded = grounded;
        grounded = false;
        GetGroundedData(out Vector3 p1, out Vector3 p2, out Vector3 gravitydir, out float castdistance);

        if (currentgravity.y <= 0f && Physics.CapsuleCast(p1, p2, character.radius, gravitydir, out RaycastHit groundhit, castdistance, groundmask, QueryTriggerInteraction.Ignore))
        {
            if (!wasgrounded)
                OnGroundLand(groundhit);

            character.Move(Vector3.down * groundhit.distance);
            grounded = true;
            lastgroundobject = groundhit.collider.gameObject;
            currentgravity = Vector3.zero;
        }
        else
        {
            if (wasgrounded)
                OnGroundLeave();

            currentgravity += gravitydir * gravitystrength;
        }

        Vector2 xzpos = new Vector2(transform.position.x, transform.position.z);
        CollisionFlags moveflags = character.Move(currentmovement);
        CollisionFlags gravityflags = character.Move(currentgravity);

        if(currentknockback != Vector3.zero)
        {
            currentknockback *= knockbackfriction;
            moveflags = character.Move(currentknockback);
        }

        // -- footstep detection
        if (grounded)
        {
            Vector2 newxzpos = new Vector2(transform.position.x, transform.position.z);
            float change = (newxzpos - xzpos).magnitude;
            distancetravelled += change;

            if (distancetravelled >= traveldistanceforstep)
            {
                distancetravelled -= traveldistanceforstep;
                GameManager.Play2D(sfxstep);
            }
        }

        // -- rotate model towards last look dir
        graphicsroot.localRotation = Quaternion.Slerp(graphicsroot.localRotation, Quaternion.LookRotation(forward, Vector3.up), rotationlerp);
    }

    public void Die()
    {
        GameManager.Play2D(sfxdeath);
        GameManager.LoseGame();
        SetFrozen(true);

        dynamicanim.SetState(EDynamicAnimState.eIdle);
        animatorbody.PlayAnimationState(bodyidle);
    }

    public void SetFrozen(bool freeze)
    {
        frozen = freeze;
    }

    private void OnGroundLand(RaycastHit hit)
    {
        //Teleport(hit.point + hit.normal * mingroundedcastdist * 0.5f);
        jumped = false;
        GameManager.Play3D(sfxland, transform.position);
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

    public void Jump()
    {
        currentgravity = -GetGravityDir() * jumpstrength;
        jumped = true;
        GameManager.Play3D(sfxjump, transform.position);
    }

    public void OnDamaged(Vector3 hitpoint, Vector3 normal)
    {
        Vector3 awaydamage = (transform.position - hitpoint).NoY().normalized;
        currentknockback = (Vector3.up * 1.5f + awaydamage * 1f).normalized;
        currentknockback *= knockbackstrength;

        GameManager.Play2D(sfxhurt);
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position;
    }

    public Vector3 GetChestTarget()
    {
        return transform.position + character.center + Vector3.up * character.height * 0.3f;
    }

    public Quaternion GetYAW()
    {
        return transform.rotation;
    }

    private void GetGroundedData(out Vector3 p1, out Vector3 p2, out Vector3 gravitydir, out float castdistance)
    {
        Vector3 radiusoffset = Vector3.up * character.radius;
        p1 = transform.position + character.center + Vector3.up * character.height * -0.5f + radiusoffset;
        p2 = transform.position + character.center + Vector3.up * character.height * 0.5f - radiusoffset;

        gravitydir = GetGravityDir();
        castdistance = Mathf.Max(mingroundedcastdist, currentgravity.magnitude);
    }

    private Vector3 GetGravityDir()
    {
        Vector3 gravitydir = Vector3.down;
        return gravitydir;
    }

    public void ItemPickedUp(Item item)
    {
        uiitems.ObtainItem(item);
    }

    public void ItemPickedUp(EItemType itemtype)
    {
        uiitems.ObtainItem(itemtype);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<EnemyEncounter>(out EnemyEncounter encounter))
        {
            encounter.SpawnEnemies();
        }
    }

    // -- debug functions
    void OnGUI()
    {
        return;
        if (!Application.isEditor)
            return;

        const float debuglineheight = 25f;
        Rect debugrect = new Rect(5, 5, 250, debuglineheight);

        GUI.Label(debugrect, string.Format("Current Speed -> {0}", currentmovement.magnitude.ToString("F2")));
        debugrect.y += debuglineheight;
        GUI.Label(debugrect, string.Format("Grounded? -> {0}", grounded.ToString()));
        debugrect.y += debuglineheight;
        GUI.Label(debugrect, string.Format("Last Ground Object -> {0}", lastgroundobject != null ? lastgroundobject.name : "N/A"));
        debugrect.y += debuglineheight;
        GUI.Label(debugrect, string.Format("Can Jump? -> {0}", CanJump));
        debugrect.y += debuglineheight;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        GetGroundedData(out Vector3 p1, out Vector3 p2, out Vector3 gravitydir, out float castdistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(p1, character.radius);
        Gizmos.DrawWireSphere(p2, character.radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + gravitydir * castdistance);
    }
}
