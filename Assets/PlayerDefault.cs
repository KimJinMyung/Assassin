using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefault : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
    }
}
