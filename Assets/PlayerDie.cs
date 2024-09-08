using System.Collections;
using EventEnum;
using Player;
using UnityEngine;

public class PlayerDie : StateMachineBehaviour
{
    PlayerView owner;
    private bool isResurrection;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<PlayerView>();
        isResurrection = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f && !isResurrection)
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.Resurrection);
            isResurrection = true;
        }
    }
}
