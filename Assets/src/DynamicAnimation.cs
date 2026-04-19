using UnityEngine;

public class DynamicAnimation : MonoBehaviour
{
    [Header("Base")]
    public Transform basetransform;
    [Range(0f, 1f)]
    public float lerppos = 0.3f;

    [Header("Idle")]
    public float idleamp = 0.2f;
    public float idleperiod = 1.0f;

    [Header("Walk")]
    public float walkamp = 0.5f;
    public float walkperiod = 2.0f;
    public float adsmult = 0.2f;

    [Header("Sprint")]
    public float sprintamp = 0.65f;
    public float sprintperiod = 4.0f;
    public Vector3 sprintoffset;
    public Vector3 sprintrot;

    private Vector3 defaultpos;
    private Vector3 targetoffset;
    private Quaternion defaultrot;
    private Quaternion targetoffsetrot;
    private float animtime = 0.0f;
    private EDynamicAnimState state = EDynamicAnimState.eIdle;

    void Awake()
    {
        defaultpos = basetransform.localPosition;
        defaultrot = basetransform.localRotation;
    }

    void Update()
    {
        animtime += Time.deltaTime;
        Vector3 offsetpos = Vector3.zero;
        Quaternion offsetrot = Quaternion.identity;
        switch(state )
        {
            case EDynamicAnimState.eIdle:
                offsetpos.y = Mathf.Sin(animtime * idleperiod) * idleamp;
                break;
            case EDynamicAnimState.eWalk:
                offsetpos.x = Mathf.Cos(animtime * walkperiod ) * walkamp;
                offsetpos.y = Mathf.Sin(animtime * walkperiod * 2.0f) * walkamp;
                break;
            case EDynamicAnimState.eSprint:
                offsetpos.x = Mathf.Cos(animtime * sprintperiod * 2.0f) * sprintamp;
                offsetpos.y = Mathf.Sin(animtime * sprintperiod) * sprintamp;

                offsetpos += sprintoffset;
                offsetrot = Quaternion.Euler(sprintrot);
                break;
        }

        targetoffset = offsetpos;
        targetoffsetrot = offsetrot;
    }

    void FixedUpdate()
    {
        basetransform.localPosition = Vector3.Lerp(basetransform.localPosition, targetoffset, lerppos);
        basetransform.localRotation = Quaternion.Slerp(basetransform.localRotation, targetoffsetrot, lerppos);
    }

    public void SetState(EDynamicAnimState state)
    {
        this.state = state;
    }
}

public enum EDynamicAnimState
{
    eIdle = 0,
    eWalk = 1,
    eSprint = 2
}