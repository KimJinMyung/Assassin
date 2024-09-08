using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAssassinated : StateMachineBehaviour
{
    private readonly int hashIsFront = Animator.StringToHash("isForward");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
        animator.SetFloat(hashIsFront, 0);
    }
}
