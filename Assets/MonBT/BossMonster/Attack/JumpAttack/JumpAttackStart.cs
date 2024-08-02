using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class JumpAttackStart : Action   
{
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [SerializeField] SharedAnimationCurve Cureve;

    private float elapsedTime = 0f;

    private bool isAction;

    private Vector3 startPoint;
    private Vector3 endPoint;

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
        rb = Owner.GetComponent<Rigidbody>();
    }

    public override void OnStart()
    {
        PointsChanged();

        agent.enabled = false;
        rb.isKinematic = false;
    }

    private void PointsChanged()
    {
        startPoint = Owner.transform.position;
        var point = Owner.transform.forward;
        point.y = 0f;

        endPoint = Owner.transform.position + point.normalized + Vector3.up * 2f;
    }

    private void CurveMove()
    {
        elapsedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        float t = Mathf.Clamp01(elapsedTime);

        // XZ 평면에서 선형 보간
        Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, t);

        // Y 값에 곡선 값을 추가
        newPosition.y += Cureve.Value.Evaluate(t);

        // 새로운 위치 설정
        Owner.transform.position = newPosition;
    }

    public override TaskStatus OnUpdate()
    {
        CurveMove();

        if (elapsedTime >= 1f || isAction) return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        elapsedTime = 0f;
        isAction = false;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("aaa")) isAction = true;
    }
}
