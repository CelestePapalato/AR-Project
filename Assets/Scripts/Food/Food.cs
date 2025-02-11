using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour
{
    public static List<Food> spawned = new List<Food>();
    public static event Action OnObjectSpawned;

    public event Action OnFoodDestroyed;

    [SerializeField]
    FoodSO foodData;
    [SerializeField]
    float timeFallingUntilDestroy;

    public FoodSO Data { get => foodData; }

    [Header("DEBUG")]
    [SerializeField]
    private int feedPoints;
    [SerializeField]
    private int waterPoints;
    [SerializeField]
    private int lovePoints;

    [System.Obsolete]
    private MaterialPropertyBlockHelper materialHelper;
    private Rigidbody rb;
    private Collider col;
    private XRGrabInteractable xr_interactable;

    private float timeFalling = 0f;

    bool initialized = false;

    private void Start()
    {
        if(foodData == null)
        {
            Destroy(gameObject);
            return;
        }
        InitializeData(foodData);
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<Collider>();
        xr_interactable = GetComponent<XRGrabInteractable>();
        InitializeScale();
        spawned.Add(this);
        OnObjectSpawned?.Invoke();
    }

    private void Update()
    {
        if(rb.velocity.y < 0)
        {
            timeFalling += Time.deltaTime;
        }

        if(timeFalling >= timeFallingUntilDestroy)
        {
            spawned.Remove(this);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        spawned.Remove(this);
        OnFoodDestroyed = null;
    }

    public void DestroyWithNotify()
    {
        spawned.Remove(this);
        OnFoodDestroyed?.Invoke();
        Destroy(gameObject);
    }

    public void InitializeData(FoodSO food)
    {
        if (initialized)
        {
            Debug.LogWarning("Food already initialized");
            return;
        }
        if(!food)
        {
            Debug.LogWarning("No food Data available");
        }
        foodData = food;
        feedPoints = foodData.FeedPoints;
        waterPoints = foodData.WaterPoints;
        lovePoints = foodData.LovePoints;
    }

    public void Eat(Pet pet)
    {
        rb.isKinematic = true;
        col.enabled = false;
        xr_interactable.enabled = false;
        spawned.Remove(this);
        pet.Feed(this);
    }

    public void InitializeScale()
    {
        transform.localScale = Pet.CurrentPet.transform.localScale;
    }

    public static void DestroyAllInstances()
    {
        Food[] instances = spawned.ToArray();
        foreach(Food food in instances)
        {
            food.DestroyWithNotify();
        }
    }
}
