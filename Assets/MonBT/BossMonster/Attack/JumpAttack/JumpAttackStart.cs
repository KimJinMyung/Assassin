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
    [SerializeField] float duration;

    private float elapsedTime = 0f;

    private bool isAction;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private int hashJumping = Animator.StringToHash("Jumpping");

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
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime/ duration);

        // XZ ��鿡�� ���� ����
        Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, t);

        // Y ���� � ���� �߰�
        newPosition.y += Cureve.Value.Evaluate(t);

        // ���ο� ��ġ ����
        Owner.transform.position = newPosition;
    }

    public override TaskStatus OnUpdate()
    {
        if (animator.GetBool(hashJumping))
        {
            CurveMove();
        }
        

        if (elapsedTime >= duration || isAction) return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        elapsedTime = 0f;
        isAction = false;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("BossMonsterZoneWall")) isAction = true;
    }
}
