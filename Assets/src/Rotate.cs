using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotation;
    public bool userandomrotation = false;
    public MinMaxFloat randomrotationamt;

    private float rotationscale = 1.0f;

    void Awake()
    {
        if (userandomrotation)
        {
            rotation = Random.onUnitSphere;
            rotationscale = randomrotationamt.PickValue();
        }
    }

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(rotation * Time.deltaTime * rotationscale);
    }
}
