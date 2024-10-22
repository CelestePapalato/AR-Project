using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetMovement : MonoBehaviour
{
    [SerializeField]
    float tolerableDistanceToGoal;

    NavMeshAgent agent;

    public event Action OnGoalReached;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
    }

    public bool MoveTowards(Vector3 point)
    {
        NavMeshPath path = new NavMeshPath();
        bool success = agent.CalculatePath(point, path);
        if (success)
        {
            agent.path = path;
            agent.enabled = true;
        }
        return success;
    }

    private IEnumerator CheckDistanceToGoal()
    {
        while(agent.remainingDistance > tolerableDistanceToGoal)
        {
            yield return null;
        }
        agent.enabled = false;
        OnGoalReached.Invoke();
    }
}
