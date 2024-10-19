using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food SO", menuName = "Pet/Food", order = 0)]
public class FoodSO : ScriptableObject
{
    public int feedPoints;
    public int lovePoints;
    public GameObject prefab;
}
