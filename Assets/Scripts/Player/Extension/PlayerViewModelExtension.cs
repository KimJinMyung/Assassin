using EventEnum;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;

public static class PlayerViewModelExtension
{
    #region HP
    public static void RegisterPlayerHPChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterPlayerHPChangedCallback(vm.OnResponsePlayerHPChangedEvent, isRegister);
        //PlayerManager.Instance.BindHPChanged(vm.OnResponsePlayerHPChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float>(isRegister, PlayerMVVM.ChangedHP, vm.OnResponsePlayerHPChangedEvent);
    }
    public static void RequestPlayerHPChanged(this PlayerViewModel vm, float hp)
    {
        //LogicManager.Instance.OnPlayerHPChanged(hp);
        //PlayerManager.Instance.SetPlayerHP(hp);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedHP, hp);
    }
    public static void OnResponsePlayerHPChangedEvent(this PlayerViewModel vm, float HP) 
    {
        vm.HP = HP;
    }
    #endregion
    #region MaxHP
    public static void RegisterPlayerMaxHPChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterPlayerMaxHPChangedCallback(vm.OnResponsePlayerMaxHPChangedEvent, isRegister);
        //PlayerManager.Instance.BindMaxHPChanged(vm.OnResponsePlayerMaxHPChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float>(isRegister, PlayerMVVM.ChangedMaxHP, vm.OnResponsePlayerMaxHPChangedEvent);
    }
    public static void RequestPlayerMaxHPChanged(this PlayerViewModel vm, float hp)
    {
        //LogicManager.Instance.OnPlayerMaxHPChanged(hp);
        //PlayerManager.Instance.SetPlayerMaxHP(hp);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedMaxHP);
    }
    public static void OnResponsePlayerMaxHPChangedEvent(this PlayerViewModel vm, float MaxHP)
    {
        vm.MaxHP = MaxHP;
    }
    #endregion
    #region Stamina
    public static void RegisterPlayerStaminaChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterPlayerStaminaChangedCallback(vm.OnResponsePlayerStaminaChangedEvent, isRegister);
        //PlayerManager.Instance.BindStaminaChanged(vm.OnResponsePlayerStaminaChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float>(isRegister, PlayerMVVM.ChangedStamina, vm.OnResponsePlayerStaminaChangedEvent);
    }
    public static void RequestPlayerStaminaChanged(this PlayerViewModel vm, float stamina)
    {
        //LogicManager.Instance.OnPlayerStaminaChanged(stamina);
        //PlayerManager.Instance.SetStamina(stamina);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedStamina);
    }
    public static void OnResponsePlayerStaminaChangedEvent(this PlayerViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    #endregion
    #region MaxStamina
    public static void RegisterPlayerMaxStaminaChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterPlayerMaxStaminaChangedCallback(vm.OnResponsePlayerMaxStaminaChangedEvent, isRegister);
        //PlayerManager.Instance.BindStaminaChanged(vm.OnResponsePlayerMaxStaminaChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float>(isRegister, PlayerMVVM.ChangedMaxStamina, vm.OnResponsePlayerMaxStaminaChangedEvent);
    }
    public static void RequestPlayerMaxStaminaChanged(this PlayerViewModel vm, float stamina)
    {
        //LogicManager.Instance.OnPlayerMaxStaminaChanged(stamina);
        //PlayerManager.Instance.SetMaxStamina(stamina);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedMaxStamina);
    }
    public static void OnResponsePlayerMaxStaminaChangedEvent(this PlayerViewModel vm, float maxStamina)
    {
        vm.MaxHP = maxStamina;
    }
    #endregion
    #region AttackDamage
    public static void RegisterPlayerAttackDamageChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterPlayerAttackDamageChangedCallback(vm.OnResponsePlayerAttackDamageChangedEvent, isRegister);
        
        EventManager<PlayerMVVM>.Binding<float>(isRegister, PlayerMVVM.ChangedATK, vm.OnResponsePlayerAttackDamageChangedEvent);
    }
    public static void RequestPlayerAttackDamageChanged(this PlayerViewModel vm, float atk)
    {
        //LogicManager.Instance.OnPlayerAttackDamageChanged(atk);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedATK);
    }
    public static void OnResponsePlayerAttackDamageChangedEvent(this PlayerViewModel vm, float atk)
    {
        vm.AttackDamage = atk;
    }
    #endregion
    #region Move
    public static void RegisterMoveVelocity(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterMoveVelocityChangedCallback(vm.OnResponseMoveVelocityChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float, float>(isRegister, PlayerMVVM.ChangedMoveDir, vm.OnResponseMoveVelocityChangedEvent);
    }
    public static void RequestMoveOnInput(this PlayerViewModel vm, float x, float y)
    {
        //LogicManager.Instance.OnMoveInput(x, y);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedMoveDir);
    }

    public static void OnResponseMoveVelocityChangedEvent(this PlayerViewModel vm, float contextValueX, float contextValueY)
    {
        vm.Movement = new Vector2(contextValueX, contextValueY);
    }
    #endregion
    #region Rotate
    public static void RegisterActorRotate(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterActorRotateChangedCallback(vm.OnResponseActorRotateChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<float, float, float>(isRegister, PlayerMVVM.ChangedRotate, vm.OnResponseActorRotateChangedEvent);
    }
    public static void RequestActorRotate(this PlayerViewModel vm, float x, float y, float z)
    {
        //LogicManager.Instance.OnActorRotate(x, y, z);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedRotate);
    }

    public static void OnResponseActorRotateChangedEvent(this PlayerViewModel vm, float contextValueX, float contextValueY, float contextValueZ)
    {
        vm.Rotation = Quaternion.Euler(new Vector3(contextValueX, contextValueY, contextValueZ));
    }
    #endregion
    #region LockOnTarget
    public static void RegisterLockOnTargetChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterLockOnTargetChangedCallback(vm.OnResponseLockOnTargetChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<Transform>(isRegister, PlayerMVVM.ChangedLockOnTarget, vm.OnResponseLockOnTargetChangedEvent);
    }

    public static void RequestLockOnTarget(this PlayerViewModel vm, Transform target)
    {
        //LogicManager.Instance.OnLockOnTarget(target);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedLockOnTarget);
    }

    public static void OnResponseLockOnTargetChangedEvent(this PlayerViewModel vm, Transform tartget)
    {
        vm.LockOnTarget = tartget;
    }
    #endregion
    #region Assassinated
    public static void ReigsterAssassinatedTypeChanged(this PlayerViewModel vm, bool isRegister)
    {
        //LogicManager.Instance.RegisterAssassinatedChangedCallback(vm.OnResponseAssassinatedTypeChangedEvent, isRegister);

        EventManager<PlayerMVVM>.Binding<MonsterView>(isRegister, PlayerMVVM.ChangedAssassinatedTarget, vm.OnResponseAssassinatedTypeChangedEvent);
    }

    public static void RequestAssassinatedType(this PlayerViewModel vm, MonsterView monster)
    {
        //LogicManager.Instance.OnAssassinated(monster);

        EventManager<PlayerMVVM>.TriggerEvent(PlayerMVVM.ChangedAssassinatedTarget);
    }

    public static void OnResponseAssassinatedTypeChangedEvent(this PlayerViewModel vm, MonsterView monster)
    {
        monster._behaviorTree.SetVariableValue("isAssassinated", true);

        if (monster.Type != MonsterType.Boss)
            monster._behaviorTree.SetVariableValue("isDead", true);
        else monster.animator.SetTrigger("Assassinated");
        vm.AssassinatedMonsters = monster;
    }
    #endregion
}
