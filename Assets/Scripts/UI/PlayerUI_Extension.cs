using EventEnum;

public static class PlayerUI_Extension
{
    public static void RegisterPlayerHPChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        //PlayerManager.Instance.BindHPChanged(vm.OnPlayerHpChanged, isRegister);
        EventManager<PlayerUI>.Binding<float>(isRegister, PlayerUI.ChangedHP, vm.OnPlayerHpChanged);
    }

    public static void OnPlayerHpChanged(this PlayerUI_ViewModel vm, float HP)
    {
        vm.HP = HP;
    }
    public static void RegisterPlayerMaxHPChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        //PlayerManager.Instance.BindMaxHPChanged(vm.OnPlayerMaxHpChanged, isRegister);
        EventManager<PlayerUI>.Binding<float>(isRegister, PlayerUI.ChangedMaxHP, vm.OnPlayerMaxHpChanged);
    }

    public static void OnPlayerMaxHpChanged(this PlayerUI_ViewModel vm, float maxHp)
    {
        vm.MaxHP = maxHp;
    }

    public static void RegisterPlayerStaminaChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        //PlayerManager.Instance.BindStaminaChanged(vm.OnPlayerStaminaiChanged, isRegister);
        EventManager<PlayerUI>.Binding<float>(isRegister, PlayerUI.ChangedStamina, vm.OnPlayerStaminaiChanged);
    }

    public static void OnPlayerStaminaiChanged(this PlayerUI_ViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    public static void RegisterPlayerMaxStaminaChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        //PlayerManager.Instance.BindMaxStaminaChanged(vm.OnPlayerMaxStaminaiChanged, isRegister);
        EventManager<PlayerUI>.Binding<float>(isRegister, PlayerUI.ChangedMaxStamina, vm.OnPlayerMaxStaminaiChanged);
    }

    public static void OnPlayerMaxStaminaiChanged(this PlayerUI_ViewModel vm, float maxStamina)
    {
        vm.MaxStamina = maxStamina;
    }
    public static void RegisterPlayerLifeCountChanged(this PlayerUI_ViewModel vm, bool isRegister)
    {
        //PlayerManager.Instance.BindLifeCountChanged(vm.OnPlayerLifeCountChanged, isRegister);
        EventManager<PlayerUI>.Binding<float>(isRegister, PlayerUI.ChangedLifeCount, vm.OnPlayerLifeCountChanged);
    }

    public static void OnPlayerLifeCountChanged(this PlayerUI_ViewModel vm, float LifeCount)
    {
        vm.LifeCount = LifeCount;
    }
}
