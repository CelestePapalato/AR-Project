using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pet SO", menuName = "Pet/Pet Data", order = 0)]
public class PetSO : ScriptableObject
{
    [Header("Hunger")]
    public int MaxFeed;
    public int FeedDecreaseRate;
    public int FeedDecreaseValue;
    [Header("Hydration")]
    public int MaxWater;
    public int WaterDecreaseRate;
    public int WaterDecreaseValue;
    [Header("Love")]
    public int MaxLove;
    public int LoveDecreaseRate;
    public int LoveDecreaseValue;
    [Header("Petting")]
    public int pettingLovePoints;
    public int MaxPettingCount;
    public float PettingCooldown;
}
