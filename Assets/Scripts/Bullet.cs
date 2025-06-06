using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 1;

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
        if (other.CompareTag("Enemy"))
        {
            NPCEstados enemy = other.GetComponent<NPCEstados>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        if (other.CompareTag("Boss"))
        {
            BossNPC boss = other.GetComponent<BossNPC>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        
        else if (!other.CompareTag("Player") && !other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}
