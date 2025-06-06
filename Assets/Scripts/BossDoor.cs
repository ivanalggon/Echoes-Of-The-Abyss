using System.Collections;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public GameObject DoorObject;
    public GameObject canvasWithCoins;   // "Presiona E para entrar al boss"
    public GameObject canvasNoCoins;     // "Necesitas 50 monedas"
    public AudioClip doorOpenSound;

    public float interactDistance = 4f;
    public bool isOpen = false;

    private AudioSource audioSource;
    private Transform player;
    private PlayerController playerController;  // Para acceder a las monedas

    public int requiredCoins = 50;

    void Start()
    {
        canvasWithCoins.SetActive(false);
        canvasNoCoins.SetActive(false);

        DoorObject.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (isOpen || player == null || playerController == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            if (playerController.coinValue >= requiredCoins)
            {
                canvasWithCoins.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(OpenDoorAndPlaySound());
                }
            }
            else
            {
                canvasNoCoins.SetActive(true);
            }
        }
        else
        {
            canvasWithCoins.SetActive(false);
            canvasNoCoins.SetActive(false);
        }
    }

    private IEnumerator OpenDoorAndPlaySound()
    {
        isOpen = true;

        if (doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
            yield return new WaitForSeconds(doorOpenSound.length);
            canvasWithCoins.SetActive(false);
        }

        DoorObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
