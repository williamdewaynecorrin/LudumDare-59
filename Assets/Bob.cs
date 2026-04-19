using UnityEngine;

public class Bob : MonoBehaviour
{
    public bool localspace = true;
    public float amp = 1.0f;
    public float period = 2.0f;
    public bool flipaxis = false;

    private Vector3 storedlocal;

    void Awake()
    {
        storedlocal = transform.localPosition;
    }

    void Update()
    {
        float sin = Mathf.Sin(Time.time * period) * amp;
        Vector3 bob = new Vector3(0f, sin, 0f);
        if(flipaxis)
            bob = new Vector3(0f, 0f, sin);

        if (localspace)
        {
            transform.localPosition = storedlocal + bob;
        }
        else
        {
            transform.position += bob;
        }
    }
}
