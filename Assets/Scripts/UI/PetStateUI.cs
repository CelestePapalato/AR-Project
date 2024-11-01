using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetStateUI : MonoBehaviour
{
    [SerializeField]
    PetStats pet;

    [Header("UI")]
    [SerializeField]
    Slider feedSlider;
    [SerializeField]
    Slider waterSlider;
    [SerializeField]
    Slider loveSlider;
    [SerializeField]
    Slider pettingCooldownSlider;

    private void Start()
    {
        pettingCooldownSlider.value = 0;
    }

    private void OnEnable()
    {
        if(pet != null)
        {
            pet.Feed.OnUpdate += UpdateFeed;
            pet.Water.OnUpdate += UpdateWater;
            pet.Love.OnUpdate += UpdateLove;
            pet.OnPettingCooldownUpdate += UpdatePettingCooldown;
        }
    }

    private void OnDisable()
    {
        if(pet != null)
        {
            pet.Feed.OnUpdate -= UpdateFeed;
            pet.Water.OnUpdate -= UpdateWater;
            pet.Love.OnUpdate -= UpdateLove;
            pet.OnPettingCooldownUpdate -= UpdatePettingCooldown;
        }
    }

    private void UpdateLove(int love, int minLove, int maxLove)
    {
        loveSlider.value = (float) love/maxLove;
    }

    private void UpdateFeed(int feed, int minFeed, int maxFeed)
    {
        feedSlider.value = (float) feed/maxFeed;
    }
    private void UpdateWater(int feed, int minFeed, int maxFeed)
    {
        waterSlider.value = (float)feed / maxFeed;
    }

    private void UpdatePettingCooldown(float current, float startTime)
    {
        pettingCooldownSlider.value = current/startTime;
    }
}
