using UnityEngine;

public class Item : MonoBehaviour
{
    public Transform graphics;
    public EItemType type = EItemType.eCrystals;
    public AudioClipXT sfxpickup;

    private bool pickedup = false;

    public void Pickup()
    {
        pickedup = true;
        GameManager.Play3D(sfxpickup, transform.position);
        GameObject.Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (pickedup)
            return;

        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.ItemPickedUp(this);
            Pickup();
        }
    }
}
