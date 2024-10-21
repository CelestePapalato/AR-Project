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
    Slider loveSlider;

    private void OnEnable()
    {
        if(pet != null)
        {
            pet.Feed.OnUpdate += UpdateFeed;
            pet.Love.OnUpdate += UpdateLove;
        }
    }

    private void OnDisable()
    {
        if(pet != null)
        {
            pet.Feed.OnUpdate -= UpdateFeed;
            pet.Love.OnUpdate -= UpdateLove;
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
}
