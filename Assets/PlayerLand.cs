using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLand : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
    }
}
