using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    public Transform target;
    public float startDelay = 2f;
    public float maxSpeed = 3f;
    public GameObject greenLight;

    private NavMeshAgent agent;
    private Animator animator;
    private bool hasArrived = false;
    private bool hasStartedMoving = false;

    public int healthPoints = 3; // Puntos de vida del NPC

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        greenLight.SetActive(false);  // Asegurarse de que la luz está apagada al inicio

        agent.speed = 0;  // El NPC no se moverá hasta que pase el retraso
        StartCoroutine(StartMovement());
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si el NPC colisiona con el jugador, se detiene y activa la luz roja
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Dead", true);
        }
    }

    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(startDelay);  // Retraso antes de empezar a moverse
        agent.SetDestination(target.position);
        agent.speed = maxSpeed;  // Establecer la velocidad después del retraso
    }

    void Update()
    {
        if (agent.pathPending || hasArrived) return;  // No hacer nada si aún está calculando el camino o ya llegó

        // Si el NPC aún no ha empezado a moverse, lo detectamos y lo marcamos
        if (!hasStartedMoving && agent.velocity.magnitude > 0.1f)
        {
            hasStartedMoving = true;
        }

        if (hasStartedMoving && !hasArrived)
        {
            // Si aún no ha llegado, aumenta la velocidad según la distancia
            float distance = agent.remainingDistance;
            agent.speed = Mathf.Lerp(1f, maxSpeed, 1f - (distance / Vector3.Distance(transform.position, target.position)));

            // Realiza la rotación suave
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.velocity.normalized), Time.deltaTime * 5f);
            }

            // Actualiza la animación en función de la velocidad
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Al llegar al destino, desactiva la velocidad y activa la luz verde
        if (hasStartedMoving && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f)
        {
            hasArrived = true;
            agent.speed = 0;
            animator.SetFloat("Speed", 0);
            greenLight?.SetActive(true);
            Debug.Log("¡El NPC ha llegado! Activando luz verde.");
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemigo recibió daño: " + damage + " Vida restante: " + (healthPoints - damage));
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            Debug.Log("Enemigo ha muerto");
            animator.SetBool("Dead", true);
            Destroy(gameObject, 2f); // Destruir el NPC después de 2 segundos
        }
    }

    // collision enter take damage

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1); // Llama a la función para recibir daño
        }
    }
}
