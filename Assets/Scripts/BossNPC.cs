using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Importar para cambiar de escena al morir

public class BossNPC : MonoBehaviour
{
    private Renderer bossRenderer;
    private Color colorOriginal;
    private Color colorEnfadado = new Color(1f, 0f, 0f, 0.8f); // Rojo con 50% de alfa

    // Referencia a la barra de vida del boss
    public BossHealthBar healthBar;
    private int vidaMaxima = 20;
    // Enum para los estados del NPC
    public enum EstadoNPC { Patrullando, Atacando, Huyendo, Perseguir }
    private EstadoNPC estadoActual = EstadoNPC.Patrullando;

    public GameObject Key; // Llave que se spawneará al morir el NPC

    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float tiempoEntreDisparos = 1f;
    private float tiempoDisparoActual = 0f;

    public float distanciaDisparo = 3.5f; // Distancia en la que dispara estando quieto

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
    public int vida = 20;

    // header de sonidos
    [Header("Sonidos del NPC")]
    public AudioClip sonidoDisparo;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoRisa;

    private AudioSource audioSource;
    private bool risaReproducida = false; // Para evitar que la risa se reproduzca varias veces


    private bool isDead = false; // Variable para controlar si el NPC está muerto

    void Start()
    {
        bossRenderer = GetComponentInChildren<Renderer>(); // O GetComponent<Renderer>() si no está en un hijo
        if (bossRenderer != null)
        {
            colorOriginal = bossRenderer.material.color;
        }

        vidaMaxima = vida;
        healthBar.SetHealth(vida, vidaMaxima);

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        agent.speed = velocidadPatrulla;
        IrAlSiguienteWaypoint();

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;  // Sonido 2D, sin efecto espacial
    }
    void Update()
    {
        if (isDead) return; // Si el NPC está muerto, no hacer nada

        float distanciaJugador = Vector3.Distance(transform.position, playerPosition.position);
        bool jugadorVisible = JugadorEnVision();

        // Control de estados con switch y llamadas a funciones separadas
        switch (estadoActual)
        {
            case EstadoNPC.Patrullando:
                Patrullar(distanciaJugador, jugadorVisible);
                break;
            case EstadoNPC.Huyendo:
                Huir(distanciaJugador, jugadorVisible);
                break;
            case EstadoNPC.Atacando:
                Atacar(distanciaJugador, jugadorVisible);
                break;
            case EstadoNPC.Perseguir:
                Perseguir(distanciaJugador, jugadorVisible);
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

        if (jugadorVisible)
        {
            CambiarEstado(EstadoNPC.Atacando);
        }
    }
    void Perseguir(float distancia, bool jugadorVisible)
    {
        agent.isStopped = false;
        agent.SetDestination(playerPosition.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (distancia <= rangoHuida)
        {
            CambiarEstado(EstadoNPC.Huyendo);
            return;
        }

        if (jugadorVisible)
        {
            CambiarEstado(EstadoNPC.Atacando);
            return;
        }

        tiempoSinVerJugador += Time.deltaTime;
        if (tiempoSinVerJugador >= tiempoPerdidaVista)
        {
            CambiarEstado(EstadoNPC.Patrullando);
        }
    }

    void Huir(float distancia, bool jugadorVisible)
    {
        // Si ya ha huido una distancia suficiente, volver a atacar o perseguir
        if (distancia > rangoHuida + 1f) // Le damos un pequeño margen extra
        {
            if (jugadorVisible)
            {
                CambiarEstado(EstadoNPC.Atacando);
            }
            else
            {
                CambiarEstado(EstadoNPC.Perseguir);
            }
            return;
        }

        // Si aún está demasiado cerca, seguir huyendo
        Vector3 direccionHuida = (transform.position - playerPosition.position).normalized;
        Vector3 destinoHuida = transform.position + direccionHuida * 2f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destinoHuida, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Atacar(float distancia, bool jugadorVisible)
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);

        transform.LookAt(new Vector3(playerPosition.position.x, transform.position.y, playerPosition.position.z));

        tiempoDisparoActual += Time.deltaTime;

        if (tiempoDisparoActual >= tiempoEntreDisparos)
        {
            Disparar();
            animator.SetTrigger("Attack"); // <-- Activar animación de ataque por Trigger
            tiempoDisparoActual = 0f;
        }

        if (distancia <= rangoHuida)
        {
            CambiarEstado(EstadoNPC.Huyendo);
            agent.isStopped = false;
        }
        else if (!jugadorVisible)
        {
            tiempoSinVerJugador += Time.deltaTime;
            if (tiempoSinVerJugador >= tiempoPerdidaVista)
            {
                CambiarEstado(EstadoNPC.Perseguir);
                agent.isStopped = false;
            }
        }
        else
        {
            tiempoSinVerJugador = 0f;
        }
    }



    void Disparar()
    {
        if (isDead) return;

        if (risaReproducida)
        {
            // Disparo doble con separación mínima
            DispararBala(offsetLateral: -0.2f);
            DispararBala(offsetLateral: 0.2f);
        }
        else
        {
            // Disparo normal sin separación
            DispararBala(offsetLateral: 0f);
        }
    }


    void DispararBala(float offsetLateral)
    {
        // Calcula dirección al jugador
        Vector3 direccion = (playerPosition.position - puntoDisparo.position).normalized;

        // Calcula una dirección lateral (perpendicular) para separar las balas
        Vector3 direccionLateral = Vector3.Cross(Vector3.up, direccion).normalized;

        // Aplica el desplazamiento lateral
        Vector3 posicionSalida = puntoDisparo.position + direccionLateral * offsetLateral;

        GameObject bala = Instantiate(balaPrefab, posicionSalida, Quaternion.LookRotation(direccion));
        Rigidbody rb = bala.GetComponent<Rigidbody>();
        rb.velocity = direccion * 10f;

        if (sonidoDisparo != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
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
                tiempoSinVerJugador = 0f;
                agent.isStopped = false;
                IrAlSiguienteWaypoint();
                break;

            case EstadoNPC.Huyendo:
                agent.speed = velocidadHuida;
                tiempoSinVerJugador = 0f;
                agent.isStopped = false;
                break;

            case EstadoNPC.Atacando:
                agent.speed = 0f;
                tiempoDisparoActual = 0f;
                tiempoSinVerJugador = 0f;
                agent.isStopped = true;
                break;

            case EstadoNPC.Perseguir:
                agent.speed = velocidadPersecucion;
                tiempoSinVerJugador = 0f;
                agent.isStopped = false;
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

    // Función para recibir daño takedamage
    public void TakeDamage(int damage)
    {
        if (isDead) return; // Si ya está muerto, no hacer nada

        vida -= damage;
        healthBar.SetHealth(vida, vidaMaxima);

        if (!risaReproducida && vida <= vidaMaxima / 2)
        {
            risaReproducida = true;

            // Reproduce la risa
            if (sonidoRisa != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoRisa);
            }

            // Cambiar a color rojo con alfa
            if (bossRenderer != null)
            {
                bossRenderer.material.color = colorEnfadado;
            }
        }

        if (vida <= 0)
        {
            Die();
            // Spawnear Llave
            if (Key != null)
            {
                Quaternion spawnRotation = Quaternion.Euler(-90f, 0.5f, 0f);
                Instantiate(Key, transform.position + Vector3.up * 0.5f, spawnRotation);
            }
        }
        else
        {
            if (sonidoGolpe != null)
            {
                audioSource.PlayOneShot(sonidoGolpe);
            }
            animator.SetBool("Hit", true);
            StartCoroutine(EsperarGolpe());
        }
    }

    // Coroutine para esperar a que la animación de golpe termine
    private IEnumerator EsperarGolpe()
    {
        yield return new WaitForSeconds(0.5f); // Espera 0.5 segundos (ajustar según la duración de la animación)
        animator.SetBool("Hit", false); // Desactiva la animación de golpe
    }

    // Función para manejar la muerte del NPC
    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die"); // Activa la animación de muerte

        Collider collider = GetComponent<Collider>(); // desactivar el collider
        if (collider != null)
        {
            collider.enabled = false;
        }

        agent.isStopped = true;
        if (sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }
        StartCoroutine(EsperarMuerte());
    }

    // Coroutine para esperar a que la animación de muerte termine
    private IEnumerator EsperarMuerte()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos (ajustar según la duración de la animación)
        Destroy(gameObject); // Destruye el objeto NPC
    }

    // Visualización con Gizmos de los rangos y campo de visión (opcional)
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
