using UnityEngine;

public class Bob : MonoBehaviour
{
    public bool localspace = true;
    public float amp = 1.0f;
    public float period = 2.0f;

    private Vector3 storedlocal;

    void Awake()
    {
        storedlocal = transform.localPosition;
    }

    void Update()
    {
        Vector3 bob = new Vector3(0f, Mathf.Sin(Time.time * period) * amp, 0f);
        if(localspace)
        {
            transform.localPosition = storedlocal + bob;
        }
        else
        {
            transform.position += bob;
        }
    }
}
