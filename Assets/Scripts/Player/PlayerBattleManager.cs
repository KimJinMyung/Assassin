using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerBattleManager : MonoBehaviour
{
    private PlayerView owner;
    private PlayerMovement ownerMovement;
    private Animator animator;

    private PlayerLockOn playerSight;

    [SerializeField] private LayerMask AttackTarget;

    [SerializeField] private float assassinationDistanceForward;
    [SerializeField] private float assassinationDistanceBack;

    public bool IsUpperPlayerToMonster { get; private set; }
    private MonsterView ViewMonster;
    private CharacterController playerController;

    public bool isUpper { get; private set; }
    public bool isForward { get; private set; }

    public bool isParried {  get; set; }
    private bool isDefence;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        ownerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerSight = GetComponent<PlayerLockOn>();
    }

    private void Update()
    {
        if (playerSight.ViewModel.LockOnAbleTarget == null) return;
        ViewMonster = playerSight.ViewModel.LockOnAbleTarget.GetComponent<MonsterView>();

        if (ViewMonster == null) return;
        if ((owner.transform.position.y - (ViewMonster.transform.position.y + ViewMonster.MonsterHeight)) > 0f) IsUpperPlayerToMonster = true;
        else IsUpperPlayerToMonster = false;
    }
    private void OnDrawGizmos()
    {
        if (owner == null)
            return;

        // BoxCast의 위치와 크기, 방향을 정의합니다.
        Vector3 boxCenter = transform.position + Vector3.up;
        Vector3 boxHalfExtents = new Vector3(0.3f, 0.5f, 1f);
        Vector3 castDirection = owner.transform.forward;
        Quaternion rotation = transform.rotation;
        float castDistance = 1f;

        // BoxCast를 시각화합니다.
        Gizmos.color = Color.red;

        // BoxCast의 변환 매트릭스를 설정합니다.
        Matrix4x4 boxMatrix = Matrix4x4.TRS(boxCenter + castDirection * castDistance * 0.5f, rotation, Vector3.one);
        Gizmos.matrix = boxMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        // RaycastHit을 시각화합니다.
        RaycastHit hit;
        if (Physics.BoxCast(boxCenter, boxHalfExtents, castDirection, out hit, rotation, castDistance, AttackTarget))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hit.point, 0.1f); // 충돌 지점을 표시합니다.
            Debug.Log(hit.transform.name);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (owner.ViewModel.HP <= 0) return;
            if (!animator.GetBool("AttackAble")) return;

            if (isDefence)
            {
                //패링
                if (animator.GetBool("ParryAble"))
                {
                    //animator.SetBool("Parring", true);
                    animator.SetTrigger("Parry");
                }
                return;
            }
            else if (Physics.BoxCast(transform.position + Vector3.up, new Vector3(0.3f, 0.5f, 1f), owner.transform.forward, out RaycastHit hit, transform.rotation, 1f, AttackTarget))
            {
                Debug.Log(hit.transform.name + "dddddd");

                MonsterView target = hit.transform.GetComponent<MonsterView>();
                if(target != null)
                {
                    isForward = Vector3.Dot(target.transform.forward, transform.forward) < 0.5f;    // true면 전방

                    if (isForward && (bool)target._behaviorTree.GetVariable("isSubded").GetValue())
                    {
                        Vector3 targetPosition = target.transform.position + target.transform.forward * assassinationDistanceForward;
                        MovePlayerToPosition(targetPosition);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat("isForward", 0);
                        animator.SetBool("Assassinated", true);
                        animator.SetTrigger("Assassinate");

                        //몬스터 암살당하는 모션 전방
                        target.animator.SetFloat("Forward", 0);
                        target.animator.SetBool("Incapacitated", true);
                        target.animator.SetTrigger("Assassinated");
                        return;
                    }
                    else if (!isForward)
                    {
                        Vector3 targetPosition = target.transform.position - target.transform.forward * assassinationDistanceBack;
                        MovePlayerToPosition(targetPosition);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat("isForward", 1);
                        animator.SetBool("Assassinated", true);
                        animator.SetTrigger("Assassinate");

                        //몬스터 암살당하는 모션 후방
                        target.animator.SetFloat("Forward", 1);
                        target.animator.SetBool("Incapacitated", true);
                        target.animator.SetTrigger("Assassinated");
                        return;
                    }
                }
            }            
            
            if (ViewMonster != null)
            {
                float distanceY = transform.position.y - ViewMonster.transform.position.y;

                if(distanceY > ViewMonster.MonsterHeight + 1)
                {
                    animator.SetBool("isUpper", true);
                    animator.SetBool("Assassinated", true);
                    animator.SetTrigger("Assassinate");

                    isForward = Vector3.Dot(ViewMonster.transform.forward, transform.forward) < 0.5f;    // true면 전방

                    if (isForward)
                    {
                        //뒤로 누워 암살당함
                    }
                    else
                    {
                        //앞으로 누워 암살당함
                    }
                }

                //LockOnTarget을 지정하지 않았다면 LockOnAbleTarget을 바라보며 공격
                bool condition1 = owner.ViewModel.LockOnTarget == null;
                bool condition2 = ownerMovement.vm.Movement.magnitude < 0.1f;

                if (condition1 && condition2)
                {
                    Vector3 dirTarget = ViewMonster.transform.position - transform.position;
                    dirTarget.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(dirTarget);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 100f * Time.fixedDeltaTime);
                }
            }

            //공격
            animator.SetTrigger("Attack");
        }
    }

    public void OnDefense(InputAction.CallbackContext context)
    {
        if (!animator.GetBool("AttackAble")) return;
        isDefence = context.ReadValue<float>() > 0.5f;

        if (isDefence && !animator.GetBool("Defense"))
        {
            animator.SetTrigger("DefenseStart");
        }

        animator.SetBool("Defense", isDefence);
    }

    private void MovePlayerToPosition(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Y축 방향의 이동을 방지하여 수평 이동만 이루어지도록 함
        playerController.Move(direction);
    }

    private void AssassinatedRotation(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
