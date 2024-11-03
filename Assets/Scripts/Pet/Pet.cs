using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(PetStats))]
[RequireComponent(typeof(PetMovement))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Pet : MonoBehaviour
{
    public static Pet CurrentPet;

    [SerializeField]
    PetSO petData;
    public PetSO Data {get => petData; }

    Animator animator;
    XRGrabInteractable XR_interactable;
    PetStats stats;
    PetMovement movement;

    private bool initialiazed = false;

    private bool isEating = false;
    private bool isSearchingFood = false;

    public UnityAction<int> OnFeed;
    public UnityAction<int> OnWater;
    public UnityAction<int> OnLove;
    public UnityAction<int> OnPet;
    public UnityAction<Vector3> OnSizeChange;

    private void Awake()
    {
        if(CurrentPet != null && CurrentPet != this)
        {
            Destroy(this);
            return;
        }
        CurrentPet = this;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PetStats>();
        InitializeData();
    }

    private void OnEnable()
    {
        if (!XR_interactable)
        {
            XR_interactable = GetComponentInChildren<XRGrabInteractable>();
        }
        if (XR_interactable)
        {
            XR_interactable.selectEntered.AddListener(Love);
        }
        if (!movement)
        {
            movement = GetComponent<PetMovement>();
        }
        if (movement)
        {
            movement.OnGoalAccepted += DisableInteractable;
            movement.OnGoalReached += EnableInteractable;
        }
        Food.OnObjectSpawned += MoveTowardsFood;
    }

    private void OnDisable()
    {
        if (XR_interactable)
        {
            XR_interactable.selectEntered?.RemoveListener(Love);
        }
        if (movement)
        {
            movement.OnGoalAccepted -= DisableInteractable;
            movement.OnGoalReached -= EnableInteractable;
        }
        Food.OnObjectSpawned -= MoveTowardsFood;
    }

    private void InitializeData()
    {
        if(initialiazed)
        {
            Debug.LogWarning("Pet data already initialized");
            return;
        }
        if (!petData)
        {
            Debug.LogWarning("No pet data available");
            return;
        }
    }

    public void DisableInteractable()
    {
        XR_interactable.enabled = false;
    }

    public void EnableInteractable()
    {
        XR_interactable.enabled = true;
    }

    public void MoveTowardsFood()
    {
        if (isSearchingFood) { return; }
        List<Food> list = Food.spawned;
        Food toEat = null;
        for(int i = 0; i < list.Count; i++)
        {
            if (ShouldEat(list[i].Data))
            {
                toEat = list[i];
                break;
            }
        }

        if (toEat)
        {
            isSearchingFood = true;
            movement.MoveTowards(toEat.transform);
        }
    }

    private void CheckIfThereIsMoreFood()
    {
        isSearchingFood = false;
        if(Food.spawned.Count > 0 )
        {
            MoveTowardsFood();
        }
    }

    private bool ShouldEat(FoodSO data)
    {
        bool shouldEat = !stats.Feed.isMax;
        bool shouldHydrate = data.FeedPoints <= 0 && !stats.Water.isMax && data.WaterPoints > 0;
        return shouldEat || shouldHydrate;
    }

    public void MoveTowards(Vector3 point)
    {
        movement.MoveTowards(point);
    }

    [ContextMenu("Feed")]
    public void Feed(Food food)
    {
        if (food && food.Data)
        {
            StartCoroutine(FeedSequence(food));
        }
    }

    public IEnumerator FeedSequence(Food food)
    {
        DisableInteractable();
        movement.Stop();
        isEating = true;
        LookAt(food.gameObject);
        animator.SetInteger("AnimationID", 5);
        yield return new WaitForSeconds(food.Data.EatingTime);
        stats.Feed.Value += food.Data.FeedPoints;
        stats.Water.Value += food.Data.WaterPoints;
        stats.Love.Value += food.Data.LovePoints;
        Destroy(food.gameObject);
        animator.SetInteger("AnimationID", 1);
        isEating = false;
        EnableInteractable();
        OnFeed?.Invoke(food.Data.FeedPoints);
        OnWater?.Invoke(food.Data.WaterPoints);
        OnLove?.Invoke(food.Data.LovePoints);
        CheckIfThereIsMoreFood();
    }

    [ContextMenu("Pet")]
    private void Love(SelectEnterEventArgs xr_event)
    {
        if(stats.Petting.isMax || isEating) { return; }
        movement.Stop();
        stats.Love.Value += petData.pettingLovePoints;
        OnLove?.Invoke(petData.pettingLovePoints);
        OnPet?.Invoke(petData.pettingLovePoints);
        animator.SetInteger("AnimationID", 7);
        stats.Petting.Value += 1;
    }

    private void LookAt(GameObject obj)
    {
        transform.LookAt(obj.transform.position);
        Vector3 euler = transform.eulerAngles;
        euler.x = 0f;
        transform.rotation = Quaternion.Euler(euler);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food") && !isEating)
        {
            Food food = collision.gameObject.GetComponent<Food>();
            if (ShouldEat(food.Data))
            {
                food.Eat(this);
            }
        }
    }

}
