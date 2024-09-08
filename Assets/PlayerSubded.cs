using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using EventEnum;

public class PlayerSubded : StateMachineBehaviour
{
    PlayerView owner;

    private float Timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<PlayerView>();

        Timer = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Timer += Time.deltaTime;

        if(Timer >= 2.5f)
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SubdedStateEnd);
        }
    }
}
