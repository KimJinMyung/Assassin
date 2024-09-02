namespace EventEnum
{
    public enum PlayerAction
    {
        Jump,
        ChangedSpeed,
    }
    public enum PlayerMVVM
    {
        ChangedHP,
        ChangedMaxHP,
        ChangedStamina,
        ChangedMaxStamina,
        ChangedATK,
        ChangedMoveDir,
        ChangedRotate,
        ChangedLockOnTarget,
        ChangedAssassinatedTarget,
    }

    public enum DataEvent
    {        
        LoadPlayerData,
    }
}