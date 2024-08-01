using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class DecideAttackMethod : Action
{
    
    private int GetSubStateMachineCount(Animator animator, string stateMachineName)
    {
        int count = 0; 

        for (int i = 0; i< animator.layerCount; i++)
        {
            
        }
    }
}
