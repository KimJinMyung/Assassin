using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossBackJump")]
public class Falling : Action
{
    private CapsuleCollider collider;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [SerializeField] SharedAnimationCurve curve;
    [SerializeField] private float duration = 1f;

    [SerializeField] private LayerMask groundLayer;

    //private bool isAction;
    private float elapsedTime = 0f;

    private float height;
    private Vector3 startPoint;
    private Vector3 endPoint;

    public override void OnAwake()
    {
        collider = Owner.GetComponent<CapsuleCollider>();
        agent = Owner.GetComponent<NavMeshAgent>();
        rb = Owner.GetComponent<Rigidbody>();
    }

    private void PointsChanged()
    {
        startPoint = Owner.transform.position;
        endPoint = DecideEndPoint() + Owner.transform.forward * collider.radius;
    }

    private Vector3 DecideEndPoint()
    {
        float ZDis = -4f;

        while (true)
        {
            var point = Owner.transform.position + (Owner.transform.forward * ZDis) + Vector3.up * height;
            
            var a = (point - Owner.transform.position).normalized;
            a.y = 0;
            var b = (Owner.transform.position - point).normalized;
            b.x = 0;
            b.z = 0;
            var dir = a+b;

            float length = Mathf.Sqrt(height * height + ZDis * ZDis);
            if (Physics.Raycast(startPoint, dir, out RaycastHit hit, length, groundLayer))
            {
                if (!hit.transform.CompareTag("aaa"))
                {
                    return hit.point;
                }
            }

            ZDis = Mathf.Clamp(ZDis + Time.deltaTime, ZDis, 0);
        }
    }


    private void FindGroundYPos()
    {        
        if(Physics.Raycast(Owner.transform.position, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            var ground = hit.point;
            height = Mathf.Abs(Owner.transform.position.y - ground.y);
        }
        
    }

    public override void OnStart()
    {
        FindGroundYPos();
        PointsChanged();
    }

    public override void OnEnd()
    {
        elapsedTime = 0f;        
    }

    public override TaskStatus OnUpdate()
    {
        CurveMove();

        if (elapsedTime >= duration)
        {
            agent.enabled = true;
            rb.isKinematic = true;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void CurveMove()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / duration);

        // XZ ��鿡�� ���� ����
        Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, t);

        // Y ���� � ���� �߰�
        newPosition.y += curve.Value.Evaluate(t);

        // ���ο� ��ġ ����
        Owner.transform.position = newPosition;
    }
}
