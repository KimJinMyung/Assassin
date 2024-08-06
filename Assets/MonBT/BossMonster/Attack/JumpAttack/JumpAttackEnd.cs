using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class JumpAttackEnd : Action
{
    private MonsterView monsterView;

    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private string attackTypeName;
    private int attackIndex;

    private float elapsedTime = 0f;
    private bool isAction;
    private bool isAttackEnd;

    [SerializeField] SharedAnimationCurve Curve;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float distance = 1f;

    private int hashNextAction = Animator.StringToHash("NextAction");
    private int hashJumpping = Animator.StringToHash("Jumpping"); 
    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");
    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
        rb = Owner.GetComponent<Rigidbody>();
    }

    public override void OnStart()
    {
        switch (animator.GetInteger(hashAttackTypeIndex))
        {
            case 0:
                attackTypeName = "JumpAttack";
                break; 
            case 1:
                attackTypeName = "ComBoAttack";
                break;
            case 2:
                attackTypeName = "DashAttack";
                break;
        }

        attackIndex = animator.GetInteger(hashAttackIndex);

        AttackPoint();        
    }

    private void AttackPoint()
    {
        startPoint = Owner.transform.position;
        var Target = monsterView.vm.TraceTarget;
        var Point = Target.position;

        var dir = (Owner.transform.position - Point);
        dir.y = 0;
        dir.Normalize();
        endPoint = Point + dir * distance;
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsAnimationRunning($"{attackTypeName}.attack1{attackIndex}"))
        {
            return TaskStatus.Running;
        }

        if (monsterView.IsAnimationRunning($"{attackTypeName}.attack2{attackIndex}"))
        {
            isAttackEnd = true;
            return TaskStatus.Running;
        }

        if (isAttackEnd) 
            return TaskStatus.Success;

        if (!isAction)
        {
            animator.SetTrigger(hashNextAction);
            isAction = true;
        }
        
        CurveMove();

        var distance = Vector3.Distance(Owner.transform.position, endPoint);
        if(distance <= 0.1f)
        {
            animator.SetBool(hashJumpping, false);
            agent.enabled = true;
            rb.isKinematic = true;
            //return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void CurveMove()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / duration);

        // XZ 평면에서 선형 보간
        Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, t);

        // Y 값에 곡선 값을 추가
        newPosition.y += Curve.Value.Evaluate(t);

        // 새로운 위치 설정
        Owner.transform.position = newPosition;
    }

    public override void OnEnd()
    {
        elapsedTime = 0f;
        isAction = false;
        isAttackEnd = false;
    }
}
