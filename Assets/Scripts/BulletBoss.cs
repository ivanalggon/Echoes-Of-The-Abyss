using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    private int damage = 10;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true; // No responde con física
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // si colisiona con un objeto que no es enemigo o jefe, la bala se destruye
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destruye la bala
        }
    }
}
