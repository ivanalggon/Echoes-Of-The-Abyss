using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject ak74Suelo;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponInventory inventory = other.GetComponent<WeaponInventory>();

            if (inventory != null)
            {
                inventory.PickupWeapon();
                ak74Suelo.SetActive(false);
            }
        }
    }
}
