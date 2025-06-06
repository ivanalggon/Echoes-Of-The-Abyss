using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject Key;
    public GameObject DoorObject;
    public GameObject canvasWithKey;
    public GameObject canvasNoKey;

    public bool isOpen = false;
    public float interactDistance = 3f;

    public AudioClip doorOpenSound;
    private AudioSource audioSource;

    private Transform player;

    void Start()
    {
        canvasWithKey.SetActive(false);
        canvasNoKey.SetActive(false);

        DoorObject.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player").transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance && !isOpen)
        {
            if (Key.activeSelf)
            {
                canvasWithKey.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(OpenDoorAndPlaySound());
                }
            }
            else
            {
                canvasNoKey.SetActive(true);
            }
        }
        else
        {
            canvasWithKey.SetActive(false);
            canvasNoKey.SetActive(false);
        }
    }

    private IEnumerator OpenDoorAndPlaySound()
    {
        isOpen = true;
        Key.SetActive(false);

        if (doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
            canvasWithKey.SetActive(false);
            // Espera el tiempo del clip para destruir o desactivar después
            yield return new WaitForSeconds(doorOpenSound.length);
        }

        // Ahora desactiva o destruye la puerta
        DoorObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
