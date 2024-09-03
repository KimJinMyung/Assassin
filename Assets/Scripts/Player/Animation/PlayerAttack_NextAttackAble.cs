using EventEnum;
using UnityEngine;

public class PlayerAttack_NextAttackAble : StateMachineBehaviour
{
    private AttackBox attackBox;

    [SerializeField] 
    private float NextAttackAbleTime = 0.25f;

    //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (attackBox == null)
    //    {
    //        attackBox = animator.GetComponentInChildren<AttackBox>();
    //    }
    //}

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("isMoveAble", stateInfo.normalizedTime < 0.1f);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, stateInfo.normalizedTime >= 0.1f);

        if (stateInfo.normalizedTime >= NextAttackAbleTime)
        {
            //attackBox.enabled = false;
            Debug.Log("공격 가능");

            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
        }
        else
        {
            //attackBox.enabled = true;

            Debug.Log("공격 불가능");
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, false);
        }
    }
}
