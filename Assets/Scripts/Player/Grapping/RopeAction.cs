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
    [SerializeField] private float waveFrequency = 2f; // 웨이브 빈도
    [SerializeField] private float waveAmplitude = 0.5f; // 웨이브 진폭

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
        float initialY = transform.position.y;  // 초기 y축 위치 저장

        while (true)
        {
            if (!IsGrappling)
            {
                StopGrapple();
                yield break;
            }

            yield return null;

            // 라인 렌더러 위치 초기화
            lr.SetPosition(0, LeftHand.position);

            // 목표 지점으로의 방향 벡터 계산 (수평 이동만 고려)
            Vector3 direction = (GrapplingPoint - transform.position).normalized;
            direction.y = 0; // 수평 방향만 계산

            // 목표 지점까지의 남은 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

            // t 값 계산 (0에서 1 사이)
            float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

            // AnimationCurve에서 상대적 y축 높이 변화 값 얻기 (0에서 시작해서 높아졌다가 다시 0으로 돌아옴)
            float heightOffset = heightCurve.Evaluate(t);

            // 속도 감소 계산 (AnimationCurve를 통해 t에 따른 속도 비율 계산)
            float speedMultiplier = speedCurve.Evaluate(t);  // 속도 감소용 curve
            float currentSpeed = grapplingSpeed * speedMultiplier;

            // 수평 이동 벡터 계산
            Vector3 moveHorizontal = direction * currentSpeed * Time.deltaTime;

            // y축 위치 계산 (기본 y 이동 + 추가 y 이동)
            float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

            // 최종 이동 벡터
            Vector3 move = new Vector3(moveHorizontal.x, newYPosition - transform.position.y, moveHorizontal.z);

            // CharacterController를 사용해 이동
            characterController.Move(move);

            // 목적지에 도착했거나, 매우 가까워지면 그래플링 종료
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

        //플레이어가 매달리는 애니메이션
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
    //    float initialY = transform.position.y;  // 초기 y축 위치 저장

    //    while (true)
    //    {
    //        if (!IsGrappling)
    //        {
    //            StopGrapple();
    //            yield break;
    //        }

    //        yield return null;

    //        // 목표 지점으로의 방향 벡터 계산 (수평 이동만 고려)
    //        Vector3 direction = (GrapplingPoint - transform.position).normalized;
    //        direction.y = 0; // 수평 방향만 계산

    //        // 목표 지점까지의 남은 거리 계산
    //        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

    //        // t 값 계산 (0에서 1 사이)
    //        float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

    //        // AnimationCurve에서 상대적 y축 높이 변화 값 얻기 (0에서 시작해서 높아졌다가 다시 0으로 돌아옴)
    //        float heightOffset = heightCurve.Evaluate(t);

    //        // 최종 이동 벡터 계산
    //        Vector3 moveHorizontal = direction * grapplingSpeed * Time.deltaTime;
    //        float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset; // 기본 y 이동 + 추가 y 이동

    //        // 최종 이동 벡터
    //        Vector3 move = new Vector3(moveHorizontal.x, newYPosition - transform.position.y, moveHorizontal.z);

    //        // CharacterController를 사용해 이동
    //        characterController.Move(move);

    //        // 목적지에 도착했거나, 매우 가까워지면 그래플링 종료
    //        if (distanceToTarget <= stopDistance)
    //        {
    //            IsGrappling = false;
    //        }
    //    }
    //}




}
