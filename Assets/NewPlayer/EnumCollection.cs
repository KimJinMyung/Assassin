namespace EventEnum
{
    public enum PlayerAction
    {
        Jump,
        ChangedSpeed,
        RecoveryHP,
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
        StopRotation,
        AttackLockOnEnable,
        SubdedStateEnd,
        Resurrection,
        ChangedBattleMode,
        KnockBack,
        ChangedKnockBackPower,
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
        HitMonsterReset,
    }

    public enum MonsterEvent
    {
        SpawnMonster,
        SpawnBossMonster,
        ChangedLockOnAbleMonsterList,
        ChangedLockOnIconEnableList,
        Attack,
        IsDead,
        SetDetectRange,
        ChangedAttackMethod,
        SetAttackType,
    }

    public enum MonsterUIEvent
    {
        UpdateLifeCount,
    }

    public enum MonsterMVVM
    {
        ChangedInstanceID,
        ChangedHP,
        ChangedMaxHP,
        ChangedStamina,
        changedMaxStamina,
        changedLifeCount,
        ChangedATK,
    }
}