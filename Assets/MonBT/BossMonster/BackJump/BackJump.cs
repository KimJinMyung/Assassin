using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossMonsterBackJump")]
public class BackJump : Action
{
    private MonsterView monsterView;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Animator animator;

    private Vector3 StartPoint;
    private Vector3 EndPoint;

    private float elapsedTime = 0f;

    [SerializeField] private AnimationCurve JumpPath;
    [SerializeField] private float duration = 1f;
    [SerializeField] private LayerMask ground;

    private readonly int hashJump = Animator.StringToHash("Jump");
    private readonly int hashIsAir = Animator.StringToHash("IsAir");
    private readonly int hashNextAction = Animator.StringToHash("NextAction");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
        rb = Owner.GetComponent<Rigidbody>();
    }

    public override void OnStart()
    {
        agent.isStopped = true;
        agent.enabled = false;
        rb.isKinematic = true;

        elapsedTime = 0f;
        StartPoint = Owner.transform.position;
        EndPoint = DecideEndPoint();

        animator.SetTrigger(hashJump);
    }

    private Vector3 DecideEndPoint()
    {
        if(Physics.Raycast(StartPoint + Vector3.up, -Owner.transform.forward, 
            out RaycastHit hit, 4f, LayerMask.GetMask("BossMonsterZoneWall")))
        {
            var Point = hit.point;
            Point.y = StartPoint.y;

            return Point;
        }

        var Pos = StartPoint + (Owner.transform.forward * -1f) * 4f;
        return Pos;
    }

    public override TaskStatus OnUpdate()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);

        if(progress < 0.2f && !animator.GetBool(hashIsAir))
        {
            animator.SetBool(hashIsAir, true);
        }

        // 점프 경로 계산
        float height = JumpPath.Evaluate(progress);
        Vector3 currentPosition = Vector3.Lerp(StartPoint, EndPoint, progress);
        currentPosition.y = StartPoint.y + height;

        Owner.transform.position = currentPosition;

        // 점프가 완료되었는지 확인
        if (progress >= 1f)
        {
            animator.SetBool(hashIsAir, false);
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        agent.enabled = true;
        agent.isStopped = false;
        rb.isKinematic = false;
    }
}
