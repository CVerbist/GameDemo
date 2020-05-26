using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class TargetCondition
{
    #region Types
    public enum ETargetGroup
    {
        AllButSelfAndDead,
        AllButDead,
        AllButSelf,
        AllDead,
        All,
        Enemies,
        Allies
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates the selector function 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static Func<BattleEntity, BattleEntity, bool> GetFunc(ETargetGroup group)
    {
        switch (group)
        {
            case ETargetGroup.All:
                return (self, other) => true;
            case ETargetGroup.AllButSelf:
                return (self, other) => self != other;
            case ETargetGroup.Enemies:
                return (self, other) => other.IsEnemyOf(self);
            case ETargetGroup.Allies:
                return (self, other) => other.IsAllyOf(self);
            case ETargetGroup.AllButSelfAndDead:
                return (self, other) => self != other && !other.IsDead;
            case ETargetGroup.AllButDead:
                return (self, other) => !other.IsDead;
            case ETargetGroup.AllDead:
                return (self, other) => other.IsDead;
            default:
                throw new ArgumentOutOfRangeException("group", group, null);
        }
    }
    #endregion
}
