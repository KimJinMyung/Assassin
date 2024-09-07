using EventEnum;
using UnityEngine;

public class GrapplingMove : StateMachineBehaviour
{
    private readonly int hashPull = Animator.StringToHash("Pull");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(hashPull);
        EventManager<GrapplingEvent>.TriggerEvent(GrapplingEvent.GrapplingMove);
    }    
}
