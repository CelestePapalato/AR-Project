using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(PetStats))]
[RequireComponent(typeof(PetMovement))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Pet : MonoBehaviour
{
    public static Pet CurrentPet;

    [SerializeField]
    PetSO petData;
    [SerializeField]
    Transform playerTransform;

    [Header("States Configuration")]
    [SerializeField]
    float maxDistanceToPlayer = 20f;
    [SerializeField]
    float timerForWandering;

    public PetSO Data {get => petData; }
    public PetMovement MovementController { get => movement; }

    XRGrabInteractable XR_interactable;
    PetStats stats;
    PetMovement movement;

    private bool initialiazed = false;

    private bool isEating = false;
    private bool isSearchingFood = false;
    private bool isWandering = false;

    public bool ShouldSearchFood = true;

    public UnityAction<int> OnFeed;
    public UnityAction<int> OnWater;
    public UnityAction<int> OnLove;
    public UnityAction<int> OnPet;
    public UnityAction<Vector3> OnSizeChange;

    public UnityAction OnStartEating;
    public UnityAction OnStopEating;
    public UnityAction OnPetting;

    private Food foodFollowing;

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
        stats = GetComponent<PetStats>();
        InitializeData();
        StartWanderingTimer();
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
            movement.OnGoalReached += StartWanderingTimer;
            movement.OnGoalReached += EnableInteractable;
        }
        Food.OnObjectSpawned += MoveTowardsFood;

        if (foodFollowing)
        {
            foodFollowing.OnFoodDestroyed += OnFoodDeleted;
        }
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
            movement.OnGoalReached -= StartWanderingTimer;
            movement.OnGoalReached -= EnableInteractable;
        }
        Food.OnObjectSpawned -= MoveTowardsFood;

        if (foodFollowing)
        {
            foodFollowing.OnFoodDestroyed -= OnFoodDeleted;
        }
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
        if (isSearchingFood || !ShouldSearchFood) { return; }
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
            foodFollowing = toEat;
            CancelInvoke();
            movement.Stop();
            movement.MoveTowards(toEat.transform);
            foodFollowing.OnFoodDestroyed += OnFoodDeleted;
        }
    }

    private void OnFoodDeleted()
    {
        foodFollowing.OnFoodDestroyed -= OnFoodDeleted;
        isSearchingFood = false;
        movement.Stop();
        CheckForFood();
    }

    public void CheckForFood()
    {
        if (isSearchingFood || !ShouldSearchFood) { return; }
        foodFollowing = null;
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
        if(isEating) { return; }
        isSearchingFood = false;
        movement?.Stop();
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
        CancelInvoke();
        DisableInteractable();
        movement.Stop();
        isEating = true;
        LookAt(food.gameObject);
        OnStartEating?.Invoke();
        yield return new WaitForSeconds(food.Data.EatingTime);
        stats.Feed.Value += food.Data.FeedPoints;
        stats.Water.Value += food.Data.WaterPoints;
        stats.Love.Value += food.Data.LovePoints;
        OnStopEating?.Invoke();
        OnFeed?.Invoke(food.Data.FeedPoints);
        OnWater?.Invoke(food.Data.WaterPoints);
        OnLove?.Invoke(food.Data.LovePoints);
        food.DestroyWithNotify();
        isEating = false;
        EnableInteractable();
        StartWanderingTimer();
    }

    [ContextMenu("Pet")]
    private void Love(SelectEnterEventArgs xr_event)
    {
        CancelInvoke();
        StartWanderingTimer();
        if (stats.Petting.isMax || isEating) { return; }
        isSearchingFood = false;
        movement.Stop();
        stats.Love.Value += petData.pettingLovePoints;
        OnLove?.Invoke(petData.pettingLovePoints);
        OnPet?.Invoke(petData.pettingLovePoints);
        OnPetting?.Invoke();
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
            if(food != foodFollowing)
            {
                foodFollowing.OnFoodDestroyed -= OnFoodDeleted;
                foodFollowing = food;
                foodFollowing.OnFoodDestroyed += OnFoodDeleted;
            }
            
            if (ShouldEat(food.Data))
            {
                food.Eat(this);
            }
        }
    }

    private void StartWanderingTimer()
    {
        Invoke(nameof(StartWandering), timerForWandering);
    }

    private void StartWandering()
    {
        movement?.Wander();
    }

}
