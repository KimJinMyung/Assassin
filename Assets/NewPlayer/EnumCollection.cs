namespace EventEnum
{
    public enum PlayerAction
    {
        Jump,
        ChangedSpeed,
        RecoveryStamina,
        StopRecoveryStamina,
        IsNotMoveAble,
        SetAttackAble,
        IsAttacking,
        IsDefense,
        ParryAble,
        Parring,
    }

    public enum PlayerMVVM
    {
        ChangedHP,
        ChangedMaxHP,
        ChangedStamina,
        ChangedMaxStamina,
        ChangedLifeCount,
        ChangedATK,
        ChangedMoveDir,
        ChangedRotate,
        ChangedLockOnTarget,
        ChangedAssassinatedTarget,
    }

    public enum PlayerUI
    {
        ChangedHP,
        ChangedMaxHP,
        ChangedStamina,
        ChangedMaxStamina,
        ChangedLifeCount,
    }

    public enum DataEvent
    {        
        LoadPlayerData,
    }

    public enum CameraPosEvent
    {
        UpdateCameraPosition,
    }

    public enum AttackBoxEvent
    {
        IsDefense,
    }

    public enum LockOnEvent
    {
        UpdateLockOnTargetList,
        UpdateLockOnAbleTarget,
        UpdateLockOnTarget,
    }
}