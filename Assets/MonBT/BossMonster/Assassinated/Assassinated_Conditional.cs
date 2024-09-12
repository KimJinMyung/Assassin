using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

[TaskCategory("BossAssassinated")]
public class Assassinated_Conditional : Conditional
{
    [SerializeField] SharedBool isAssassinated;

    public override TaskStatus OnUpdate()
    {
        if(isAssassinated.Value)
        {
            return TaskStatus.Success;
        }
        else return TaskStatus.Failure;
    }
}
