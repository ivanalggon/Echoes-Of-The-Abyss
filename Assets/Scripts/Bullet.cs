using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 1; // Daño predeterminado de la bala

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Si golpea a un enemigo
        {
            NPCMovement enemy = other.GetComponent<NPCMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Aplica daño al enemigo
            }
            Destroy(gameObject); // Destruye la bala después del impacto
        }

    }
}