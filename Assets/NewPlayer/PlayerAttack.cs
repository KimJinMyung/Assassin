using EventEnum;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
        private bool isDie;

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
            EventManager<PlayerMVVM>.Binding<bool>(true, PlayerMVVM.IsDie, SetIsDie);
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.ParryAble_PlayerAttack, SetParring);
            EventManager<PlayerAction>.Binding<MonsterView>(false, PlayerAction.AttackLockOnEnable, EnableLockOn);
            EventManager<CameraEvent>.Binding(true, CameraEvent.PlayerAttackSuccess, ShakeCamera);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ChangedBattleMode, SetBattleMode);
            EventManager<PlayerMVVM>.Binding<bool>(false, PlayerMVVM.IsDie, SetIsDie);
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
                if (isBattleMode || isDie)
                {
                    BattleModeCheck = null;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 3초 동안 isBattleMode가 false일 때 Heal 실행
            Heal();
            BattleModeCheck = null;
        }

        private void Heal()
        {
            isHealing = true;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.RecoveryHP, true);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.RecoveryStamina);
        }

        private void SetIsDie(bool isDie)
        {
            if (this.isDie == isDie) return;

            this.isDie = isDie;
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
                if(isDie) return;
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
                        EnableLockOn(target);

                        // 플레이어 위치 및 방향 설정
                        AssassinatedPos = target.transform.position + target.transform.forward * assassinationDistanceForward;
                        MovePlayerToPosition(AssassinatedPos);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat(hashFront, 0);
                        
                        //몬스터 암살당하는 모션 전방
                        target.animator.SetFloat("Forward", 0);
                        target.vm.RequestTraceTargetChanged(target.monsterId, playerMesh.transform);

                        // 몬스터 위치 및 방향 설정
                        var monsterPos = playerMesh.transform.position + playerMesh.transform.forward * assassinationDistanceForward;
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedPosition, target.monsterId, target.transform.position);
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedRotation, target.monsterId, playerMesh.transform.position);
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.SetAssassinated, target.monsterId, true);

                        playerMesh.ViewModel.RequestAssassinatedType(target);
                        
                        animator.SetTrigger(hashAssassinate);                        
                        return;
                    }
                    else if(!IsFront)
                    {
                        if (target.Type == MonsterType.Boss)
                        {
                            if (!(bool)target._behaviorTree.GetVariable("isSubded").GetValue())
                            {
                                characterRotate(target.transform.position);

                                //공격
                                animator.SetTrigger("Attack");
                                return;
                            }
                        }

                        EnableLockOn(target);

                        AssassinatedPos = target.transform.position - target.transform.forward * assassinationDistanceBack;
                        MovePlayerToPosition(AssassinatedPos);
                        AssassinatedRotation(target.transform.position);

                        animator.SetFloat(hashFront, 1);

                        //몬스터 암살당하는 모션 후방
                        target.animator.SetFloat("Forward", 1);
                        target.vm.RequestTraceTargetChanged(target.monsterId, playerMesh.transform);

                        var monsterPos = playerMesh.transform.position + playerMesh.transform.forward * assassinationDistanceBack;
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedPosition, target.monsterId, monsterPos);
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedRotation, target.monsterId, playerMesh.transform.position);
                        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.SetAssassinated, target.monsterId, true);

                        playerMesh.ViewModel.RequestAssassinatedType(target);

                        animator.SetTrigger(hashAssassinate);
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

            //공격
            animator.SetTrigger(hashAttack);
        }

        private void EnableLockOn(MonsterView monster)
        {
            if(playerMesh.ViewModel.LockOnTarget == monster.transform)
            {
                if(monster.Type == MonsterType.Boss && monster.vm.LifeCount > 0) return;

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

                // 각도가 제한 내에 있는지 확인
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
            if (isDie) return;

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
            //rigidbody.MovePosition(targetPosition);
            transform.position = targetPosition;
            playerMesh.transform.forward = transform.forward;
        }

        private void AssassinatedRotation(Vector3 targetPos)
        {
            Vector3 direction = (targetPos - playerMesh.transform.position).normalized;
            playerMesh.transform.forward = direction;
        }

        private void characterRotate(Vector3 targetPos)
        {
            //LockOnTarget을 지정하지 않았다면 LockOnAbleTarget을 바라보며 공격
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
