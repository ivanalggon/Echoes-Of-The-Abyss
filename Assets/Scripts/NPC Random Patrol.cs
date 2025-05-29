using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCRandomPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private Animator animator;

    public Transform playerPosition;
    public float rangoDeteccion = 5f;
    private bool persiguiendoJugador = false;
    private Vector3 ultimoDestino;

    public GameObject obstaculoEscena;

    void Start()
    {
        if (obstaculoEscena != null)
            obstaculoEscena.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ultimoDestino = waypoints[currentWaypoint].position;
        agent.SetDestination(ultimoDestino);

        Invoke(nameof(BloquearWaypoint), 10f); // activa obstáculo a los 10 segundos
    }

    void Update()
    {
        if (!persiguiendoJugador && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            int siguiente = ObtenerSiguienteWaypointAleatorioAccesible();
            if (siguiente != -1)
            {
                currentWaypoint = siguiente;
                ultimoDestino = waypoints[currentWaypoint].position;
                agent.SetDestination(ultimoDestino);
                animator.SetFloat("Speed", 1f);
            }
        }
    }

    void FixedUpdate()
    {
        if (playerPosition == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, playerPosition.position);

        if (distanciaAlJugador <= rangoDeteccion)
        {
            persiguiendoJugador = true;
            agent.SetDestination(playerPosition.position);
        }
        else if (persiguiendoJugador)
        {
            persiguiendoJugador = false;
            agent.SetDestination(ultimoDestino);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Dead", true);
            agent.isStopped = true;
        }
    }

    void BloquearWaypoint()
    {
        if (obstaculoEscena != null)
        {
            obstaculoEscena.SetActive(true);
            Debug.Log("Obstáculo activado");
        }
    }

    int ObtenerSiguienteWaypointAleatorioAccesible()
    {
        List<int> disponibles = new List<int>();

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (i == currentWaypoint) continue;

            if (EsCaminoValido(waypoints[i].position))
                disponibles.Add(i);
        }

        if (disponibles.Count == 0)
            return -1;

        return disponibles[Random.Range(0, disponibles.Count)];
    }

    bool EsCaminoValido(Vector3 destino)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destino, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}
