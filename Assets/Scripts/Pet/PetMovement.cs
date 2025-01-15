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
    [SerializeField]
    float wanderingDirectionUpdateRate;

    NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }

    public event Action OnGoalAccepted;
    public event Action OnGoalReached;
    public event Action OnMovementStart;
    public event Action OnMovementEnd;

    private Coroutine currentCoroutine;

    bool isWandering = false;
    float maxSpeed = 1f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        maxSpeed = agent.speed;
    }

    public void MoveTowards(Vector3 point)
    {
        isWandering = false;
        if (currentCoroutine != null) { StopCoroutine(currentCoroutine); }
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.destination = agent.transform.position;
        currentCoroutine = StartCoroutine(GoToPoint(point));
    }

    public void MoveTowards(Transform transform)
    {
        isWandering = false;
        if (currentCoroutine != null) { StopCoroutine(currentCoroutine); }
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.destination = agent.transform.position;
        currentCoroutine = StartCoroutine(Follow(transform));
    }

    public void Stop()
    {
        isWandering = false;
        if (currentCoroutine != null)
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
            while (NavMeshSurface.activeSurfaces.Count == 0)
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

            if (!objective) { break; }
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

    public void Wander()
    {
        if (currentCoroutine != null) { StopCoroutine(currentCoroutine); }
        agent.enabled = true;
        currentCoroutine = StartCoroutine(WanderCoroutine());
    }

    IEnumerator WanderCoroutine()
    {
        isWandering = true;

        float speed = maxSpeed * 0.6f;

        SurfaceBaker.Instance?.BakeSurfaces();

        while (NavMeshSurface.activeSurfaces.Count == 0)
        {
            yield return bakeWaitTime;
        }

        float bakingTimer = 1f;
        float t = bakingTimer;

        Vector3 velocity = Vector3.zero;
        Vector3 desired_velocity = Vector3.zero;
        float t_velocity = 0;
        float lerp = 1.2f;

        agent.velocity = agent.transform.forward * speed;

        OnMovementStart?.Invoke();

        while (agent.enabled && isWandering)
        {
            t_velocity -= Time.deltaTime;
            if (t_velocity <= 0f)
            {
                velocity = UnityEngine.Random.onUnitSphere;
                velocity.z = 0;
                velocity.Normalize();
                t_velocity = wanderingDirectionUpdateRate;
                desired_velocity = velocity * speed;
            }
            Vector3 current = agent.velocity;
            agent.velocity = Vector3.Lerp(agent.velocity, desired_velocity, Time.deltaTime * lerp);
            t -= Time.deltaTime;
            if (t <= 0f)
            {
                SurfaceBaker.Instance?.BakeSurfaces();
                t = bakingTimer;
            }
            yield return null;
        }
        isWandering = false;
        agent.enabled = false;
        OnMovementEnd?.Invoke();
        currentCoroutine = null;
    }
}
