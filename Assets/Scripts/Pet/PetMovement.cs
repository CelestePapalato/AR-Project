using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PetMovement : MonoBehaviour
{
    [SerializeField]
    float bakeWaitTime;
    [SerializeField]
    float tolerableDistanceToGoal;
    [SerializeField]
    float checkRate;
    [SerializeField]
    float pathBakeRate;

    NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    public event Action OnGoalAccepted;
    public event Action OnGoalReached;
    public event Action OnMovementStart;
    public event Action OnMovementEnd;

    private Coroutine currentCoroutine;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void MoveTowards(Vector3 point)
    {
        if(currentCoroutine != null) { StopCoroutine(currentCoroutine); }
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.destination = agent.transform.position;
        currentCoroutine = StartCoroutine(GoToPoint(point));
    }

    public void MoveTowards(Transform transform)
    {
        if (currentCoroutine != null) { StopCoroutine(currentCoroutine); }
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.destination = agent.transform.position;
        currentCoroutine = StartCoroutine(Follow(transform));
    }

    public void Stop()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        agent.enabled = false;
    }

    private IEnumerator GoToPoint(Vector3 objective)
    {
        float distanceToGoal = Mathf.Infinity;
        while (distanceToGoal > tolerableDistanceToGoal && agent.enabled)
        {
            SurfaceBaker.Instance.BakeSurfaces();
            while(NavMeshSurface.activeSurfaces.Count == 0)
            {
                yield return null;
            }

            OnMovementStart?.Invoke();

            agent.SetDestination(objective);

            while (agent.pathPending)
            {
                yield return new WaitForSeconds(checkRate);
            }

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                agent.Warp(objective);
                agent.enabled = false;
                OnMovementEnd?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(pathBakeRate);

            distanceToGoal = agent.remainingDistance;
        }

        if (agent.remainingDistance <= tolerableDistanceToGoal)
        {
            OnGoalReached?.Invoke();
        }

        agent.enabled = false;

        OnMovementEnd?.Invoke();
    }

    IEnumerator Follow(Transform objective)
    {
        if (!objective) { yield break; }
        while (agent.enabled && objective)
        {
            SurfaceBaker.Instance.BakeSurfaces();
            while (NavMeshSurface.activeSurfaces.Count == 0)
            {
                yield return null;
            }

            OnMovementStart?.Invoke();

            if (!objective) {  break; }
            agent.SetDestination(objective.position);

            while (agent.pathPending)
            {
                yield return new WaitForSeconds(checkRate);
            }

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                agent.Warp(objective.position);
                agent.enabled = false;
                OnMovementEnd?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(pathBakeRate);
        }

        agent.enabled = false;

        OnMovementEnd?.Invoke();
    }
}
