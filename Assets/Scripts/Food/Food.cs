using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField]
    FoodSO foodData;

    [Header("DEBUG")]
    [SerializeField]
    private int feedPoints;
    [SerializeField]
    private int lovePoints;

    void Start()
    {
        if(foodData == null)
        {
            Destroy(gameObject);
            return;
        }
        InitializeData(foodData);
    }

    void InitializeData(FoodSO food)
    {
        if (!food)
        {
            Debug.LogWarning("No food data");
            return;
        }
        feedPoints = foodData.feedPoints;
        lovePoints = foodData.lovePoints;
    }
}
