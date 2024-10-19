using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pet SO", menuName = "Pet/Pet Data", order = 0)]
public class PetSO : ScriptableObject
{
    public int MaxFeed;
    public int MaxLove;
    public int MaxPettingCount;
    public float PettingCooldown;
}
