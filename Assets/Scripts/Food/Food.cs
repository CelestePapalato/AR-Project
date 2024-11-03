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

    private void OnDestroy()
    {
        spawned.Remove(this);
        OnFoodDestroyed?.Invoke();
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

}
