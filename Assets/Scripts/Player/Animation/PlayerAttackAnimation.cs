using EventEnum;
using UnityEngine;

public class PlayerAttackAnimation : StateMachineBehaviour
{
    private int _attackCount = 0;

    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackCount = animator.GetInteger(hashAttackIndex);
        _attackCount = (_attackCount + 1) % 4;

        animator.SetInteger(hashAttackIndex, _attackCount);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, false);

        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);

        animator.applyRootMotion = true;
        //animator.SetBool("AttackAble", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<CameraEvent>.TriggerEvent(CameraEvent.UpdateCameraPosition);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(hashAttack);
        //animator.SetBool("isMoveAble", true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        _attackCount = 0;
        animator.SetInteger(hashAttackIndex, 0);
        animator.ResetTrigger(hashAttack);
        //animator.SetBool("AttackAble", true);

        EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);

        animator.applyRootMotion = false;
    }
}
