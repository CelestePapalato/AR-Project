using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Touchable : MonoBehaviour
{
    public UnityAction OnTouch;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouch?.Invoke();
    }
}
