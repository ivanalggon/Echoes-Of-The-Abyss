using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para Image
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject walkingcamera;
    public Animator animator;
    public TextMeshProUGUI AmmoText;

    private Rigidbody _rb;

    // Vida total y actual (60 max, 6 corazones)
    public int maxHealth = 60;
    public int currentHealth = 60;

    // Array con las im�genes de los corazones en UI (6)
    public Image[] vidaHUD;

    // Sprites para coraz�n lleno, medio y vac�o
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;

    private bool invulnerable = false;

    void Start()
    {
        walkingcamera.SetActive(true);
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            animator.SetBool("Aim", true);
        else if (Input.GetButtonUp("Fire1"))
            animator.SetBool("Aim", false);
    }

    private IEnumerator RecibirDa�o(int damage)
    {
        if (invulnerable)
            yield break;

        invulnerable = true;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth == 0)
        {
            animator.SetBool("Death", true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            yield return new WaitForSeconds(1);
            invulnerable = false;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Por ejemplo, da�o aleatorio 5, 10 o 15
            int damage = 5; // Cambia seg�n el enemigo
            StartCoroutine(RecibirDa�o(damage));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += 10; // Cura 10 puntos = 1 coraz�n completo
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                UpdateHealthUI();
                Destroy(other.gameObject);
            }
        }
    }

    // Actualiza los sprites de los corazones seg�n la vida actual
    private void UpdateHealthUI()
    {
        int pointsPerHeart = maxHealth / vidaHUD.Length; // 10

        for (int i = 0; i < vidaHUD.Length; i++)
        {
            int heartValue = currentHealth - i * pointsPerHeart;

            if (heartValue >= pointsPerHeart)
            {
                vidaHUD[i].sprite = heartFull;
            }
            else if (heartValue >= pointsPerHeart / 2)
            {
                vidaHUD[i].sprite = heartHalf;
            }
            else
            {
                vidaHUD[i].sprite = heartEmpty;
            }
        }
    }
}
