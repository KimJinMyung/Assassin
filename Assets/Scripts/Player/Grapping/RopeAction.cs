using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Temp;

public class RopeAction : MonoBehaviour
{
    private PlayerMovement ownerMovement;
    private PlayerLockOn ownerViewZone;
    private CharacterController characterController;
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
    private bool isShootHook;

    private void Awake()
    {
        ownerMovement = GetComponent<PlayerMovement>();
        ownerViewZone = GetComponent<PlayerLockOn>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        lr = GetComponentInChildren<LineRenderer>();
    }

    private void OnEnable()
    {
        lr.enabled = false;
    }

    private void OnDisable()
    {
        lr.enabled = false;
    }

    private void LateUpdate()
    {
        if (IsGrappling)
        {
            
        }
    }

    public void OnRopeMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //if (animator.GetBool(hashIsGrappling)) StopGrapple();
            //else StartGrapple();
            StartGrapple();
        }
    }

    private void StartGrapple()
    {
        if (ownerViewZone.ViewModel.LockOnAbleTarget == null) return;
        
        if (GetRopePoint(out GrapplingPoint))
        {
            ownerMovement.isGravityAble = false;
            grappleStartTime = Time.time;
            IsGrappling = true;

            CharacterRotate();

            int layerIndex = animator.GetLayerIndex("Grappling");
            ////owner.Animator.SetLayerWeight(layerIndex, 1);
            animator.SetBool(hashIsMoveAble, true);
            animator.SetBool(hashIsGrappling, true);

            StartCoroutine(HookShootAnimation());
        }
    }

    private void CharacterRotate()
    {
        Vector3 directionToGrapplingPoint = GrapplingPoint - transform.position;
        directionToGrapplingPoint.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToGrapplingPoint);
        transform.rotation = targetRotation;
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
        animator.SetBool(hashIsGrappling, false);

        animator.applyRootMotion = false;
        animator.SetBool(hashIsMoveAble, true);

        isShootHook = false;
        IsGrappling = false;

        ownerMovement.isGravityAble = true;
        lr.enabled = false;
    }

    private IEnumerator Grapping()
    {
        Vector3 startPosition = transform.position;
        float totalDistance = Vector3.Distance(startPosition, GrapplingPoint);
        float initialY = transform.position.y;  // �ʱ� y�� ��ġ ����

        while (true)
        {
            if (!IsGrappling)
            {
                StopGrapple();
                yield break;
            }

            yield return null;

            // ���� ������ ��ġ �ʱ�ȭ
            lr.SetPosition(0, LeftHand.position);

            // ��ǥ ���������� ���� ���� ��� (���� �̵��� ���)
            Vector3 direction = (GrapplingPoint - transform.position).normalized;
            direction.y = 0; // ���� ���⸸ ���

            // ��ǥ ���������� ���� �Ÿ� ���
            float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

            // t �� ��� (0���� 1 ����)
            float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

            // AnimationCurve���� ����� y�� ���� ��ȭ �� ��� (0���� �����ؼ� �������ٰ� �ٽ� 0���� ���ƿ�)
            float heightOffset = heightCurve.Evaluate(t);

            // �ӵ� ���� ��� (AnimationCurve�� ���� t�� ���� �ӵ� ���� ���)
            float speedMultiplier = speedCurve.Evaluate(t);  // �ӵ� ���ҿ� curve
            float currentSpeed = grapplingSpeed * speedMultiplier;

            // ���� �̵� ���� ���
            Vector3 moveHorizontal = direction * currentSpeed * Time.deltaTime;

            // y�� ��ġ ��� (�⺻ y �̵� + �߰� y �̵�)
            float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

            // ���� �̵� ����
            Vector3 move = new Vector3(moveHorizontal.x, newYPosition - transform.position.y, moveHorizontal.z);

            // CharacterController�� ����� �̵�
            characterController.Move(move);

            // �������� �����߰ų�, �ſ� ��������� �׷��ø� ����
            if (distanceToTarget <= stopDistance)
            {
                IsGrappling = false;
            }
        }
    }

    IEnumerator HookShootAnimation()
    {
        float distance = Vector3.Distance(LeftHand.position, GrapplingPoint);
        float startTime = Time.time;

        isShootHook = true;
        lr.enabled = true;

        while (true)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed * 20f / distance;

            if (t >= 1f)
            {
                t = 1f;
                break;
            }

            Vector3 currentPoint = Vector3.Lerp(LeftHand.position, GrapplingPoint, t);
            float wave = Mathf.Sin(t * waveFrequency * Mathf.PI) * waveAmplitude;
            currentPoint.y += wave;

            lr.SetPosition(0, LeftHand.position);
            lr.SetPosition(1, currentPoint);

            yield return null;
        }

        lr.SetPosition(1, GrapplingPoint);

        //�÷��̾ �Ŵ޸��� �ִϸ��̼�
        animator.SetTrigger(hashPullGrappling);

        IsGrappling = true;
        ownerMovement.isGravityAble = true;

        StartCoroutine(Grapping());
        yield break;
    }

    //private IEnumerator Grapping()
    //{
    //    Vector3 startPosition = transform.position;
    //    float totalDistance = Vector3.Distance(startPosition, GrapplingPoint);
    //    float initialY = transform.position.y;  // �ʱ� y�� ��ġ ����

    //    while (true)
    //    {
    //        if (!IsGrappling)
    //        {
    //            StopGrapple();
    //            yield break;
    //        }

    //        yield return null;

    //        // ��ǥ ���������� ���� ���� ��� (���� �̵��� ���)
    //        Vector3 direction = (GrapplingPoint - transform.position).normalized;
    //        direction.y = 0; // ���� ���⸸ ���

    //        // ��ǥ ���������� ���� �Ÿ� ���
    //        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

    //        // t �� ��� (0���� 1 ����)
    //        float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

    //        // AnimationCurve���� ����� y�� ���� ��ȭ �� ��� (0���� �����ؼ� �������ٰ� �ٽ� 0���� ���ƿ�)
    //        float heightOffset = heightCurve.Evaluate(t);

    //        // ���� �̵� ���� ���
    //        Vector3 moveHorizontal = direction * grapplingSpeed * Time.deltaTime;
    //        float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset; // �⺻ y �̵� + �߰� y �̵�

    //        // ���� �̵� ����
    //        Vector3 move = new Vector3(moveHorizontal.x, newYPosition - transform.position.y, moveHorizontal.z);

    //        // CharacterController�� ����� �̵�
    //        characterController.Move(move);

    //        // �������� �����߰ų�, �ſ� ��������� �׷��ø� ����
    //        if (distanceToTarget <= stopDistance)
    //        {
    //            IsGrappling = false;
    //        }
    //    }
    //}




}
