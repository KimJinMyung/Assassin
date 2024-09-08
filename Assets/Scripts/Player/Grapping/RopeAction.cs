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
    private bool IsRotation;
    private bool IsGrapplingMove;

    private bool isRotating = false;  // 회전 중인지 여부를 체크하는 변수
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
            // GrapplingPoint까지의 방향 벡터 계산 (y축만 고려한 회전)
            Vector3 directionToGrapplingPoint = GrapplingPoint - playerMesh.transform.position;
            directionToGrapplingPoint.y = 0;  // y축 고정 (수평 회전만 고려)

            // 목표 회전 계산
            Quaternion targetRotation = Quaternion.LookRotation(directionToGrapplingPoint);
            float angleDifference = Quaternion.Angle(playerMesh.transform.rotation, targetRotation);

            Debug.Log($"Angle difference: {angleDifference}");

            // 각도 차이가 1도 이하가 되면 회전 종료
            if (angleDifference < 1f)
            {
                break;
            }

            // 현재 회전 방향과 목표 방향의 내적 값 계산 (Dot product)
            float dotProduct = Vector3.Dot(playerMesh.transform.forward, directionToGrapplingPoint.normalized);

            // 만약 목표가 캐릭터의 뒤에 있다면 (Dot product가 0보다 작음)
            if (dotProduct < 0 && angleDifference > 90f)
            {
                // 180도를 돌지 않도록 방향을 조절해 짧은 경로로 회전
                targetRotation = Quaternion.LookRotation(-playerMesh.transform.forward);
            }

            // 현재 회전에서 목표 회전으로 회전하는데, 한 프레임에 최대 300도까지 회전
            playerMesh.transform.rotation = Quaternion.RotateTowards(
                playerMesh.transform.rotation,
                targetRotation,
                Time.deltaTime * 300f  // 회전 속도 조절 (300도/초)
            );

            yield return null; // 한 프레임 대기
        }

        // 회전이 끝난 후 애니메이션과 이벤트 처리
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
    //            Debug.Log("그래플링 정지");
    //            yield break;
    //        }

    //        yield return null;

    //        // 라인 렌더러 위치 초기화
    //        lr.SetPosition(0, LeftHand.position);

    //        // 목표 지점으로의 방향 벡터 계산 (수평 이동만 고려)
    //        Vector3 direction = (GrapplingPoint - transform.position).normalized;
    //        direction.y = 0; // 수평 방향만 계산

    //        // 목표 지점까지의 남은 거리 계산
    //        float distanceToTarget = Vector3.Distance(transform.position, GrapplingPoint);

    //        // t 값 계산 (0에서 1 사이)
    //        float t = 1 - Mathf.Clamp01(distanceToTarget / totalDistance);

    //        // AnimationCurve에서 상대적 y축 높이 변화 값 얻기
    //        float heightOffset = heightCurve.Evaluate(t);

    //        // 속도 감소 계산 (AnimationCurve를 통해 t에 따른 속도 비율 계산)
    //        float speedMultiplier = speedCurve.Evaluate(t);
    //        float currentSpeed = grapplingSpeed * speedMultiplier;

    //        // 수평 이동 벡터 계산
    //        Vector3 moveHorizontal = direction * currentSpeed;

    //        // y축 위치 계산 (기본 y 이동 + 추가 y 이동)
    //        float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

    //        // 최종 이동 벡터
    //        Vector3 targetPosition = new Vector3(transform.position.x + moveHorizontal.x, newYPosition, transform.position.z + moveHorizontal.z);

    //        // Rigidbody의 MovePosition으로 이동 (정확한 경로를 따름)
    //        rigidbody.MovePosition(targetPosition);

    //        // 목적지에 도착했거나, 매우 가까워지면 그래플링 종료
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
                //플레이어가 매달리는 애니메이션
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

        float currentDistance = 0f; // 현재 이동한 거리
        float initialY = transform.position.y;

        while (true)
        {
            if (!IsGrappling)
            {
                StopGrapple();
                Debug.Log("그래플링 정지");
                yield break;
            }

            yield return null;

            // 현재까지 이동한 거리 계산
            currentDistance += grapplingSpeed * Time.deltaTime;

            // 이동 비율 t 계산 (0에서 1 사이)
            float t = Mathf.Clamp01(currentDistance / totalDistance);

            // AnimationCurve에서 y축 높이 변화 값 얻기
            float heightOffset = heightCurve.Evaluate(t);

            // 목표 지점으로의 방향 벡터 계산
            Vector3 direction = (GrapplingPoint - startPosition).normalized;

            // 수평 이동 벡터 계산 (t에 따른 이동 비율)
            Vector3 moveHorizontal = direction * grapplingSpeed * Time.deltaTime;

            // y축 위치 계산 (기본 y 이동 + 추가 y 이동)
            float newYPosition = Mathf.Lerp(initialY, GrapplingPoint.y, t) + heightOffset;

            // 최종 이동 벡터 계산
            Vector3 targetPosition = new Vector3(startPosition.x + direction.x * currentDistance, newYPosition, startPosition.z + direction.z * currentDistance);

            // Rigidbody의 MovePosition으로 이동
            rigidbody.MovePosition(targetPosition);

            lr.SetPosition(0, LeftHand.position);

            // 목표 지점에 도착하면 그래플링 종료
            if (t >= 1f)
            {
                IsGrappling = false;
                break;
            }
        }

        // 애니메이션 종료 및 그래플링 관련 처리
        IsGrapplingMove = false;
        StopGrapple();
    }


}
