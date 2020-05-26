using System;
using UnityEngine;
using TargetConditionClass = TargetCondition;

/// <summary>
/// Skills are all of type Skill. Stats of a skill are held in a data file such as .json or .xml.
/// </summary>
[Serializable]
public class Skill
{
    #region Types
    /// <summary>
    /// Different types of damages
    /// </summary>
    [Flags]
    public enum EType
    {
        None      = 0,
        Strength  = 1 << 0, //1
        Komte     = 1 << 1, //2
        Aliac     = 1 << 2, //4
        Ortayn    = 1 << 3, //8
        Plyesma   = 1 << 4, //16
        Nogylath  = 1 << 5, //32
        Stakolyn  = 1 << 6, //64
        Baleor    = 1 << 7, //128
        Auxianteh = 1 << 8  //256
    }

    /// <summary>
    /// Holds the statistics of the skill
    /// </summary>
    [Serializable] public class Statistics
    {
        #region Fields
        /// <summary>
        /// Type of the attack; interacts with <see cref="BattleEntity.Statistics.Weakness"/> and <see cref="BattleEntity.Statistics.Resists"/>
        /// </summary>
        public EType Type;

        /// <summary>
        /// Damage value of the attack
        /// </summary>
        public float Damage;

        /// <summary>
        /// Speed of the attack
        /// </summary>
        public float DppCost;

        /// <summary>
        /// The possible targets
        /// </summary>
        public TargetConditionClass.ETargetGroup TargetGroup;
        #endregion
    }
    #endregion

    #region Fields
    /// <summary>
    /// Name of the attack
    /// </summary>
    public string Name;
    /// <summary>
    /// The statistics of this skill
    /// </summary>
    public Statistics Stats;
    /// <summary>
    /// Who can be targetted by this skill?
    /// </summary>
    public Func<BattleEntity, BattleEntity, bool> TargetCondition
    {
        get { return TargetConditionClass.GetFunc(Stats.TargetGroup); }
    }

    #endregion

    #region Constructor
    /// <summary>
    /// The default constructor no one uses
    /// </summary>
    public Skill() {}

    /// <summary>
    /// The good kind of constructor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="stats"></param>
    public Skill(string name, Statistics stats)
    {
        Name = name;
        Stats = stats;
    }
    #endregion

    #region Methods

    public override string ToString()
    {
        return string.Format("{0} (Skill)", Name);
    }

    #endregion
}
