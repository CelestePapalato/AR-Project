using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [ContextMenu("Feed")]
    public void Feed(FoodSO food)
    {
        feed = Mathf.Clamp(feed + food.feedPoints, 0, maxFeed);
        love = Mathf.Clamp(love + food.lovePoints, 0, maxLove);
    }

    [ContextMenu("Pet")]
    public void Love()
    {
        love = Mathf.Clamp(love + 1, 0, maxLove);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Food food = collision.gameObject.GetComponent<Food>();
            food.Feed(this);
        }
    }
}
