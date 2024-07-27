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

        // BoxCast�� ��ġ�� ũ��, ������ �����մϴ�.
        Vector3 boxCenter = transform.position + Vector3.up;
        Vector3 boxHalfExtents = new Vector3(0.3f, 0.5f, 1f);
        Vector3 castDirection = owner.transform.forward;
        Quaternion rotation = transform.rotation;
        float castDistance = 1f;

        // BoxCast�� �ð�ȭ�մϴ�.
        Gizmos.color = Color.red;

        // BoxCast�� ��ȯ ��Ʈ������ �����մϴ�.
        Matrix4x4 boxMatrix = Matrix4x4.TRS(boxCenter + castDirection * castDistance * 0.5f, rotation, Vector3.one);
        Gizmos.matrix = boxMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        // RaycastHit�� �ð�ȭ�մϴ�.
        RaycastHit hit;
        if (Physics.BoxCast(boxCenter, boxHalfExtents, castDirection, out hit, rotation, castDistance, AttackTarget))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hit.point, 0.1f); // �浹 ������ ǥ���մϴ�.
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
                //�и�
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
                    isForward = Vector3.Dot(target.transform.forward, transform.forward) < 0.5f;    // true�� ����

                    if (isForward && (bool)target._behaviorTree.GetVariable("isSubded").GetValue())
                    {
                        Vector3 targetPosition = target.transform.position + target.transform.forward * assassinationDistanceForward;
                        MovePlayerToPosition(targetPosition);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat("isForward", 0);
                        animator.SetBool("Assassinated", true);
                        animator.SetTrigger("Assassinate");

                        //���� �ϻ���ϴ� ��� ����
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

                        //���� �ϻ���ϴ� ��� �Ĺ�
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

                    isForward = Vector3.Dot(ViewMonster.transform.forward, transform.forward) < 0.5f;    // true�� ����

                    if (isForward)
                    {
                        //�ڷ� ���� �ϻ����
                    }
                    else
                    {
                        //������ ���� �ϻ����
                    }
                }

                //LockOnTarget�� �������� �ʾҴٸ� LockOnAbleTarget�� �ٶ󺸸� ����
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

            //����
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
        direction.y = 0; // Y�� ������ �̵��� �����Ͽ� ���� �̵��� �̷�������� ��
        playerController.Move(direction);
    }

    private void AssassinatedRotation(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
