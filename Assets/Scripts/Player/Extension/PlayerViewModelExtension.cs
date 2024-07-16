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
}
