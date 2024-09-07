using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootGrapplingHook : StateMachineBehaviour
{
    private bool isShootHook;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isShootHook = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.5f && !isShootHook)
        {
            isShootHook = true;
            EventManager<GrapplingEvent>.TriggerEvent(GrapplingEvent.GrapplingPull);
        }
    }
}
