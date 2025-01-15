using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimationBehaviour : StateMachineBehaviour
{
    [SerializeField]
    bool petCanMove = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("should_move", petCanMove);
    }
}
