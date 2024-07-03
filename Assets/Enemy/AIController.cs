using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    HealthEnemy healthEnemy;
    public float chaseDistance = 5f;

    private NavMeshAgent agent;
    private List<Transform> chosenPatrolPoints;
    private int currentPatrolIndex;
    private bool isChasing;

    void Start()
    {
        healthEnemy = GetComponent<HealthEnemy>();
        agent = GetComponent<NavMeshAgent>();
        chosenPatrolPoints = new List<Transform>();
        ChooseClusteredPatrolPoints();
        Debug.Log(chosenPatrolPoints.Count);
        currentPatrolIndex = 0;
        agent.destination = chosenPatrolPoints[0].position;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= chaseDistance || healthEnemy.registerHit == true)
            {
                isChasing = true;

            }
            if (isChasing)
            {
                agent.destination = player.position;
                healthEnemy.registerHit = false;
                if (!agent.pathPending && agent.remainingDistance > chaseDistance / 2)
                {
                    isChasing = false;

                }
            }

            if (agent.destination == chosenPatrolPoints[currentPatrolIndex].position)
            {
                Debug.Log("Previous:" + currentPatrolIndex + chosenPatrolPoints.Count);

                if (currentPatrolIndex < chosenPatrolPoints.Count)
                {
                    currentPatrolIndex += 1;
                }
                else if(currentPatrolIndex ==chosenPatrolPoints.Count-1)
                {
                    currentPatrolIndex = 0;
                }
                MoveToPatrolPoint();
            }


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
        agent.destination = chosenPatrolPoints[currentPatrolIndex].position;
    }
}
