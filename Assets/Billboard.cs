using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool yawonly = false;
    public bool hasscaling = false;
    public MinMaxFloat scalingdistances;
    public MinMaxFloat scales;

    private Vector3 initialscale;

    void Awake()
    {
        initialscale = transform.localScale;
    }

    void Update()
    {
        if(yawonly)
        {
            Vector3 euler = GameManager.Player.camera.transform.rotation.eulerAngles;
            euler.x = transform.rotation.eulerAngles.x;
            euler.z = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(euler);
        }
        else
        {
            transform.rotation = GameManager.Player.camera.transform.rotation;
        }

        if(hasscaling)
        {
            Vector3 totarget = transform.position - GameManager.Player.camera.transform.position;
            float distance = totarget.magnitude;

            float distval = Mathf.Clamp01(Mathf.InverseLerp(scalingdistances.min, scalingdistances.max, distance));
            float scale = Mathf.Lerp(scales.max, scales.min, distval);
            transform.localScale = initialscale * scale;
        }
    }
}
