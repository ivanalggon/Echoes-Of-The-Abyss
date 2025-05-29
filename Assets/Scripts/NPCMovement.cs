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
        greenLight.SetActive(false);  // Asegurarse de que la luz est� apagada al inicio

        agent.speed = 0;  // El NPC no se mover� hasta que pase el retraso
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
        agent.speed = maxSpeed;  // Establecer la velocidad despu�s del retraso
    }

    void Update()
    {
        if (agent.pathPending || hasArrived) return;  // No hacer nada si a�n est� calculando el camino o ya lleg�

        // Si el NPC a�n no ha empezado a moverse, lo detectamos y lo marcamos
        if (!hasStartedMoving && agent.velocity.magnitude > 0.1f)
        {
            hasStartedMoving = true;
        }

        if (hasStartedMoving && !hasArrived)
        {
            // Si a�n no ha llegado, aumenta la velocidad seg�n la distancia
            float distance = agent.remainingDistance;
            agent.speed = Mathf.Lerp(1f, maxSpeed, 1f - (distance / Vector3.Distance(transform.position, target.position)));

            // Realiza la rotaci�n suave
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.velocity.normalized), Time.deltaTime * 5f);
            }

            // Actualiza la animaci�n en funci�n de la velocidad
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Al llegar al destino, desactiva la velocidad y activa la luz verde
        if (hasStartedMoving && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f)
        {
            hasArrived = true;
            agent.speed = 0;
            animator.SetFloat("Speed", 0);
            greenLight?.SetActive(true);
            Debug.Log("�El NPC ha llegado! Activando luz verde.");
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemigo recibi� da�o: " + damage + " Vida restante: " + (healthPoints - damage));
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            Debug.Log("Enemigo ha muerto");
            animator.SetBool("Dead", true);
            Destroy(gameObject, 2f); // Destruir el NPC despu�s de 2 segundos
        }
    }

    // collision enter take damage

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1); // Llama a la funci�n para recibir da�o
        }
    }
}
