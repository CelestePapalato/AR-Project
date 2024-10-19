using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pet : MonoBehaviour
{
    [SerializeField]
    PetSO petData;
    public PetSO Data {get => petData; }

    [Header("Debug")]
    [SerializeField]
    private int maxFeed;
    [SerializeField]
    private int maxLove;
    [SerializeField]
    public int feed;
    [SerializeField]
    public int love;

    Animator animator;
    XRSimpleInteractable XR_interactable;

    private bool initiliazed = false;

    private bool isEating = false;

    public UnityAction<int, int> OnFeed;
    public UnityAction<int, int> OnLove;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
        if(initiliazed)
        {
            Debug.LogWarning("Pet data already initialized");
            return;
        }
        if (!petData)
        {
            Debug.LogWarning("No pet data available");
            return;
        }
        maxFeed = petData.MaxFeed;
        maxLove = petData.MaxLove;
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
        isEating = true;
        LookAt(food.gameObject);
        animator.SetInteger("AnimationID", 5);
        yield return new WaitForSeconds(food.Data.EatingTime);
        feed = Mathf.Clamp(feed + food.Data.FeedPoints, 0, maxFeed);
        love = Mathf.Clamp(love + food.Data.LovePoints, 0, maxLove);
        OnFeed?.Invoke(feed, maxFeed);
        OnLove?.Invoke(love, maxLove);
        Destroy(food.gameObject);
        animator.SetInteger("AnimationID", 1);
        isEating = false;
    }

    [ContextMenu("Pet")]
    private void Love(SelectEnterEventArgs xr_event)
    {
        love = Mathf.Clamp(love + 1, 0, maxLove);
        OnLove?.Invoke(love, maxLove);
        animator.SetInteger("AnimationID", 7);
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
            collision.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            Food food = collision.gameObject.GetComponent<Food>();

            food.Eat(this);
        }
    }

}
