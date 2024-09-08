using System.ComponentModel;
using System.Collections;
using UnityEngine;
using EventEnum;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        public PlayerData playerData { get; private set; }
        private PlayerViewModel vm;
        public PlayerViewModel ViewModel { get { return vm; } }

        private Animator animator;

        private Vector3 attackerPosition;
        private float knockbackPower;
        [SerializeField] private float knockbackTime;
        private float defaultKnockbackTime;
        [SerializeField] private float SubdedTime;
        private float defaultSubdedTime;

        public bool isKnockback;
        private bool isDefense;
        private bool isParring;
        private bool isParryAble;
        private bool isDie;
        public bool isSubded { get; private set; }

        private Coroutine staminaRecoveryCoroutine;
        private Coroutine HpRecoveryCoroutine;

        private readonly int hashIncapacitated = Animator.StringToHash("Incapacitated");
        private readonly int hashIncapacitate = Animator.StringToHash("Incapacitate");
        private readonly int hashDie = Animator.StringToHash("Die");
        private readonly int hashDead = Animator.StringToHash("Dead");

        private void Awake()
        {
            animator = GetComponent<Animator>();

            defaultKnockbackTime = knockbackTime;
            defaultSubdedTime = SubdedTime;

            AddViewModel();
            AddEvent();
        }

        private void OnDestroy()
        {
            RemoveViewModel();
            RemoveEvent();
        }

        private void OnEnable()
        {
            vm.RequestPlayerHPChanged(vm.MaxHP);
            vm.RequestPlayerStaminaChanged(vm.MaxStamina);

            isParryAble = true;
        }

        private void AddViewModel()
        {
            if (vm == null)
            {
                vm = new PlayerViewModel();
                vm.PropertyChanged += OnPropertyChanged;
                vm.RegisterPlayerHPChanged(true);
                vm.RegisterPlayerMaxHPChanged(true);
                vm.RegisterPlayerStaminaChanged(true);
                vm.RegisterPlayerMaxStaminaChanged(true);
                vm.RegisterPlayerLifeCountChanged(true);
                vm.RegisterPlayerAttackDamageChanged(true);
                vm.RegisterLockOnTargetChanged(true);
                vm.ReigsterAssassinatedTypeChanged(true);
            }
        }

        private void RemoveViewModel()
        {
            if (vm != null)
            {
                vm.ReigsterAssassinatedTypeChanged(false);
                vm.RegisterLockOnTargetChanged(false);
                vm.RegisterPlayerAttackDamageChanged(false);
                vm.RegisterPlayerLifeCountChanged(false);
                vm.RegisterPlayerMaxStaminaChanged(false);
                vm.RegisterPlayerStaminaChanged(false);
                vm.RegisterPlayerMaxHPChanged(false);
                vm.RegisterPlayerHPChanged(false);
                vm.PropertyChanged -= OnPropertyChanged;
                vm = null;
            }
        }

        private void AddEvent()
        {
            EventManager<DataEvent>.Binding<PlayerData>(true, DataEvent.LoadPlayerData, ReadPlayerData);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.RecoveryHP, RecoveryPlayerValues);
            EventManager<PlayerAction>.Binding(true, PlayerAction.RecoveryStamina, RecoveryPlayerStamina);
            EventManager<PlayerAction>.Binding(true, PlayerAction.StopRecoveryStamina, StopRecoveryPlayerStamina);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.Parring, SetParring);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ParryAble_PlayerView, SetParryAble);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.IsDefense, SetIsDefense);
            EventManager<PlayerAction>.Binding(true, PlayerAction.SubdedStateEnd, SubdedEnd);
            EventManager<PlayerAction>.Binding(true, PlayerAction.Resurrection, Resurrection);
        }

        private void RemoveEvent()
        {
            EventManager<DataEvent>.Binding<PlayerData>(false, DataEvent.LoadPlayerData, ReadPlayerData);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.RecoveryHP, RecoveryPlayerValues);
            EventManager<PlayerAction>.Binding(false, PlayerAction.RecoveryStamina, RecoveryPlayerStamina);
            EventManager<PlayerAction>.Binding(false, PlayerAction.StopRecoveryStamina, StopRecoveryPlayerStamina);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.Parring, SetParring);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.ParryAble_PlayerView, SetParryAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.IsDefense, SetIsDefense);
            EventManager<PlayerAction>.Binding(false, PlayerAction.SubdedStateEnd, SubdedEnd);
            EventManager<PlayerAction>.Binding(false, PlayerAction.Resurrection, Resurrection);
        }

        private void ReadPlayerData(PlayerData data)
        {
            if (data == null) return;

            playerData = data.Clone();
            vm.RequestPlayerMaxHPChanged(playerData.MaxHP);
            vm.RequestPlayerHPChanged(playerData.HP);
            vm.RequestPlayerMaxStaminaChanged(playerData.MaxStamina);
            vm.RequestPlayerStaminaChanged(playerData.Stamina);
            vm.RequestPlayerLifeCountChanged(playerData.Life);
            vm.RequestPlayerAttackDamageChanged(playerData.ATK);

            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ChangedSpeed, playerData.WalkSpeed, playerData.RunSpeed);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(vm.HP):
                    //체력 UI와 연관
                    if(vm.HP <= 0)
                    {
                        //var lifeCount = vm.LifeCount - 1;
                        //vm.RequestPlayerLifeCountChanged(lifeCount);
                        animator.SetBool(hashDie, true);
                        animator.SetTrigger(hashDead);
                    }

                    isDie = vm.HP <= 0;
                    break;
                case nameof(vm.Stamina):
                    //stamina UI와 연관
                    if(vm.Stamina <= 0)
                    {
                        animator.SetBool(hashIncapacitate, true);
                        animator.SetTrigger(hashIncapacitated);
                    }
                    break;
                case nameof(vm.MaxHP):
                    break;
                case nameof(vm.MaxStamina):
                    break;
                case nameof(vm.LockOnTarget):
                    break;
            }
        }

        public void Hurt(MonsterView attacker, float damage)
        {
            if (isDie || animator.GetBool("Assassinated")) return;

            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ChangedBattleMode, true);

            //방어 성공
            if (isDefense && IsDefenceSuccess(attacker.transform.position))
            {
                if (attacker.Type == MonsterType.Boss)
                {
                    isKnockback = true;
                    attackerPosition = attacker.transform.position;
                }

                if (isParring)
                {
                    if (attacker.vm.CurrentAttackMethod.AttackType != "Long")
                    {
                        attacker.Parried(this);
                        return;
                    }
                    //원거리 공격은 아무런 효과가 없음으로 처리
                    return;
                }

                if (animator.GetBool("Defense") && isParryAble)
                {
                    //방어 성공
                    vm.RequestPlayerStaminaChanged(vm.Stamina - attacker._monsterData.Strength);
                    animator.SetTrigger("Hurt");
                    return;
                }
            }

            Debug.Log("공격 받음");
            //방어 실패
            vm.RequestPlayerHPChanged(vm.HP - damage);

            //UnityEngine.Debug.Log(player_info.HP);
            HurtAnimation(attacker);
            isDie = vm.HP <= 0f;
        }

        private bool IsDefenceSuccess(Vector3 attackerPosition)
        {
            Vector3 attackDir = attackerPosition - transform.position;
            attackDir.y = 0;

            Vector3 playerForward = transform.forward;
            playerForward.y = 0;

            float angle = Vector3.Angle(playerForward, attackDir);
            if (angle <= 45f) return true;
            else return false;
        }

        private void Update()
        {
            if (isSubded)
            {
                SubdedTime = Mathf.Clamp(SubdedTime - Time.deltaTime, 0, defaultSubdedTime);
                if (SubdedTime <= 0)
                {
                    vm.RequestPlayerStaminaChanged(playerData.Stamina);
                    isSubded = false;
                    animator.SetBool("Incapacitated", false);
                }
            }

            if (knockbackTime <= 0)
            {
                isKnockback = false;
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, false);
                //animator.SetBool("isMoveAble", true);
                knockbackTime = defaultKnockbackTime;
            }

            if (isKnockback)
            {
                knockbackTime = Mathf.Clamp(knockbackTime - Time.deltaTime, 0, knockbackTime);
                NockBacking();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                var hp = Mathf.Clamp(vm.HP - 20, 0, vm.MaxHP);
                vm.RequestPlayerHPChanged(hp);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                var stamina = Mathf.Clamp(vm.Stamina - 100, 0, vm.MaxStamina);
                vm.RequestPlayerStaminaChanged(stamina);
            }
        }

        private void NockBacking()
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsNotMoveAble, true);
            //animator.SetBool("isMoveAble", false);
            Vector3 AttackDir = transform.position - attackerPosition;
            KnockBack(AttackDir, knockbackPower);
        }

        private void KnockBack(Vector3 AttackDir, float addPower)
        {
            AttackDir.y = 0;
            AttackDir.Normalize();

            //playerController.Move(AttackDir * addPower * Time.deltaTime);            
        }

        private void HurtAnimation(MonsterView attaker)
        {
            Vector3 attakerPosition = attaker.transform.position;
            if (attaker.Type == MonsterType.Boss) knockbackPower = 5;
            else knockbackPower = 1;

            Vector3 attackDir = transform.position - attakerPosition;
            attackDir.y = 0;
            attackDir.Normalize();

            animator.SetFloat("Hurt_z", attackDir.z);
            animator.SetFloat("Hurt_x", attackDir.x);
            animator.SetBool("Defense", false);
            animator.SetTrigger("Hurt");
        }

        private void RecoveryPlayerStamina()
        {
            StartCoroutine(ChargeStamina());
        }

        private void StopRecoveryPlayerStamina()
        {
            StopCoroutine(ChargeStamina());
        }

        IEnumerator ChargeStamina()
        {
            while(vm.Stamina < vm.MaxStamina)
            {
                vm.RequestPlayerStaminaChanged(vm.Stamina + 10 * Time.deltaTime);
                yield return new WaitForSeconds(1f);
            }

            staminaRecoveryCoroutine = null;
        }

        private void SetParring(bool isParring)
        {
            if (this.isParring == isParring) return;
            
            this.isParring = isParring;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ParryAble_PlayerAttack, !isParring);
        }

        private void SetParryAble(bool isParryAble)
        {
            if (this.isParryAble == isParryAble) return;

            this.isParryAble = isParryAble;
        }

        private void SetIsDefense(bool isDefense)
        {
            if (this.isDefense == isDefense) return;

            this.isDefense = isDefense;
        }

        private void RecoveryPlayerValues(bool isHealing)
        {
            if (isHealing && HpRecoveryCoroutine == null)
            {
                HpRecoveryCoroutine = StartCoroutine(Heal());
            }
            else if(!isHealing && HpRecoveryCoroutine != null)
            {
                StopCoroutine(HpRecoveryCoroutine);
            }
        }

        IEnumerator Heal()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                var HealingHP = Mathf.Clamp(vm.HP + 5f, 0, vm.MaxHP);
                vm.RequestPlayerHPChanged(HealingHP);
            }           
        }

        private void SubdedEnd()
        {
            animator.SetBool(hashIncapacitate, false);
            animator.ResetTrigger(hashIncapacitated);

            vm.RequestPlayerStaminaChanged(vm.MaxStamina);
        }

        private void Resurrection()
        {
            if (vm.LifeCount <= 0) return;

            animator.SetBool(hashDie, false);

            vm.RequestPlayerHPChanged(vm.MaxHP);

            var lifeCount = vm.LifeCount - 1;
            vm.RequestPlayerLifeCountChanged(lifeCount);
        }
    }
}
