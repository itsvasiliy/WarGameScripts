using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
public class ObstacleAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    [SerializeField] private Transform obstacleAgentTransform;

    [SerializeField] private float carvingTime = 0.5f;
    [SerializeField] private float carvingMoveThreshold = 0.1f;
    
    private Vector3 lastPosition;

    private float lastMoveTime;

    private void Awake()
    {
        navMeshObstacle.enabled = false;
        navMeshObstacle.carveOnlyStationary = false;
        navMeshObstacle.carving = true;

        lastPosition = obstacleAgentTransform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(lastPosition, obstacleAgentTransform.position) > carvingMoveThreshold)
        {
            lastMoveTime = Time.time;
            lastPosition = obstacleAgentTransform.position;
        }
        if (lastMoveTime + carvingTime < Time.time)
        {
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        }
    }

    public void SetDestination(Vector3 Position)
    {
        navMeshObstacle.enabled = false;

        lastMoveTime = Time.time;
        lastPosition = obstacleAgentTransform.position;

        StartCoroutine(MoveAgent(Position));
    }

    private IEnumerator MoveAgent(Vector3 Position)
    {
        yield return null;
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(Position);
    }
}