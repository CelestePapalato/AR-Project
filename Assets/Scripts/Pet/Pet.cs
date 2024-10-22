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
public class Pet : MonoBehaviour
{
    [SerializeField]
    PetSO petData;
    public PetSO Data {get => petData; }

    Animator animator;
    XRSimpleInteractable XR_interactable;
    PetStats stats;

    private bool initialiazed = false;

    private bool isEating = false;

    public UnityAction<int> OnFeed;
    public UnityAction<int> OnLove;
    public UnityAction<int> OnPet;

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
            XR_interactable = GetComponentInChildren<XRSimpleInteractable>();
        }
        if (XR_interactable)
        {
            XR_interactable.selectEntered.AddListener(Love);
        }
    }

    private void OnDisable()
    {
        if (XR_interactable)
        {
            XR_interactable.selectEntered?.RemoveListener(Love);
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

    [ContextMenu("Feed")]
    public void Feed(Food food)
    {
        if (food && food.Data && !stats.Feed.isMax)
        {
            StartCoroutine(FeedSequence(food));
        }
    }

    public IEnumerator FeedSequence(Food food)
    {
        isEating = true;
        LookAt(food.gameObject);
        animator.SetInteger("AnimationID", 5);
        yield return new WaitForSeconds(food.Data.EatingTime);
        stats.Feed.Value += food.Data.FeedPoints;
        stats.Love.Value += food.Data.LovePoints;
        Destroy(food.gameObject);
        animator.SetInteger("AnimationID", 1);
        isEating = false;
        OnFeed?.Invoke(food.Data.FeedPoints);
        OnLove?.Invoke(food.Data.LovePoints);
    }

    [ContextMenu("Pet")]
    private void Love(SelectEnterEventArgs xr_event)
    {
        if(stats.Petting.isMax) { return; }
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
        if (collision.gameObject.CompareTag("Food") && !isEating && !stats.Feed.isMax)
        {
            collision.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            Food food = collision.gameObject.GetComponent<Food>();

            food.Eat(this);
        }
    }

}
