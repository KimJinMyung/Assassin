using EventEnum;
using UnityEngine;

public class PlayerAttack_NextAttackAble : StateMachineBehaviour
{
    private AttackBox attackBox;
    //private bool IsAttackShaking;

    [SerializeField] 
    private float NextAttackAbleTime = 0.25f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //IsAttackShaking = false;
        //if (attackBox == null)
        //{
        //    attackBox = animator.GetComponentInChildren<AttackBox>();
        //}
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("isMoveAble", stateInfo.normalizedTime < 0.1f);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, stateInfo.normalizedTime >= 0.1f);

        if (stateInfo.normalizedTime >= NextAttackAbleTime)
        {
            //attackBox.enabled = false;
            //if (!IsAttackShaking)
            //{
            //    EventManager<CameraEvent>.TriggerEvent(CameraEvent.PlayerAttackSuccess);
            //    IsAttackShaking = true;
            //}

            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, true);
        }
        else
        {
            //attackBox.enabled = true;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.SetAttackAble, false);
        }
    }
}
