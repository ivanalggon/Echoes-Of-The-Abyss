using UnityEngine;
using System.Collections;

public class CofreInteractivo : MonoBehaviour
{
    public GameObject canvasUI;           // Canvas con la UI para abrir el cofre
    public int monedasPorAbrir = 10;
    public float distanciaActivacion = 3f;
    public AudioClip coinSound;           // Sonido de moneda
    private AudioSource audioSource;

    private PlayerController playerController;
    private Transform playerTransform;
    private bool canvasActivo = false;
    private bool cofreAbierto = false;

    void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
            audioSource = playerObj.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = playerObj.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || playerController == null || cofreAbierto)
            return;

        float distancia = Vector3.Distance(transform.position, playerTransform.position);

        if (distancia <= distanciaActivacion)
        {
            if (!canvasActivo && canvasUI != null)
            {
                canvasUI.SetActive(true);
                canvasActivo = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                AbrirCofre();
            }
        }
        else
        {
            if (canvasActivo && canvasUI != null)
            {
                canvasUI.SetActive(false);
                canvasActivo = false;
            }
        }
    }

    void AbrirCofre()
    {
        cofreAbierto = true;

        // Dar monedas al jugador
        playerController.coinValue += monedasPorAbrir;
        playerController.coinText.text = "x " + playerController.coinValue.ToString();

        // Reproducir sonido de moneda
        if (coinSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

        // Ocultar canvas UI
        if (canvasUI != null)
        {
            canvasUI.SetActive(false);
            canvasActivo = false;
        }

        // Destruir el cofre tras 2 segundos
        StartCoroutine(DestruirCofre());
    }

    IEnumerator DestruirCofre()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
