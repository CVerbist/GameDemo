using System;
using UnityEngine;

public class WaitForCondition : CustomYieldInstruction
{
    private readonly Func<bool> _condition;

    /// <summary>
    /// Property which is called each frame. Do not call manually, Unity handles this.
    /// </summary>
    public override bool keepWaiting
    {
        get { return !_condition(); }
    }

    /// <summary>
    /// Wait until this predicate returns true
    /// </summary>
    /// <param name="condition"></param>
    public WaitForCondition(Func<bool> condition)
    {
        _condition = condition;
    }
}