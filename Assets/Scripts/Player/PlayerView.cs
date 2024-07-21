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
        }

        SetPlayerInfo();
    }

    private void OnDisable()
    {
        if(vm != null )
        {
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
                StartCoroutine(PushBack(attacker.transform.position, 1));
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

            if (animator.GetBool("Defense"))
            {
                //방어 성공
                vm.RequestPlayerStaminaChanged(vm.Stamina - attacker.vm.Stamina);
                isAssassinated = vm.Stamina <= 0f;
                return;
            }
        }

        //방어 실패
        vm.RequestPlayerHPChanged(vm.HP - damage);

        //UnityEngine.Debug.Log(player_info.HP);
        AttackDir(attacker.transform.position);
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

    IEnumerator PushBack(Vector3 attackerPosition, float AddPower)
    {
        Vector3 dir = transform.position - attackerPosition;
        dir.y = 0;
        dir.Normalize();

        float timer = 0f;

        while (timer < 1f)
        {
            playerController.Move(dir * AddPower * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    private void AttackDir(Vector3 attakerPosition)
    {
        StartCoroutine(PushBack(attakerPosition, 5));

        Vector3 knockbackDir = transform.position - attakerPosition;
        knockbackDir.y = 0;
        knockbackDir.Normalize();

        animator.SetFloat("Hurt_z", knockbackDir.z);
        animator.SetFloat("Hurt_x", knockbackDir.x);
    }

    private bool IsAnimationRunning(string animationName)
    {
        if (animator == null) return false;

        bool isRunning = false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime >= 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }
}
