using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

public class Pet : MonoBehaviour
{
    [SerializeField]
    private int maxFeed;
    [SerializeField]
    private int maxLove;

    [Header("Debug")]
    [SerializeField]
    public int feed;
    [SerializeField]
    public int love;

    Animator animator;

    private bool isEating = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
        feed = Mathf.Clamp(feed + food.Data.feedPoints, 0, maxFeed);
        love = Mathf.Clamp(love + food.Data.lovePoints, 0, maxLove);
        yield return new WaitForSeconds(food.Data.eatingTime);
        Destroy(food.gameObject);
        animator.SetInteger("AnimationID", 1);
        isEating = false;
    }

    [ContextMenu("Pet")]
    public void Love()
    {
        love = Mathf.Clamp(love + 1, 0, maxLove);
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
