using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public Enemy parent;
    public void FireBullet()
    {
        parent.ShootAtTarget();
    }
}
