using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmediateState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(AnimalPlayer.TRANSITION_STATE, false);
    }
}