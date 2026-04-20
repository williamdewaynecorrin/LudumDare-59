using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(ExecutionOrders.PLAYER_CAMERA)]
public class PlayerCamera : MonoBehaviour
{
    public PlayerController player;
    public Vector3 initialoffset = new Vector3(0f, 1.0f, 0.5f);
    public float sensitivity = 1.0f;
    public Camera[] cameras;

    private Vector3 targetpos = Vector3.zero;
    private Quaternion targetrot = Quaternion.identity;
    private float currentpitch = 0.0f;
    private float currentyaw = 0.0f;

    public Camera Camera => cameras[0];

    void Awake()
    {
        CalculateTarget();
        UpdateTarget();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CalculateTarget();

        // -- update look
        Vector2 mouseinput = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        currentpitch -= mouseinput.x * sensitivity;
        currentpitch = Mathf.Clamp(currentpitch, -89f, 89f);
        currentyaw += mouseinput.y * sensitivity;
    }

    void LateUpdate()
    {
        UpdateTarget();
    }

    public void DisableAll()
    {
        foreach (Camera cam in cameras)
        {
            cam.enabled = false;
            if(cam.TryGetComponent<AudioListener>(out AudioListener listen))
            {
                listen.enabled = false;
            }
        }
    }

    public void SetFOV(float fov)
    {
        foreach(Camera cam in cameras)
        {
            cam.fieldOfView = fov;
        }
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
        return transform.forward.NoY().normalized;
    }

    public Vector3 StrafeMovement()
    {
        return transform.right.NoY().normalized;
    }

}
