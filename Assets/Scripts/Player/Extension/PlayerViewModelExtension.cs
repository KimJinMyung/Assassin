using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;

public static class PlayerViewModelExtension
{
    #region HP
    public static void RegisterPlayerHPChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerHPChangedCallback(vm.OnResponsePlayerHPChangedEvent, isRegister);
    }
    public static void RequestPlayerHPChanged(this PlayerViewModel vm, float hp)
    {
        LogicManager.instance.OnPlayerHPChanged(hp);
    }
    public static void OnResponsePlayerHPChangedEvent(this PlayerViewModel vm, float HP) 
    {
        vm.HP = HP;

    }
    #endregion
    #region Stamina
    public static void RegisterPlayerStaminaChanged(this PlayerViewModel vm, bool isRegister)
    {
        LogicManager.instance.RegisterPlayerStaminaChangedCallback(vm.OnResponsePlayerStaminaChangedEvent, isRegister);
    }
    public static void RequestPlayerStaminaChanged(this PlayerViewModel vm, float stamina)
    {
        LogicManager.instance.OnPlayerStaminaChanged(stamina);
    }
    public static void OnResponsePlayerStaminaChangedEvent(this PlayerViewModel vm, float stamina)
    {
        vm.Stamina = stamina;

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
