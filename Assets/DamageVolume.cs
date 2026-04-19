using UnityEngine;

public class DamageVolume : MonoBehaviour
{
    public float damage = 25f;
    [Range(GameManager.kPlayerTeam, GameManager.kEnemyTeam)]
    public uint teamid = GameManager.kEnemyTeam;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Health>(out Health health))
        {
            if(health.teamid != teamid)
            {
                health.DealDamage(damage, transform.position, (other.transform.position - transform.position).normalized);
            }
        }
    }
}
