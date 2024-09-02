public static class PlayerUI_Extension
{
    public static void RegisterPlayerHPChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        PlayerManager.Instance.BindHPChanged(vm.OnPlayerHpChanged, isRegister);
    }

    public static void OnPlayerHpChanged(this PlayerUI_ViewModel vm, float HP)
    {
        vm.HP = HP;
    }
    public static void RegisterPlayerMaxHPChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        PlayerManager.Instance.BindMaxHPChanged(vm.OnPlayerMaxHpChanged, isRegister);
    }

    public static void OnPlayerMaxHpChanged(this PlayerUI_ViewModel vm, float maxHp)
    {
        vm.MaxHP = maxHp;
    }

    public static void RegisterPlayerStaminaChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        PlayerManager.Instance.BindStaminaChanged(vm.OnPlayerStaminaiChanged, isRegister);
    }

    public static void OnPlayerStaminaiChanged(this PlayerUI_ViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    public static void RegisterPlayerMaxStaminaChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        PlayerManager.Instance.BindMaxStaminaChanged(vm.OnPlayerMaxStaminaiChanged, isRegister);
    }

    public static void OnPlayerMaxStaminaiChanged(this PlayerUI_ViewModel vm, float maxStamina)
    {
        vm.MaxStamina = maxStamina;
    }
    public static void RegisterPlayerLifeCountChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        PlayerManager.Instance.BindLifeCountChanged(vm.OnPlayerLifeCountChanged, isRegister);
    }

    public static void OnPlayerLifeCountChanged(this PlayerUI_ViewModel vm, float LifeCount)
    {
        vm.LifeCount = LifeCount;
    }
}
