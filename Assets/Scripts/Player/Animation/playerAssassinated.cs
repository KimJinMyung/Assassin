using EventEnum;
using UnityEngine;

public class playerAssassinated : StateMachineBehaviour
{
    private readonly int hashIsFront = Animator.StringToHash("isForward");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRotation, true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRotation, false);
        animator.SetFloat(hashIsFront, 0);
    }
}
