using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PetAnimationController : MonoBehaviour
{
    Animator animator;
    Pet pet;

    [SerializeField]
    float pettingTransitionLength;

    [SerializeField]
    float pettingLength;

    NavMeshAgent agent;

    float og_speed;

    private void Awake()
    {
        pet = GetComponentInChildren<Pet>();
        animator = pet.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        pet.OnStartEating.AddListener(EatAnimation);
        pet.OnStopEating.AddListener(EatingEnd);
        pet.OnPetting.AddListener(PettingAnimation);
        pet.MovementController.OnMovementStart += IdleAnimation;
    }

    private void OnDisable()
    {
        pet.OnStartEating?.RemoveListener(EatAnimation);
        pet.OnStopEating?.RemoveListener(EatingEnd);
        pet.OnPetting?.RemoveListener(PettingAnimation);
        pet.MovementController.OnMovementStart -= IdleAnimation;
    }

    private void Update()
    {
        bool canMove = animator.GetBool("should_move");
        float speedModifier = (canMove) ? 1 : 0;
        agent.speed = og_speed * speedModifier;

        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed/og_speed);
    }

    private void Start()
    {
        agent = pet.MovementController.Agent;
        og_speed = pet.MovementController.Agent.speed;
        IdleAnimation();
    }

    public void IdleAnimation()
    {
        StopAllCoroutines();
        pet.ShouldSearchFood = true;
        animator.SetBool("Wiggling Tail", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Eating", false);
        animator.SetBool("Angry", false);
    }

    public void EatAnimation()
    {
        StopAllCoroutines();
        animator.SetBool("Wiggling Tail", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Angry", false);
        animator.SetBool("Eating", true);
    }

    public void EatingEnd()
    {
        StopAllCoroutines();
        animator.SetBool("Wiggling Tail", true);
        animator.SetBool("Eating", false);
        pet.ShouldSearchFood = true;
    }

    public void PettingAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PetAnimationCoroutine());
    }

    private IEnumerator PetAnimationCoroutine()
    {
        animator.SetBool("Wiggling Tail", false);
        animator.SetBool("Eating", false);
        animator.SetBool("Angry", false);
        animator.SetBool("Eating", false);
        pet.ShouldSearchFood = false;
        animator.SetBool("Sitting", true);
        yield return new WaitForSeconds(pettingLength);
        animator.SetBool("Sitting", false);
        pet.ShouldSearchFood = true;
        yield return null;
        pet?.CheckForFood();
    }
}
