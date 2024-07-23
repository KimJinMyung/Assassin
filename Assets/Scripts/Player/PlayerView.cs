using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class PlayerView : MonoBehaviour
{
    public PlayerData playerData {  get; private set; }
    private PlayerViewModel vm;
    public PlayerViewModel ViewModel { get {  return vm; } }

    private CharacterController playerController;
    private Animator animator;

    private Vector3 attackerPosition;
    private float knockbackPower;

    public bool isKnockback;
    private bool isDie;
    private bool isAssassinated;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if(vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterPlayerHPChanged(true);
            vm.RegisterPlayerStaminaChanged(true);
            vm.RegisterLockOnTargetChanged(true);
        }

        SetPlayerInfo();
    }

    private void OnDisable()
    {
        if(vm != null )
        {
            vm.RegisterLockOnTargetChanged(false);
            vm.RegisterPlayerStaminaChanged(false);
            vm.RegisterPlayerHPChanged(false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        playerData = player.Clone();
        vm.RequestPlayerHPChanged(player.Clone().HP);
        vm.RequestPlayerStaminaChanged(player.Clone().Stamina);
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
            case nameof(vm.LockOnTarget):
                animator.SetBool("LockOn", vm.LockOnTarget != null);
                break;
        }
    }

    public void Hurt(MonsterView attacker, float damage)
    {
        if (isDie || isAssassinated) return;

        //방어 성공
        if (IsDefenceSuccess(attacker.transform.position))
        {
            if (attacker.Type == MonsterType.Boss)
            {
                //StartCoroutine(PushBack(attacker.transform.position, 1));
                isKnockback = true;
                attackerPosition = attacker.transform.position;
            }

            if (animator.GetBool("Parring"))
            {
                if (attacker.vm.CurrentAttackMethod.AttackType != "Long")
                {
                    attacker.Parried(this);
                    return;
                }
                //원거리 공격은 아무런 효과가 없음으로 처리
                return;
            }

            if (animator.GetBool("Defense") && animator.GetBool("ParryAble"))
            {
                //방어 성공
                vm.RequestPlayerStaminaChanged(vm.Stamina - attacker.vm.Stamina);
                isAssassinated = vm.Stamina <= 0f;
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
        if(isKnockback) NockBacking();
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

        playerController.Move(AttackDir * addPower * Time.deltaTime);
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
        animator.SetTrigger("Hurt");
    }

    private bool IsHurtAnimationRunning()
    {
        if (animator == null) return false;

        bool isRunning = false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // "Hurt" 상태가 현재 상태인지 확인
        if (stateInfo.IsTag("Hurt"))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime >= 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }
}
