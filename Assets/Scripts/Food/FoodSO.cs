using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food SO", menuName = "Pet/Food", order = 0)]
public class FoodSO : ScriptableObject
{
    public int FeedPoints;
    public int WaterPoints;
    public int LovePoints;
    public float EatingTime;
}
