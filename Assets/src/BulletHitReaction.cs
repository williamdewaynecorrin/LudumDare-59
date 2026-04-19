using UnityEngine;
using UnityEngine.Events;

public class BulletHitReaction : MonoBehaviour
{
    [SerializeField]
    private AudioClipXT sfxhit;
    [SerializeField]
    private UnityEvent<SBulletParams, Vector3, Vector3> onhit;

    public void OnHit(SBulletParams parms, Vector3 hitpoint, Vector3 hitnormal)
    {
        GameManager.Play3D(sfxhit, hitpoint);
        onhit?.Invoke(parms, hitpoint, hitnormal);
    }
}
