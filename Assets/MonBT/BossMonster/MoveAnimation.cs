using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

[TaskCategory("BossMove")]
public class MoveAnimation : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent; 

    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAttacking;

    private float MovementValue_x;
    private float MovementValue_z;

    private static int hashMoveSpeed_x = Animator.StringToHash("MoveSpeed_X");
    private static int hashMoveSpeed_z = Animator.StringToHash("MoveSpeed_Z");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 velocity = agent.velocity;


        if(!isHurt.Value && !isDead.Value && !isAttacking.Value && monsterView.vm.TraceTarget != null)
        {
            MovementValue_x = velocity.x;
            MovementValue_z = velocity.z;
        }else
        {
            MovementValue_x = 0;
            MovementValue_z = 0;
        }

        animator.SetFloat(hashMoveSpeed_x, MovementValue_x);
        animator.SetFloat(hashMoveSpeed_z, MovementValue_z);

        return TaskStatus.Running;
    }

    //Vector3 dir = target.position - owner.transform.position;
    //    dir.y = 0;
    //            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
    //    owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, 3f * Time.deltaTime);

    //            Vector3 velocity = owner.Agent.velocity;
    //    MovementValue_x = velocity.x;
    //            MovementValue_z = velocity.z;

    //    MovementValue_x = 0;
    //    MovementValue_z = 0;


    //owner.animator.SetFloat(hashMoveSpeed_x, MovementValue_x);
    //owner.animator.SetFloat(hashMoveSpeed_z, MovementValue_z);
}
