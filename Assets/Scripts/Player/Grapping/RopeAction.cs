using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using EventEnum;
using static UnityEngine.UI.GridLayoutGroup;

public class RopeAction : MonoBehaviour
{
    private PlayerView playerMesh;
    private PlayerLockOn ownerViewZone;
    private Rigidbody rigidbody;
    private Animator animator;

    [Header("Grappling Move")]
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float grapplingSpeed = 30f;
    [SerializeField] private float stopDistance = 0.5f;

    [Header("Hook Move")]
    [SerializeField] private float waveFrequency = 2f; // ���̺� ��
    [SerializeField] private float waveAmplitude = 0.5f; // ���̺� ����

    [Header("Grappling StartPos")]
    [SerializeField] private Transform LeftHand;

    private LineRenderer lr;
    private Vector3 GrapplingPoint;
    private float grappleStartTime;

    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    protected readonly int hashIsGrappling = Animator.StringToHash("Grappling");
    protected readonly int hashPullGrappling = Animator.StringToHash("Pull");

    public bool IsGrappling { get; private set; }
    private bool IsRotation;
    private bool IsGrapplingMove;

    private bool isRotating = false;  // ȸ�� ������ ���θ� üũ�ϴ� ����
    private Quaternion targetRotation;

    int GrapplinglayerIndex = 0;

    private void Awake()
    {
        playerMesh = GetComponentInChildren<PlayerView>();
        ownerViewZone = GetComponentInChildren<PlayerLockOn>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        lr = GetComponentInChildren<LineRenderer>();

        AddEvent();

        GrapplinglayerIndex = animator.GetLayerIndex("Grappling");
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void OnEnable()
    {
        lr.enabled = false;

        animator.SetLayerWeight(GrapplinglayerIndex, 0);
    }

    private void OnDisable()
    {
        lr.enabled = false;
    }

    private void AddEvent()
    {
        EventManager<GrapplingEvent>.Binding(true, GrapplingEvent.GrapplingMove, StartGrappling);
        EventManager<GrapplingEvent>.Binding(true, GrapplingEvent.GrapplingPull, ShootGrapplingHook);
    }

    private void RemoveEvent()
    {
        EventManager<GrapplingEvent>.Binding(false, GrapplingEvent.GrapplingMove, StartGrappling);
        EventManager<GrapplingEvent>.Binding(false, GrapplingEvent.GrapplingPull, ShootGrapplingHook);
    }

    public void OnRopeMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartGrapple();
        }
    }

    private void StartGrapple()
    {
        if (ownerViewZone.ViewModel.LockOnAbleTarget == null) return;
        
        if (GetRopePoint(out GrapplingPoint))
        {
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;

            grappleStartTime = Time.time;
            IsGrappling = true;

            if (IsRotation) StopCoroutine(CharacterRotate());

            StartCoroutine(CharacterRotate());
        }
    }

    private IEnumerator CharacterRotate()
    {
        IsRotation = true;
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRotation, true);

        while (true)
        {
            // GrapplingPoint������ ���� ���� ��� (y�ุ ����� ȸ��)
            Vector3 directionToGrapplingPoint = GrapplingPoint - playerMesh.transform.position;
            directionToGrapplingPoint.y = 0;  // y�� ���� (���� ȸ���� ���)

            // ��ǥ ȸ�� ���
            Quaternion targetRotation = Quaternion.LookRotation(directionToGrapplingPoint);
            float angleDifference = Quaternion.Angle(playerMesh.transform.rotation, targetRotation);

            Debug.Log($"Angle difference: {angleDifference}");

            // ���� ���̰� 1�� ���ϰ� �Ǹ� ȸ�� ����
            if (angleDifference < 1f)
            {
                break;
            }

            // ���� ȸ�� ����� ��ǥ ������ ���� �� ��� (Dot product)
            float dotProduct = Vector3.Dot(playerMesh.transform.forward, directionToGrapplingPoint.normalized);

            // ���� ��ǥ�� ĳ������ �ڿ� �ִٸ� (Dot product�� 0���� ����)
            if (dotProduct < 0 && angleDifference > 90f)
            {
                // 180���� ���� �ʵ��� ������ ������ ª�� ��η� ȸ��
                targetRotation = Quaternion.LookRotation(-playerMesh.transform.forward);
            }

            // ���� ȸ������ ��ǥ ȸ������ ȸ���ϴµ�, �� �����ӿ� �ִ� 300������ ȸ��
            playerMesh.transform.rotation = Quaternion.RotateTowards(
                playerMesh.transform.rotation,
                targetRotation,
                Time.deltaTime * 300f  // ȸ�� �ӵ� ���� (300��/��)
            );

            yield return null; // �� ������ ���
        }

        // ȸ���� ���� �� �ִϸ��̼ǰ� �̺�Ʈ ó��
        animator.SetLayerWeight(GrapplinglayerIndex, 1);
        animator.SetBool(hashIsGrappling, true);

        IsRotation = false;
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRotation, false);
        yield break;
    }




    private bool GetRopePoint(out Vector3 targetPos)
    {
        if (ownerViewZone.ViewModel.LockOnAbleTarget.CompareTag("RopePoint"))
        {
            if (!ownerViewZone.ViewModel.HitColliders.Contains(ownerViewZone.ViewModel.LockOnAbleTarget))
            {
                targetPos = Vector3.zero;
                return false;
            }

            targetPos = ownerViewZone.ViewModel.LockOnAbleTarget.position;
            return true;
        }

        targetPos = Vector3.zero;
        return false;
    }

    public void StopGrapple()
    {
        IsGrapplingMove = false;

        //animator.SetLayerWeight(GrapplinglayerIndex, 0);
        animator.SetBool(hashIsGrappling, false);

        animator.applyRootMotion = false;

        IsGrappling = false;

        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
        rigidbody.useGravity = true;
        lr.enabled = false;
    }

    //private IEnumerator Grapping()
    //{
    //    IsGrapplingMove = true;

    //    Vector3 startPosition = transform.position;
    //    float totalDistance = Vector3.Distance(startPosition, GrapplingPoint);
    //    float initialY = transform.position.y;

    //    while (true)
    //    {
    //        if (!IsGrappling)
    //        {
    //            StopGrapple();
    //            Debug.Log("�׷��ø� ����");
    //            yield break;
    //        }

    //        yield return null;

    //        // ���� ������ ��ġ �ʱ�ȭ
    //        lr.SetPosition(0, LeftHand.position);

    //        // ��ǥ ���������� ���� ���� ��� (���� �̵��� ���)
    //        Vector3 direction = (GrapplingPoint - transform.position).normalized;
    //        direction.y = 0; // ���� ���⸸ ���

    //        // ��ǥ ���������� ���� �Ÿ� ���
    //        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

    //        // t �� ��� (0���� 1 ����)
    //        float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

    //        // AnimationCurve���� ����� y�� ���� ��ȭ �� ���
    //        float heightOffset = heightCurve.Evaluate(t);

    //        // �ӵ� ���� ��� (AnimationCurve�� ���� t�� ���� �ӵ� ���� ���)
    //        float speedMultiplier = speedCurve.Evaluate(t);
    //        float currentSpeed = grapplingSpeed * speedMultiplier;

    //        // ���� �̵� ���� ���
    //        Vector3 moveHorizontal = direction * currentSpeed;

    //        // y�� ��ġ ��� (�⺻ y �̵� + �߰� y �̵�)
    //        float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

    //        // ���� �̵� ����
    //        Vector3 targetPosition = new Vector3(transform.position.x + moveHorizontal.x, newYPosition, transform.position.z + moveHorizontal.z);

    //        // Rigidbody�� MovePosition���� �̵� (��Ȯ�� ��θ� ����)
    //        rigidbody.MovePosition(targetPosition);

    //        // �������� �����߰ų�, �ſ� ��������� �׷��ø� ����
    //        if (distanceToTarget <= stopDistance)
    //        {
    //            IsGrappling = false;
    //        }
    //    }
    //}

    private void StartGrappling()
    {
        StartCoroutine(Grapping());
    }

    private void ShootGrapplingHook()
    {
        StartCoroutine(HookShootAnimation());
    }

    IEnumerator HookShootAnimation()
    {
        float distance = Vector3.Distance(LeftHand.position, GrapplingPoint);
        float startTime = Time.time;

        lr.enabled = true;

        while (true)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed * 20f / distance;

            Vector3 currentPoint = Vector3.Lerp(LeftHand.position, GrapplingPoint, t);

            if (t >= 1f || Vector3.Distance(currentPoint, GrapplingPoint) <= 1f )
            {
                t = 1f;
                lr.SetPosition(1, GrapplingPoint);
                //�÷��̾ �Ŵ޸��� �ִϸ��̼�
                animator.SetTrigger(hashPullGrappling);

                if (IsGrapplingMove) break;
            }
            
            float wave = Mathf.Sin(t * waveFrequency * Mathf.PI) * waveAmplitude;
            currentPoint.y += wave;

            lr.SetPosition(0, LeftHand.position);
            lr.SetPosition(1, currentPoint);

            yield return null;
        }

        IsGrappling = true;
        EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
        yield break;
    }
    private IEnumerator Grapping()
    {
        IsGrapplingMove = true;

        Vector3 startPosition = transform.position;
        float totalDistance = Vector3.Distance(startPosition, GrapplingPoint);

        float currentDistance = 0f; // ���� �̵��� �Ÿ�
        float initialY = transform.position.y;

        while (true)
        {
            if (!IsGrappling)
            {
                StopGrapple();
                Debug.Log("�׷��ø� ����");
                yield break;
            }

            yield return null;

            // ������� �̵��� �Ÿ� ���
            currentDistance += grapplingSpeed * Time.deltaTime;

            // �̵� ���� t ��� (0���� 1 ����)
            float t = Mathf.Clamp01(currentDistance / totalDistance);

            // AnimationCurve���� y�� ���� ��ȭ �� ���
            float heightOffset = heightCurve.Evaluate(t);

            // ��ǥ ���������� ���� ���� ���
            Vector3 direction = (GrapplingPoint - startPosition).normalized;

            // ���� �̵� ���� ��� (t�� ���� �̵� ����)
            Vector3 moveHorizontal = direction * grapplingSpeed * Time.deltaTime;

            // y�� ��ġ ��� (�⺻ y �̵� + �߰� y �̵�)
            float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

            // ���� �̵� ���� ���
            Vector3 targetPosition = new Vector3(startPosition.x + direction.x * currentDistance, newYPosition, startPosition.z + direction.z * currentDistance);

            // Rigidbody�� MovePosition���� �̵�
            rigidbody.MovePosition(targetPosition);

            lr.SetPosition(0, LeftHand.position);

            // ��ǥ ������ �����ϸ� �׷��ø� ����
            if (t >= 1f)
            {
                IsGrappling = false;
                break;
            }
        }

        // �ִϸ��̼� ���� �� �׷��ø� ���� ó��
        IsGrapplingMove = false;
        StopGrapple();
    }


}
