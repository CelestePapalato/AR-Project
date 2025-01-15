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
    float eatingTransitionLength;
    [SerializeField]
    float pettingTransitionLength;

    [SerializeField]
    float pettingLength;

    NavMeshAgent agent;
    AnimationEventHandler animatorEvent;

    float og_speed;

    private void Awake()
    {
        pet = GetComponentInChildren<Pet>();
        animator = pet.GetComponentInChildren<Animator>();
        animatorEvent = pet.GetComponentInChildren<AnimationEventHandler>();
    }

    private void OnEnable()
    {
        pet.OnStartEating += EatAnimation;
        pet.OnStopEating += EatingEnd;
        pet.OnPetting += PettingAnimation;
        pet.MovementController.OnMovementStart += IdleAnimation;
        animatorEvent.onAnimationEvent += ModifySpeed;
    }

    private void OnDisable()
    {
        pet.OnStartEating -= EatAnimation;
        pet.OnStopEating -= EatingEnd;
        pet.OnPetting -= PettingAnimation;
        pet.MovementController.OnMovementStart -= IdleAnimation;
        animatorEvent.onAnimationEvent -= ModifySpeed;
    }

    private void Update()
    {
        float currentSpeed = agent.velocity.magnitude;
        float maxSpeed = agent.speed;
        animator.SetFloat("Speed", currentSpeed/maxSpeed);
    }

    private void ModifySpeed(CharacterAnimatorState state)
    {
        switch (state)
        {
            case CharacterAnimatorState.MOVE:
                agent.speed = og_speed;
                break;
            case CharacterAnimatorState.STOP:
                agent.speed = 0;
                break;
        }
    }

    private void Start()
    {
        IdleAnimation();
        agent = pet.MovementController.Agent;
        og_speed = pet.MovementController.Agent.speed;
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
        StartCoroutine(EatingEndAnimationCoroutine());
    }

    public void PettingAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PetAnimationCoroutine());
    }

    private IEnumerator EatingEndAnimationCoroutine()
    {
        pet.ShouldSearchFood = false;
        animator.SetBool("Wiggling Tail", true);
        animator.SetBool("Eating", false);
        yield return new WaitForSeconds(eatingTransitionLength);
        pet.ShouldSearchFood = true;
        pet?.CheckForFood();
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
        yield return new WaitForSeconds(pettingTransitionLength);
        pet.ShouldSearchFood = true;
        pet?.CheckForFood();
    }
}
