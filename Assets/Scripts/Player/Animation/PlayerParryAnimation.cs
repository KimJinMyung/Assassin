using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryAnimation : StateMachineBehaviour
{
    private PlayerBattleManager owner;

    [SerializeField] float ParringTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.ParryAble_PlayerAttack, true);
        //animator.SetBool("ParryAble", false);
        //owner = animator.GetComponent<PlayerBattleManager>();
        //owner.isParried = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime <= ParringTime)
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.Parring, true);
        }
        else
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.Parring, false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.ParryAble_PlayerAttack, true);
        //owner.isParried = false;
    }
}
