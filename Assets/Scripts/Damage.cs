using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Damage
{
    #region Fields
    /// <summary>
    /// Multiplier if entity is weak against the attack
    /// </summary>
    public const float WeaknessMultiplier = 1.5f;

    /// <summary>
    /// Multiplier if entity is resistant against the attack
    /// </summary>
    public const float ResistMultiplier = .75f;

    /// <summary>
    /// Multiplier if there is a critical
    /// </summary>
    public const float CritMultiplier = 1.2f;
    #endregion

    #region Method
    /// <summary>
    /// Calculates the damage done to entity with 1 damage as a minimum
    /// </summary>
    /// <param name="source">The attacking entity</param>
    /// <param name="target">The targeted entity</param>
    /// <param name="skill">The used skill</param>
    /// <returns></returns>
    public static float DamageDone(BattleController.BattleEntityMoveInfo info)
    {
        string events;
        return DamageDone(info, out events);
    }

    /// <summary>
    /// Calculates the damage done to entity with 1 damage as a minimum
    /// </summary>
    /// <param name="source">The attacking entity</param>
    /// <param name="target">The targeted entity</param>
    /// <param name="skill">The used skill</param>
    /// <returns></returns>
    public static float DamageDone(BattleController.BattleEntityMoveInfo info, out string events)
    {
        events = "";
        // Check if skill is healing skill or not
        if (info.Skill.Stats.Damage < 0)
            return info.Skill.Stats.Damage; //TODO: add heal scaling

        // Roll dodge
        if (Random.value < info.Target.Stats.Dodge)
        {
            events = " (dodged)";
            return 0;
        }
        // Calculate damage
        var atk = info.Skill.Stats.Type == Skill.EType.Strength ? info.Source.Stats.Strength : info.Source.Stats.DrantoPegis;
        var def = info.Skill.Stats.Type == Skill.EType.Strength ? info.Target.Stats.Defense : info.Target.Stats.DrantoPegisDefense;
        var weak = (info.Skill.Stats.Type & info.Target.Stats.Weakness) != 0 ? WeaknessMultiplier : 0;
        var resist = (info.Skill.Stats.Type & info.Target.Stats.Resists) != 0 ? ResistMultiplier : 0;
        var result = Mathf.Max(info.Skill.Stats.Damage*atk - def + weak - resist, 1f);

        // Roll crit
        if (Random.value < info.Target.Stats.CritChance)
        {
            result *= CritMultiplier;
            events = " (crit)";
        }
        return result;
    }

    /// <summary>
    /// Sets the new health of target using <see cref="DamageDone"/>.
    /// </summary>
    /// <param name="source">The attacking entity</param>
    /// <param name="target">The targeted entity</param>
    /// <param name="skill">The used skill</param>
    public static void ResolveHealth(BattleController.BattleEntityMoveInfo info)
    {
        string events;
        var dmg = DamageDone(info, out events);
        Debug.Log(string.Format("<b>{0} {1}</b> targets <b>{2}</b> at {3} <b>{4}</b> and does <b>{5} damage</b>{6}",
                                info.Source.IsAlly ? "Ally" : "Enemy",
                                info.Source.name,
                                info.Skill.Name,
                                info.Target.IsAlly ? "Ally" : "Enemy",
                                info.Target.name,
                                dmg,
                                events));
        info.Target.Hp -= dmg;
    }

    /// <summary>
    /// Sets the new health of <paramref name="target"/>.
    /// </summary>
    /// <param name="target">The targeted entity</param>
    /// <param name="skill">The used skill</param>
    /// <remarks>DamageDone and ResolveHealth now fills this purpose.</remarks>
    [Obsolete]
    public static void HealHealth(ref BattleEntity target, Skill skill)
    {
        target.Hp += skill.Stats.Damage;
    }

    /// <summary>
    /// Sets the new DPP of <paramref name="target"/>
    /// </summary>
    /// <param name="target">The targeted entity</param>
    /// <param name="skill">The used skill</param>
    public static void ResolveDpp(BattleController.BattleEntityMoveInfo info)
    {
        info.Source.Dpp -= info.Skill.Stats.DppCost;
    }
    #endregion
}
