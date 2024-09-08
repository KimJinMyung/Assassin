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
        ParryAble_PlayerAttack,
        ParryAble_PlayerView,
        Parring,
        IsLockOn,
        ChangedLockOnTarget,
        StopRotation
    }

    public enum GrapplingEvent
    {
        GrapplingPull,
        GrapplingMove,       
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

    public enum CameraEvent
    {
        UpdateCameraPosition,
        PlayerAttackSuccess,
    }

    public enum AttackBoxEvent
    {
        IsDefense,
        IsAttacking,
    }

    public enum MonsterEvent
    {
        SpawnMonster,
        ChangedLockOnAbleMonsterList,
        ChangedLockOnIconEnableList,
    }
}