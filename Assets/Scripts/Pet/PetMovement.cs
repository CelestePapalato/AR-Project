using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetMovement : MonoBehaviour
{
    [SerializeField]
    float tolerableDistanceToGoal;
    [SerializeField]
    float checkRate;

    NavMeshAgent agent;

    public event Action OnGoalReached;

    private Vector3 goal;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
    }

    public void MoveTowards(Vector3 point)
    {
        CancelInvoke(nameof(CheckDistanceToGoal));
        StopCoroutine(WaitForPath());
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.destination = point;
        StartCoroutine(WaitForPath());
    }

    private void CheckDistanceToGoal()
    {
        if (!agent.enabled)
        {
            CancelInvoke(nameof(CheckDistanceToGoal));
            return;
        }
        if(agent.remainingDistance < tolerableDistanceToGoal)
        {
            agent.enabled = false;
            OnGoalReached?.Invoke();
        }
    }

    private IEnumerator WaitForPath()
    {
        while(agent.pathPending)
        {
            yield return new WaitForSeconds(checkRate);
        }
        Debug.Log(agent.pathStatus);
        if(agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            agent.enabled = false;
        }
        else
        {
            InvokeRepeating(nameof(CheckDistanceToGoal), checkRate, checkRate);
        }
    }
}