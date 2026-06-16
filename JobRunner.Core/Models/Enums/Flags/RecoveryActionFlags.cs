using System;

namespace JobRunner.Core.Models.Enums.Flags
{
    /// <summary>
    /// Флаги действий для массового восстановления
    /// </summary>
    [Flags]
    public enum RecoveryActionFlags
    {
        None = 0,
        RecoverStuck = 1 << 0,
        ExecuteMissed = 1 << 1,
        SkipMissed = 1 << 2,
        DisableInfiniteLoop = 1 << 3,
        DisableRetryExhaustion = 1 << 4,
        All = RecoverStuck | 
            ExecuteMissed |
            DisableInfiniteLoop | 
            DisableRetryExhaustion
    }
}
