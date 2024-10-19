using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetStateUI : MonoBehaviour
{
    [SerializeField]
    Pet pet;

    [Header("UI")]
    [SerializeField]
    Slider feedSlider;
    [SerializeField]
    Slider loveSlider;

    private void OnEnable()
    {
        if(pet != null)
        {
            pet.OnFeed += UpdateFeed;
            pet.OnLove += UpdateLove;
        }
    }

    private void OnDisable()
    {
        if(pet != null)
        {
            pet.OnFeed -= UpdateFeed;
            pet.OnLove -= UpdateLove;
        }
    }

    private void UpdateLove(int love, int maxLove)
    {
        loveSlider.value = (float) love/maxLove;
    }

    private void UpdateFeed(int feed, int maxFeed)
    {
        feedSlider.value = (float) feed/maxFeed;
    }
}
