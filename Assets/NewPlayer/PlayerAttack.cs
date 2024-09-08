using EventEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

namespace Player 
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform shakeCamera;

        [SerializeField] private LayerMask AttackTarget;
        [SerializeField] private float assassinationDistanceForward;
        [SerializeField] private float assassinationDistanceBack;

        private bool isShakeRotate;
        private bool isAttackAble;
        private bool isBattleMode;
        private bool isDefense;
        private bool isParryAble;
        private bool isHealing;

        private Vector3 originPos;
        private Quaternion originRotation;
        private Vector3 AssassinatedPos;

        private PlayerView playerMesh;
        private Animator animator;
        private PlayerLockOn playerSight;
        private Rigidbody rigidbody;

        private Coroutine BattleModeCheck;

        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashDefense = Animator.StringToHash("Defense");
        private readonly int hashDefenseStart = Animator.StringToHash("DefenseStart");
        private readonly int hashParry = Animator.StringToHash("Parry");
        private readonly int hashFront = Animator.StringToHash("isForward");
        private readonly int hashAssassinate = Animator.StringToHash("Assassinate");
        private readonly int hashAssassinated = Animator.StringToHash("Assassinated");

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            playerMesh = GetComponentInChildren<PlayerView>();
            playerSight = GetComponentInChildren<PlayerLockOn>();
            rigidbody = GetComponent<Rigidbody>();

            AddEvent();
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }

        private void AddEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ParryAble_PlayerAttack, SetParring);
            EventManager<PlayerAction>.Binding<MonsterView>(true, PlayerAction.AttackLockOnEnable, EnableLockOn);
            EventManager<CameraEvent>.Binding(true, CameraEvent.PlayerAttackSuccess, ShakeCamera);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ChangedBattleMode, SetBattleMode);
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.ParryAble_PlayerAttack, SetParring);
            EventManager<PlayerAction>.Binding<MonsterView>(false, PlayerAction.AttackLockOnEnable, EnableLockOn);
            EventManager<CameraEvent>.Binding(true, CameraEvent.PlayerAttackSuccess, ShakeCamera);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ChangedBattleMode, SetBattleMode);
        }

        private void OnEnable()
        {
            isAttackAble = true;
            isBattleMode = false;
            isDefense = false;
            isShakeRotate = false;
            isParryAble = true;
            isHealing = false;
        }

        private void Update()
        {
            if(!isBattleMode && BattleModeCheck == null)
            {
                BattleModeCheck = StartCoroutine(CheckHealDelay(3f));
            }else if(isBattleMode && BattleModeCheck != null)
            {
                StopCoroutine(BattleModeCheck);
                BattleModeCheck = null;


            }

            if(isBattleMode &&  isHealing)
            {
                isHealing = false;
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.RecoveryHP, false);
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRecoveryStamina);
            }
        }

        private IEnumerator CheckHealDelay(float delay) 
        {
            float elapsedTime = 0f;

            while (elapsedTime < delay)
            {
                if (isBattleMode)
                {
                    BattleModeCheck = null;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 3�� ���� isBattleMode�� false�� �� Heal ����
            Heal();
            BattleModeCheck = null;
        }

        private void Heal()
        {
            isHealing = true;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.RecoveryHP, true);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.RecoveryStamina);
        }

        private void SetAttackAble(bool isAttackAble)
        {
            this.isAttackAble = isAttackAble;
        }

        private void SetParring(bool isParring) 
        {
            this.isParryAble = isParring;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ParryAble_PlayerView, this.isParryAble);
        }

        private void SetBattleMode(bool isBattleMode)
        {
            this.isBattleMode = isBattleMode;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                if (!isAttackAble) return;

                if (!isBattleMode)
                    isBattleMode = true;

                if (!isDefense)
                {
                    SelectAttackAnimation();
                    //animator.SetTrigger(hashAttack);
                }                    
                else if(isParryAble)
                    animator.SetTrigger(hashParry);
            }
        }

        private void SelectAttackAnimation()
        {
            Transform hit = GetAssassinatedTarget();
            if (hit != null) 
            { 
                var target = hit.GetComponent<MonsterView>();
                if(target != null)
                {
                    var IsFront = Vector3.Dot(target.transform.forward, playerMesh.transform.forward) < 0.5f;
                    if (IsFront && (bool)target._behaviorTree.GetVariable("isSubded").GetValue())
                    {                        
                        AssassinatedPos = target.transform.position + target.transform.forward * assassinationDistanceForward;
                        MovePlayerToPosition(AssassinatedPos);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat(hashFront, 0);
                        
                        //���� �ϻ���ϴ� ��� ����
                        target.animator.SetFloat("Forward", 0);
                        target.vm.RequestTraceTargetChanged(target.monsterId, playerMesh.transform);

                        playerMesh.ViewModel.RequestAssassinatedType(target);
                        
                        animator.SetTrigger(hashAssassinate);
                        EnableLockOn(target);
                        return;
                    }
                    else if(!IsFront)
                    {
                        if (target.Type == MonsterType.Boss)
                        {
                            if (!(bool)target._behaviorTree.GetVariable("isSubded").GetValue())
                            {
                                characterRotate(target.transform.position);

                                //����
                                animator.SetTrigger("Attack");
                                return;
                            }
                        }

                        AssassinatedPos = target.transform.position - target.transform.forward * assassinationDistanceBack;
                        MovePlayerToPosition(AssassinatedPos);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat(hashFront, 1);

                        //���� �ϻ���ϴ� ��� �Ĺ�
                        target.animator.SetFloat("Forward", 1);
                        target.vm.RequestTraceTargetChanged(target.monsterId, playerMesh.transform);

                        playerMesh.ViewModel.RequestAssassinatedType(target);

                        animator.SetTrigger(hashAssassinate);
                        EnableLockOn(target);
                        return;
                    }

                }  
                

            }

            if(playerSight.ViewModel.LockOnAbleTarget != null)
            {
                var ViewMonster = playerSight.ViewModel.LockOnAbleTarget.GetComponent<MonsterView>();
                if (ViewMonster != null)
                {
                    characterRotate(ViewMonster.transform.position);
                }
            }           

            //����
            animator.SetTrigger(hashAttack);
        }

        private void EnableLockOn(MonsterView monster)
        {
            if(playerMesh.ViewModel.LockOnTarget == monster.transform)
            {
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsLockOn, false);
            }
        }

        private Transform GetAssassinatedTarget()
        {
            Collider[] hitColliders = Physics.OverlapSphere(playerMesh.transform.position + Vector3.up, 2f, AttackTarget);

            Transform closestObject = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hit in hitColliders)
            {
                var monster = hit.GetComponent<MonsterView>();
                if (monster == null)  continue;

                Vector3 directionToCollider = (hit.transform.position - playerMesh.transform.position).normalized;
                float angle = Vector3.Angle(playerMesh.transform.forward, directionToCollider);

                // ������ ���� ���� �ִ��� Ȯ��
                if (angle <= 45)
                {
                    float distance = Vector3.Distance(playerMesh.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = hit.transform;
                    }
                }
            }

            return closestObject;
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            isDefense = context.ReadValue<float>() > 0.5f;

            if (isDefense && !animator.GetBool(hashDefense))
            {
                animator.SetTrigger(hashDefenseStart);
            }
            animator.SetBool(hashDefense, isDefense);           

            EventManager<AttackBoxEvent>.TriggerEvent(AttackBoxEvent.IsDefense, isDefense);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsDefense, isDefense);
        }

        private void ShakeCamera()
        {
            originPos = shakeCamera.transform.localPosition;
            originRotation = shakeCamera.transform.localRotation;

            StartCoroutine(ShakingCamera());
        }

        IEnumerator ShakingCamera(float duration = 0.05f,
                                    float magnitudePos = 0.09f,
                                    float magnitudeRot = 0.1f)
        {
            float passTime = 0f;

            while(passTime < duration)
            {
                Vector3 shakePos = Random.insideUnitSphere;
                shakeCamera.localPosition += shakePos * magnitudePos;

                //if(isShakeRotate)
                //{
                //    Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                    
                //    shakeCamera.localRotation = Quaternion.Euler(shakeRot);
                //}

                passTime += Time.deltaTime;

                yield return null;  
            }

            shakeCamera.localPosition = originPos;
            shakeCamera.localRotation = originRotation;
        }

        private void MovePlayerToPosition(Vector3 targetPosition)
        {
            rigidbody.MovePosition(targetPosition);
        }

        private void AssassinatedRotation(Vector3 targetPos)
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.StopRotation, true);
            Vector3 direction = targetPos - playerMesh.transform.position;
            direction.y = 0f;
            playerMesh.transform.rotation = Quaternion.LookRotation(direction.normalized);
        }

        private void characterRotate(Vector3 targetPos)
        {
            //LockOnTarget�� �������� �ʾҴٸ� LockOnAbleTarget�� �ٶ󺸸� ����
            bool condition1 = playerMesh.ViewModel.LockOnTarget == null;
            //bool condition2 = ownerMovement.vm.Movement.magnitude < 0.1f;

            if (condition1 /*&& condition2*/)
            {
                Vector3 dirTarget = targetPos - playerMesh.transform.position;
                dirTarget.y = 0;
                Quaternion rotation = Quaternion.LookRotation(dirTarget);
                playerMesh.transform.rotation = Quaternion.Slerp(playerMesh.transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 100f * Time.fixedDeltaTime);
            }
        }
    }
}
