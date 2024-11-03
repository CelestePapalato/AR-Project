using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    private void Awake()
    {
        pet = GetComponent<Pet>();
        animator = pet.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        pet.OnStartEating += EatAnimation;
        pet.OnStopEating += EatingEnd;
        pet.OnPetting += PettingAnimation;
        pet.MovementController.OnMovementStart += MoveAnimation;
        pet.MovementController.OnMovementEnd += IdleAnimation;
    }

    private void OnDisable()
    {
        pet.OnStartEating -= EatAnimation;
        pet.OnStopEating -= EatingEnd;
        pet.OnPetting -= PettingAnimation;
        pet.MovementController.OnMovementStart -= MoveAnimation;
        pet.MovementController.OnMovementEnd -= IdleAnimation;
    }

    private void Start()
    {
        IdleAnimation();
        pet = GetComponentInChildren<Pet>();
    }

    public void IdleAnimation()
    {
        StopAllCoroutines();
        animator.SetInteger("AnimationID", 0);
    }

    public void MoveAnimation()
    {
        StopAllCoroutines();
        animator.SetInteger("AnimationID", 4);
    }

    public void EatAnimation()
    {
        StopAllCoroutines();
        animator.SetInteger("AnimationID", 5);
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

    public void HappyAnimation()
    {
        StopAllCoroutines();
        animator.SetInteger("AnimationID", 1);
    }

    private IEnumerator EatingEndAnimationCoroutine()
    {
        pet.ShouldSearchFood = false;
        animator.SetInteger("AnimationID", 1);
        yield return new WaitForSeconds(eatingTransitionLength);
        Debug.Log(pet.ShouldSearchFood);
        pet.ShouldSearchFood = true;
        pet?.CheckForFood();
    }

    private IEnumerator PetAnimationCoroutine()
    {
        pet.ShouldSearchFood = false;
        animator.SetInteger("AnimationID", 7);
        yield return new WaitForSeconds(pettingLength);
        animator.SetInteger("AnimationID", 0);
        yield return new WaitForSeconds(pettingTransitionLength);
        pet.ShouldSearchFood = true;
        pet?.CheckForFood();
    }
}
