using UnityEngine;
using UnityEngine.Windows;

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
}
