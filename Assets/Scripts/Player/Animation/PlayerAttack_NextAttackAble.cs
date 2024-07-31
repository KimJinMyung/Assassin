using UnityEngine;

public class PlayerAttack_NextAttackAble : StateMachineBehaviour
{
    private AttackBox attackBox;
    private Collider coliider;

    [SerializeField] 
    private float NextAttackAbleTime = 0.25f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", true);

        if(attackBox == null)
        {
            attackBox = animator.GetComponentInChildren<AttackBox>();
        }       
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", stateInfo.normalizedTime < 0.1f);

        if (stateInfo.normalizedTime >= NextAttackAbleTime)
        {
            attackBox.enabled = false;
            animator.SetBool("AttackAble", true);
        }
        else
        {
            attackBox.enabled = true;
            animator.SetBool("AttackAble", false);
        }

    }
}
