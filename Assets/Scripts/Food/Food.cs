using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour
{
    [SerializeField]
    FoodSO foodData;
    public FoodSO Data { get => foodData; }

    [Header("DEBUG")]
    [SerializeField]
    private int feedPoints;
    [SerializeField]
    private int lovePoints;

    [System.Obsolete]
    private MaterialPropertyBlockHelper materialHelper;
    private Rigidbody rb;
    private Collider col;
    private XRGrabInteractable xr_interactable;

    bool initialized = false;

    void Start()
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
        feedPoints = foodData.feedPoints;
        lovePoints = foodData.lovePoints;
    }

    public void Eat(Pet pet)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        xr_interactable.enabled = false;
        pet.Feed(this);
    }

}
