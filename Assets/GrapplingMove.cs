using EventEnum;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GrapplingMove : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager<GrapplingEvent>.TriggerEvent(GrapplingEvent.GrapplingMove);
    }    
}
