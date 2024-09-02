using System.ComponentModel;
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
        private bool isDie;
        public bool isSubded { get; private set; }

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
        }

        private void RemoveEvent()
        {
            EventManager<DataEvent>.Binding<PlayerData>(false, DataEvent.LoadPlayerData, ReadPlayerData);
        }

        private void ReadPlayerData(PlayerData data)
        {
            if (data == null) return;

            playerData = data.Clone();
            vm.RequestPlayerHPChanged(playerData.HP);
            vm.RequestPlayerStaminaChanged(playerData.Stamina);
            vm.RequestPlayerMaxHPChanged(playerData.MaxHP);
            vm.RequestPlayerMaxStaminaChanged(playerData.MaxStamina);
            vm.RequestPlayerAttackDamageChanged(playerData.ATK);

            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ChangedSpeed, playerData.WalkSpeed, playerData.RunSpeed);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(vm.HP):
                    //체력 UI와 연관
                    break;
                case nameof(vm.Stamina):
                    //stamina UI와 연관
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

            //방어 성공
            if (IsDefenceSuccess(attacker.transform.position))
            {
                if (attacker.Type == MonsterType.Boss)
                {
                    isKnockback = true;
                    attackerPosition = attacker.transform.position;
                }

                if (animator.GetBool("Parring"))
                {
                    if (attacker.vm.CurrentAttackMethod.AttackType != "Long")
                    {
                        Debug.Log("패링중...");
                        attacker.Parried(this);
                        return;
                    }
                    //원거리 공격은 아무런 효과가 없음으로 처리
                    return;
                }

                if (animator.GetBool("Defense") && animator.GetBool("ParryAble"))
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
            //if (isSubded)
            //{
            //    SubdedTime = Mathf.Clamp(SubdedTime - Time.deltaTime, 0, defaultSubdedTime);
            //    if (SubdedTime <= 0)
            //    {
            //        vm.RequestPlayerStaminaChanged(playerData.Stamina);
            //        isSubded = false;
            //        animator.SetBool("Incapacitated", false);
            //    }
            //}

            //if (knockbackTime <= 0)
            //{
            //    isKnockback = false;
            //    animator.SetBool("isMoveAble", true);
            //    knockbackTime = defaultKnockbackTime;
            //}

            //if (isKnockback)
            //{
            //    knockbackTime = Mathf.Clamp(knockbackTime - Time.deltaTime, 0, knockbackTime);
            //    NockBacking();
            //}
        }

        private void NockBacking()
        {
            animator.SetBool("isMoveAble", false);
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
    }
}
