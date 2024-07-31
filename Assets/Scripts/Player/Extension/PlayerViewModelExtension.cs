using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;

public static class PlayerViewModelExtension
{
    #region HP
    public static void RegisterPlayerHPChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerHPChangedCallback(vm.OnResponsePlayerHPChangedEvent, isRegister);
        PlayerManager.instance.BindHPChanged(vm.OnResponsePlayerHPChangedEvent, isRegister);
    }
    public static void RequestPlayerHPChanged(this PlayerViewModel vm, float hp)
    {
        LogicManager.instance.OnPlayerHPChanged(hp);
        PlayerManager.instance.SetPlayerHP(hp);
    }
    public static void OnResponsePlayerHPChangedEvent(this PlayerViewModel vm, float HP) 
    {
        vm.HP = HP;
    }
    #endregion
    #region MaxHP
    public static void RegisterPlayerMaxHPChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerMaxHPChangedCallback(vm.OnResponsePlayerMaxHPChangedEvent, isRegister);
        PlayerManager.instance.BindMaxHPChanged(vm.OnResponsePlayerMaxHPChangedEvent, isRegister);
    }
    public static void RequestPlayerMaxHPChanged(this PlayerViewModel vm, float hp)
    {
        LogicManager.instance.OnPlayerMaxHPChanged(hp);
        PlayerManager.instance.SetPlayerMaxHP(hp);
    }
    public static void OnResponsePlayerMaxHPChangedEvent(this PlayerViewModel vm, float MaxHP)
    {
        vm.MaxHP = MaxHP;
    }
    #endregion
    #region Stamina
    public static void RegisterPlayerStaminaChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerStaminaChangedCallback(vm.OnResponsePlayerStaminaChangedEvent, isRegister);
        PlayerManager.instance.BindStaminaChanged(vm.OnResponsePlayerStaminaChangedEvent, isRegister);
    }
    public static void RequestPlayerStaminaChanged(this PlayerViewModel vm, float stamina)
    {
        LogicManager.instance.OnPlayerStaminaChanged(stamina);
        PlayerManager.instance.SetStamina(stamina);
    }
    public static void OnResponsePlayerStaminaChangedEvent(this PlayerViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    #endregion
    #region MaxStamina
    public static void RegisterPlayerMaxStaminaChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerMaxStaminaChangedCallback(vm.OnResponsePlayerMaxStaminaChangedEvent, isRegister);
        PlayerManager.instance.BindStaminaChanged(vm.OnResponsePlayerMaxStaminaChangedEvent, isRegister);
    }
    public static void RequestPlayerMaxStaminaChanged(this PlayerViewModel vm, float stamina)
    {
        LogicManager.instance.OnPlayerMaxStaminaChanged(stamina);
        PlayerManager.instance.SetMaxStamina(stamina);
    }
    public static void OnResponsePlayerMaxStaminaChangedEvent(this PlayerViewModel vm, float maxStamina)
    {
        vm.MaxHP = maxStamina;
    }
    #endregion
    #region Move
    public static void RegisterMoveVelocity(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterMoveVelocityChangedCallback(vm.OnResponseMoveVelocityChangedEvent, isRegister);
    }
    public static void RequestMoveOnInput(this PlayerViewModel vm, float x, float y)
    {
        LogicManager.instance.OnMoveInput(x, y);
    }

    public static void OnResponseMoveVelocityChangedEvent(this PlayerViewModel vm, float contextValueX, float contextValueY)
    {
        vm.Movement = new Vector2(contextValueX, contextValueY);
    }
    #endregion
    #region Rotate
    public static void RegisterActorRotate(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterActorRotateChangedCallback(vm.OnResponseActorRotateChangedEvent, isRegister);
    }
    public static void RequestActorRotate(this PlayerViewModel vm, float x, float y, float z)
    {
        LogicManager.instance.OnActorRotate(x, y, z);
    }

    public static void OnResponseActorRotateChangedEvent(this PlayerViewModel vm, float contextValueX, float contextValueY, float contextValueZ)
    {
        vm.Rotation = Quaternion.Euler(new Vector3(contextValueX, contextValueY, contextValueZ));
    }
    #endregion
    #region LockOnTarget
    public static void RegisterLockOnTargetChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterLockOnTargetChangedCallback(vm.OnResponseLockOnTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnTarget(this PlayerViewModel vm, Transform target)
    {
        LogicManager.instance.OnLockOnTarget(target);
    }

    public static void OnResponseLockOnTargetChangedEvent(this PlayerViewModel vm, Transform tartget)
    {
        vm.LockOnTarget = tartget;
    }
    #endregion
    #region Assassinated
    public static void ReigsterAssassinatedTypeChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterAssassinatedChangedCallback(vm.OnResponseAssassinatedTypeChangedEvent, isRegister);
    }

    public static void RequestAssassinatedType(this PlayerViewModel vm, MonsterView monster)
    {
        LogicManager.instance.OnAssassinated(monster);
    }

    public static void OnResponseAssassinatedTypeChangedEvent(this PlayerViewModel vm, MonsterView monster)
    {
        monster._behaviorTree.SetVariableValue("isAssassinated", true);
        monster._behaviorTree.SetVariableValue("isDead", true);
        vm.AssassinatedMonsters = monster;
    }
    #endregion
}
