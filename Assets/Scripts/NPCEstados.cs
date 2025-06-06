using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCEstados : MonoBehaviour
{
    // Enum para los estados del NPC
    public enum EstadoNPC { Patrullando, Persiguiendo, Huyendo }
    private EstadoNPC estadoActual = EstadoNPC.Patrullando;

    public Transform[] waypoints;
    private int currentWaypoint = 0;

    public Transform playerPosition;
    public float rangoDeteccion = 5f; // Rango para detectar jugador
    public float anguloVision = 90f;   // Campo de visión
    public float rangoHuida = 1f;      // Rango de huida
    public float tiempoPerdidaVista = 2f; // Tiempo para perder de vista al jugador

    private NavMeshAgent agent;
    private Animator animator;
    private float tiempoSinVerJugador = 0f;

    // Velocidades para cada estado
    public float velocidadPatrulla = 1f;
    public float velocidadPersecucion = 1.5f;
    public float velocidadHuida = 1f;

    // Vida del NPC
    public int vida = 3;

    // Moneda spawneada
    public GameObject moneda; // Moneda a spawnear

    // Sonidos
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        agent.speed = velocidadPatrulla;
        IrAlSiguienteWaypoint();
    }

    void Update()
    {
        float distanciaJugador = Vector3.Distance(transform.position, playerPosition.position);
        bool jugadorVisible = JugadorEnVision();

        // Control de estados con switch y llamadas a funciones separadas
        switch (estadoActual)
        {
            case EstadoNPC.Patrullando:
                Patrullar(distanciaJugador, jugadorVisible);
                break;
            case EstadoNPC.Persiguiendo:
                Perseguir(distanciaJugador, jugadorVisible);
                break;
            case EstadoNPC.Huyendo:
                Huir(distanciaJugador, jugadorVisible);
                break;
        }
    }

    void Patrullar(float distancia, bool jugadorVisible)
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            IrAlSiguienteWaypoint();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (jugadorVisible && distancia <= rangoDeteccion)
        {
            CambiarEstado(EstadoNPC.Persiguiendo);
        }
    }

    void Perseguir(float distancia, bool jugadorVisible)
    {
        agent.SetDestination(playerPosition.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (distancia <= rangoHuida)
        {
            CambiarEstado(EstadoNPC.Huyendo);
        }
        else if (!jugadorVisible || distancia > rangoDeteccion)
        {
            tiempoSinVerJugador += Time.deltaTime;
            if (tiempoSinVerJugador >= tiempoPerdidaVista)
            {
                CambiarEstado(EstadoNPC.Patrullando);
            }
        }
        else
        {
            tiempoSinVerJugador = 0f;
        }
    }

    void Huir(float distancia, bool jugadorVisible)
    {
        Vector3 direccionHuida = (transform.position - playerPosition.position).normalized;
        Vector3 destinoHuida = transform.position + direccionHuida * 5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destinoHuida, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (distancia > rangoHuida + 2f && jugadorVisible)
        {
            CambiarEstado(EstadoNPC.Persiguiendo);
        }
        else if (!jugadorVisible)
        {
            tiempoSinVerJugador += Time.deltaTime;
            if (tiempoSinVerJugador >= tiempoPerdidaVista)
            {
                CambiarEstado(EstadoNPC.Patrullando);
            }
        }
    }

    void IrAlSiguienteWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void CambiarEstado(EstadoNPC nuevoEstado)
    {
        estadoActual = nuevoEstado;

        switch (nuevoEstado)
        {
            case EstadoNPC.Patrullando:
                agent.speed = velocidadPatrulla;
                IrAlSiguienteWaypoint();
                break;

            case EstadoNPC.Persiguiendo:
                agent.speed = velocidadPersecucion;
                tiempoSinVerJugador = 0f;
                break;

            case EstadoNPC.Huyendo:
                agent.speed = velocidadHuida;
                tiempoSinVerJugador = 0f;
                break;
        }
    }

    // Función para detectar si el jugador está en el campo de visión y línea de vista
    bool JugadorEnVision()
    {
        Vector3 direccionJugador = playerPosition.position - transform.position;
        float angulo = Vector3.Angle(transform.forward, direccionJugador);

        if (angulo < anguloVision / 2f && Vector3.Distance(transform.position, playerPosition.position) <= rangoDeteccion)
        {
            Ray ray = new Ray(transform.position + Vector3.up, direccionJugador.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, rangoDeteccion))
            {
                if (hit.transform == playerPosition)
                    return true;
            }
        }

        return false;
    }

    public void TakeDamage(int damage)
    {
        vida -= damage;

        if (vida <= 0)
        {
            PlayDeathSound();
            Die();
        }
        else
        {
            PlayHitSound();
            animator.SetBool("Hit", true); // Activa la animación de golpe
            StartCoroutine(EsperarGolpe());
        }
    }

    private IEnumerator EsperarGolpe()
    {
        yield return new WaitForSeconds(0.5f); // Espera 0.5 segundos (ajustar según la duración de la animación)
        animator.SetBool("Hit", false); // Desactiva la animación de golpe
    }

    private void Die()
    {
        animator.SetTrigger("Die"); // Activa la animación de muerte

        Collider collider = GetComponent<Collider>(); // desactivar el collider
        if (collider != null)
        {
            collider.enabled = false;
        }

        agent.isStopped = true;
        StartCoroutine(EsperarMuerte());
    }

    private IEnumerator EsperarMuerte()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos (ajustar según la duración de la animación)
        Destroy(gameObject); // Destruye el objeto NPC

        // Spawn de un objeto de loot
        if (moneda != null)
        {
            Quaternion spawnRotation = Quaternion.Euler(-90f, 0.5f, 0f);
            Instantiate(moneda, transform.position + Vector3.up * 0.5f, spawnRotation);
        }
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoHuida);

        if (playerPosition != null)
        {
            Vector3 leftRay = Quaternion.Euler(0, -anguloVision / 2, 0) * transform.forward;
            Vector3 rightRay = Quaternion.Euler(0, anguloVision / 2, 0) * transform.forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, leftRay * rangoDeteccion);
            Gizmos.DrawRay(transform.position + Vector3.up, rightRay * rangoDeteccion);
        }
    }
}
