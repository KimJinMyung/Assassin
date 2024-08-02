using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Battle")]
public class Circling : Action
{
    MonsterView monsterView;
    NavMeshAgent agent;
    Animator animator;

    [SerializeField] SharedBool isAttackAble;

    private float CirclingTimer;
    private float circlingDir;

    private int hashCircling = Animator.StringToHash("Circling");
    private int hashCirDir = Animator.StringToHash("CirclingDir");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        agent = Owner.GetComponent<NavMeshAgent>();
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        CirclingTimer = Random.Range(3f, 6f);
        circlingDir = Random.Range(0, 2) == 0 ? 1f : -1f;
    }

    public override TaskStatus OnUpdate()
    {
        if(CirclingTimer > 0)
        {
            CirclingTimer -= Time.deltaTime;
            animator.SetBool(hashCircling, true);
            return TaskStatus.Running;
        }

        //animator.SetBool(hashCircling, false);
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        animator.SetBool(hashCircling, false);
    }

    public override void OnFixedUpdate()
    {
        var VecToTarget = Owner.transform.position - monsterView.vm.TraceTarget.position;
        var rotatedPos = Quaternion.Euler(0, circlingDir * 20f * Time.fixedDeltaTime, 0) * VecToTarget;

        agent.Move(rotatedPos - VecToTarget);
        Owner.transform.rotation = Quaternion.LookRotation(-rotatedPos);
        animator.SetFloat(hashCirDir, circlingDir);
    }


}
