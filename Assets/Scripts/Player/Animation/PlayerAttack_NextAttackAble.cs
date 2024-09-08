using EventEnum;
using Player;
using UnityEngine;

public class PlayerAttack_NextAttackAble : StateMachineBehaviour
{
    private PlayerAttackBox attackBox;
    //private bool IsAttackShaking;

    [SerializeField] 
    private float NextAttackAbleTime = 0.25f;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("isMoveAble", stateInfo.normalizedTime < 0.1f);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, stateInfo.normalizedTime >= 0.1f);

        if (stateInfo.normalizedTime >= NextAttackAbleTime)
        {
            EventManager<AttackBoxEvent>.TriggerEvent(AttackBoxEvent.IsAttacking, false);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
        }
        else
        {
            //attackBox.enabled = true;
            EventManager<AttackBoxEvent>.TriggerEvent(AttackBoxEvent.IsAttacking, true);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, false);
        }
    }
}
