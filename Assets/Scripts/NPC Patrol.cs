using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private Animator animator;

    public Transform playerPosition;
    public float rangoDeteccion = 5f; // Rango donde persigue al jugador
    private bool persiguiendoJugador = false;
    private Vector3 ultimoDestino;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ultimoDestino = waypoints[currentWaypoint].position;
        agent.SetDestination(ultimoDestino);
    }

    void Update()
    {
        if (!persiguiendoJugador && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            ultimoDestino = waypoints[currentWaypoint].position;
            agent.SetDestination(ultimoDestino);
            animator.SetFloat("Speed", 1f);
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
}
