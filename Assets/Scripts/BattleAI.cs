using System.Collections.Generic;
using System.Linq;
using Util;

/// <summary>
/// AI thinking process, inherit from this and override <see cref="ChooseSkillAndTarget"/> to set behaviour. This AI default to random all the way.
/// </summary>
public class BattleAI {

    #region Fields
    /// <summary>
    /// Shared battle info for everybody!
    /// </summary>
    public static List<BattleController.BattleEntityMoveInfo> BattleInfo;

    /// <summary>
    /// Reference to self
    /// </summary>
    public BattleEntity _self;
    #endregion

    #region Properties

    #endregion

    #region Constructor
    public BattleAI(BattleEntity self) {
        _self = self;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Chooses a skill and a target according to its logic
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="target"></param>
    public virtual void ChooseSkillAndTarget(out Skill skill, out BattleEntity target) {
        skill = Globals.Instance.Skills[_self.Skillset.Random()];
        target = BattleInfo.Random(bi => bi.Source.IsEnemyOf(_self)).Source;
    }
    #endregion
}
