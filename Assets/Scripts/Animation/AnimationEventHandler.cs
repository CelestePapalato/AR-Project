using System;
using UnityEngine;

public enum CharacterAnimatorState
{
    MOVE,
    STOP
}

public class AnimationEventHandler : MonoBehaviour
{
    public event Action<CharacterAnimatorState> onAnimationEvent;

    public void EventHandler(CharacterAnimatorState state)
    {
        onAnimationEvent?.Invoke(state);
    }
}
