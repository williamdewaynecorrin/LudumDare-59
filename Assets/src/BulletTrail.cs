using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public TrailRenderer trail;

    private Vector3 frametravel;
    private bool on = false;

    public void TurnOn(Vector3 travel)
    {
        on = true;
     
        frametravel = travel;
        trail.emitting = false;
    }

    public void TurnOff()
    {
        on = false;

        trail.Clear();
        trail.emitting = true;
    }

    void FixedUpdate()
    {
        if (on)
            transform.position += frametravel;
    }
}
