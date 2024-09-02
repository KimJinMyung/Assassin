using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventEnum;

public class PlayerJump : StateMachineBehaviour
{
    private bool isJumping;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isJumping = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!isJumping && stateInfo.normalizedTime >= 0.14f)
        {
            // Player의 Jump 메서드 이벤트 실행
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.Jump);

            isJumping = true;
        }
    }
}
