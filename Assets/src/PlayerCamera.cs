using UnityEngine;

[DefaultExecutionOrder(ExecutionOrders.PLAYER_CAMERA)]
public class PlayerCamera : MonoBehaviour
{
    public PlayerController player;
    public Vector3 initialoffset = new Vector3(0f, 1.0f, 0.5f);
    public float sensitivity = 1.0f;

    private Vector3 targetpos = Vector3.zero;
    private Quaternion targetrot = Quaternion.identity;
    private float currentpitch = 0.0f;
    private float currentyaw = 0.0f;

    void Awake()
    {
        CalculateTarget();
        UpdateTarget();
    }

    void Update()
    {
        CalculateTarget();

        // -- update look
        Vector2 mouseinput = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        currentpitch -= mouseinput.x * sensitivity;
        currentyaw += mouseinput.y * sensitivity;
    }

    void LateUpdate()
    {
        UpdateTarget();
    }

    private void CalculateTarget()
    {
        // -- position offset is only rotated by YAW
        Quaternion yaw = Quaternion.Euler(0f, currentyaw, 0f);
        Vector3 rotatedoffset = yaw * initialoffset;
        targetpos = player.GetTargetPosition() + rotatedoffset;

        targetrot = Quaternion.Euler(currentpitch, currentyaw, 0f);
    }

    private void UpdateTarget()
    {
        transform.localPosition = targetpos;
        transform.localRotation = targetrot;
    }

    public Vector3 ForwardMovement()
    {
        return transform.forward.NoY();
    }

    public Vector3 StrafeMovement()
    {
        return transform.right.NoY();
    }

}
