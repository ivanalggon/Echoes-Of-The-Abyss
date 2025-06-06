using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnNPC : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public Transform player;
    public AudioClip spawnSound;

    private GameObject currentNPC;
    private float spawnInterval = 10f;
    private float deathTime = -1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (currentNPC == null)
        {
            if (deathTime < 0f)
            {
                deathTime = Time.time;
            }

            if (Time.time >= deathTime + spawnInterval)
            {
                // Spawnea el NPC
                currentNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

                var npcScript = currentNPC.GetComponent<NPCEstados>();
                if (npcScript != null)
                {
                    npcScript.playerPosition = player;
                }

                // Reproduce el sonido de spawn
                if (spawnSound != null)
                {
                    audioSource.PlayOneShot(spawnSound);
                }

                deathTime = -1f;
            }
        }
    }
}
