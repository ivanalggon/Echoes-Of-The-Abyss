using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public int coinValue = 0;

    public GameObject Key;
    public GameObject walkingcamera;
    public Animator animator;
    public TextMeshProUGUI AmmoText;

    private Rigidbody _rb;

    public int maxHealth = 60;
    public int currentHealth = 60;

    public Image[] vidaHUD;
    public Sprite heartFull;
    public Sprite heartHalf;
    public Sprite heartEmpty;

    private bool invulnerable = false;

    [Header("Audio Clips")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip coinSound;
    public AudioClip healSound;

    private AudioSource audioSource;

    void Start()
    {
        Key.SetActive(false);

        coinText.text = "x " + coinValue.ToString();

        walkingcamera.SetActive(true);
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            animator.SetBool("Aim", true);
        else if (Input.GetButtonUp("Fire1"))
            animator.SetBool("Aim", false);
    }

    private IEnumerator RecibirDaño(int damage)
    {
        if (invulnerable)
            yield break;

        invulnerable = true;
        animator.SetBool("Hit", true);

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // Hit
        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        UpdateHealthUI();

        if (currentHealth == 0)
        {
            // Muerte
            if (deathSound != null)
                audioSource.PlayOneShot(deathSound);

            animator.SetBool("Death", true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            invulnerable = false;
            animator.SetBool("Hit", false);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            int damage = 5;
            StartCoroutine(RecibirDaño(damage));
        }
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(RecibirDaño(damage));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += 10;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                UpdateHealthUI();
                Destroy(other.gameObject);

                // Heal
                if (healSound != null)
                    audioSource.PlayOneShot(healSound);
            }
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            coinValue += 5;
            coinText.text = "x " + coinValue.ToString();
            Destroy(other.gameObject);

            // Moneda
            if (coinSound != null)
                audioSource.PlayOneShot(coinSound);
        }

        if (other.gameObject.CompareTag("Key"))
        {
            Key.SetActive(true);
            Destroy(other.gameObject);

            // Llave
            if (coinSound != null)
                audioSource.PlayOneShot(coinSound);
        }
    }

    private void UpdateHealthUI()
    {
        int pointsPerHeart = maxHealth / vidaHUD.Length;

        for (int i = 0; i < vidaHUD.Length; i++)
        {
            int heartValue = currentHealth - i * pointsPerHeart;

            if (heartValue >= pointsPerHeart)
                vidaHUD[i].sprite = heartFull;
            else if (heartValue >= pointsPerHeart / 2)
                vidaHUD[i].sprite = heartHalf;
            else
                vidaHUD[i].sprite = heartEmpty;
        }
    }
}
