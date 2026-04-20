using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healamt = 25.0f;
    [Range(GameManager.kPlayerTeam, GameManager.kEnemyTeam)]
    public uint teamid = GameManager.kPlayerTeam;
    public AudioClipXT sfxpickup;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out Health health))
        {
            if (health.teamid == teamid && health.health < health.MaxHealth)
            {
                health.Heal(healamt);

                UIDynamicText texth = GameManager.TextPooler.Handle() as UIDynamicText;
                texth.CreateText("HEALTH!" , Color.red, new Vector3(Random.Range(-80f, -30f), Random.Range(70f, 10f), 0f), Random.Range(-10f, 10f), Vector3.one);
                texth.DisableRotation();

                GameManager.Play2D(sfxpickup);
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
