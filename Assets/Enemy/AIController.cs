using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseDistance = 5f;

    private NavMeshAgent agent;
    private List<Transform> chosenPatrolPoints;
    private int currentPatrolIndex;
    private bool isChasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        chosenPatrolPoints = new List<Transform>();
        ChooseClusteredPatrolPoints();
        currentPatrolIndex = 0;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (player!=null )
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= chaseDistance)
            {
                isChasing = true;
                agent.destination = player.position;
            }
            else
            {
                if (isChasing)
                {
                    isChasing = false;
                    GoToNextPatrolPoint();
                }

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextPatrolPoint();
                }
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

    void GoToNextPatrolPoint()
    {
        if (chosenPatrolPoints.Count == 0)
            return;

        agent.destination = chosenPatrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % chosenPatrolPoints.Count;
    }
}
