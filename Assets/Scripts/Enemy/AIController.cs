using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points
    public Transform player; // Reference to the player
    HealthEnemy healthEnemy; // Reference to the enemy's health script
    public float chaseDistance = 5f; // Distance within which the enemy starts chasing the player

    private NavMeshAgent agent; // NavMeshAgent component for pathfinding
    private List<Transform> chosenPatrolPoints; // List of patrol points to be followed
    private int currentPatrolIndex; // Index of the current patrol point
    private bool isChasing; // Flag to indicate if the enemy is chasing the player
    private float waitTime = 1.5f; // Time to wait at each patrol point
    private float waitTimer = 0f; // Timer to track waiting time

    void Start()
    {
        healthEnemy = GetComponent<HealthEnemy>(); // Get the HealthEnemy component
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        chosenPatrolPoints = new List<Transform>(); // Initialize the list of chosen patrol points
        ChooseClusteredPatrolPoints(); // Select the patrol points
        currentPatrolIndex = 0; // Start with the first patrol point
        agent.destination = chosenPatrolPoints[0].position; // Set the destination to the first patrol point
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance || healthEnemy.registerHit)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            agent.destination = player.position;

            if (!agent.pathPending && agent.remainingDistance <= chaseDistance / 2)
            {
                isChasing = false;
            }
        }
        else
        {
            WaitAtPatrolPoint();
        }
    }

    void ChooseClusteredPatrolPoints()
    {
        // Clear the current list of chosen patrol points
        chosenPatrolPoints.Clear();

        if (patrolPoints.Length == 0)
            return;

        // Choose a random starting point
        int startIndex = Random.Range(0, patrolPoints.Length);
        Transform startPoint = patrolPoints[startIndex];

        // Create a list of remaining patrol points
        List<Transform> remainingPoints = new List<Transform>(patrolPoints);
        remainingPoints.RemoveAt(startIndex);

        // Add the starting point to the chosen patrol points
        chosenPatrolPoints.Add(startPoint);

        // Sort the remaining points by distance to the starting point
        remainingPoints.Sort((a, b) => Vector3.Distance(startPoint.position, a.position).CompareTo(Vector3.Distance(startPoint.position, b.position)));

        // Add the closest points to the chosen patrol points
        for (int i = 0; i < remainingPoints.Count; i++)
        {
            chosenPatrolPoints.Add(remainingPoints[i]);
        }
    }

    void MoveToPatrolPoint()
    {
        if (chosenPatrolPoints.Count == 0)
            return;
        agent.destination = chosenPatrolPoints[currentPatrolIndex].position; // Set the destination to the current patrol point
    }

    void WaitAtPatrolPoint()
    {
        // Check if the agent has reached its destination and is not currently chasing
        if (!isChasing && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime; // Increment the timer by the elapsed time

            if (waitTimer >= waitTime) // Check if the wait time has elapsed
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % chosenPatrolPoints.Count; // Move to the next patrol point
                MoveToPatrolPoint(); // Set the destination to the next patrol point
                waitTimer = 0f; // Reset the timer
            }
        }
    }
}
