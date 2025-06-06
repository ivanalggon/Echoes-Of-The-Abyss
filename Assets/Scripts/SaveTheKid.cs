using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveTheKid : MonoBehaviour
{
    public float interactDistance = 3f;
    public GameObject kid;                  // Referencia al objeto del niño
    public Transform player;               // Referencia al jugador
    public GameObject interactionCanvas;   // El canvas con "Press E to save the kid"

    void Start()
    {
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, kid.transform.position);

        if (distance <= interactDistance)
        {
            if (interactionCanvas != null)
                interactionCanvas.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                SaveKid();
            }
        }
        else
        {
            if (interactionCanvas != null)
                interactionCanvas.SetActive(false);
        }
    }

    void SaveKid()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    void OnDrawGizmosSelected()
    {
        if (kid != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(kid.transform.position, interactDistance);
        }
    }
}
