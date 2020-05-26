using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class BattleEntity : MonoBehaviour
{
    #region Types
    /// <summary>
    /// Stats associated with a <see cref="BattleEntity"/>.
    /// </summary>
    [Serializable]
    public struct Statistics
    {
        #region Fields
        /// <summary>
        /// The maximum amount of HP of an entity.
        /// </summary>
        public float MaxHp;

        /// <summary>
        /// The maximum amount of DPP of an entity.
        /// </summary>
        public float MaxDpp;

        /// <summary>
        /// Physical attack strength
        /// </summary>
        public float Strength;

        /// <summary>
        /// Physical defense
        /// </summary>
        public float Defense;

        /// <summary>
        /// Magic attack strength
        /// </summary>
        public float DrantoPegis;

        /// <summary>
        /// Magic defense strength
        /// </summary>
        public float DrantoPegisDefense;

        /// <summary>
        /// Chance to do bonus damage
        /// </summary>
        public float CritChance;

        /// <summary>
        /// Chance to nullify damage
        /// </summary>
        public float Dodge;

        /// <summary>
        /// Speed of the entity. Determines initial turn.
        /// </summary>
        public float Speed;

        /// <summary>
        /// Weakness against certain attack types
        /// </summary>
        public Skill.EType Weakness;

        /// <summary>
        /// Resistance against certain attack types
        /// </summary>
        public Skill.EType Resists;
        #endregion

        #region Methods
        /// <summary>
        /// Override for ToString!
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "{0}|{1}/{2}|{3}/{4}|{5} / {6}|{7}|{8}", MaxHp,
                MaxDpp, Strength, Defense, DrantoPegis, DrantoPegisDefense, CritChance, Dodge, Speed);
        }

        #endregion
    }
    #endregion

    #region Fields

    [Header("References")]
    public BattleUI BattleUI;

    [Header("Properties")]
    [SerializeField]
    private float _hp;
    [SerializeField]
    private float _dpp;

    /// <summary>
    /// Current statistics of an entity
    /// </summary>
    public Statistics Stats = new Statistics();

    /// <summary>
    /// Is this entity currently an ally?
    /// </summary>
    public bool IsAlly;

    /// <summary>
    /// The skillset of the entity. For enemies, this is entered via prefabs.
    /// </summary>
    public List<string> Skillset = new List<string>();

    /// <summary>
    /// The AI ruling this entity if it is an enemy
    /// </summary>
    public BattleAI BattleAI;

    public event Action Hit;
    #endregion

    #region Properties
    /// <summary>
    /// Name of the entity (same as name in the hierarchy)
    /// </summary>
    public string Name
    {
        get { return gameObject.name; }
        set { gameObject.name = value; }
    }

    /// <summary>
    /// Current hp value for this entity
    /// </summary>
    public float Hp
    {
        get { return _hp; }
        set
        {
            _hp = Mathf.Clamp(value, 0, Stats.MaxHp);
            BattleUI.HealthBar.fillAmount = value / Stats.MaxHp;
        }
    }

    /// <summary>
    /// Current dpp value for this entity
    /// </summary>
    public float Dpp
    {
        get { return _dpp; }
        set
        {
            _dpp = Mathf.Clamp(value, 0, Stats.MaxDpp);
            BattleUI.DppBar.fillAmount = value / Stats.MaxDpp;
        }
    }

    /// <summary>
    /// Is this entity dead?
    /// </summary>
    public bool IsDead
    {
        get { return Hp <= 0; }
    }

    /// <summary>
    /// Inverse of <see cref="IsAlly"/>
    /// </summary>
    public bool IsEnemy
    {
        get { return !IsAlly; }
        set { IsAlly = !value; }
    }

    /// <summary>
    /// Allows to highlight or unhighlight the entity
    /// </summary>
    //public bool Highlight
    //{
    //    get { return BattleUI.HighlighterObject.activeSelf; }
    //    set
    //    {
    //        BattleUI.HighlighterObject.SetActive(value);
    //        GetComponent<ClickableOnEnable>().enabled = value;
    //    }
    //}

    #endregion

    #region Initialisation
    void Start()
    {
        BattleAI = new BattleAI(this);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Applies the stats (and skillset) to the entity
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="skillset"></param>
    public void AssignStats(Statistics stats, string[] skillset =null)
    {
        Stats = stats;
        _hp = Stats.MaxHp;
        _dpp = Stats.MaxDpp;
        Skillset.Clear();
        if (skillset == null)
            return;
        foreach (var skill in skillset)
        {
            if (Globals.Instance.Skills.All(s => s.Name != skill))
                Debug.LogError(string.Format("Cannot assign unexisting skill {0} to entity {1}", skill, Name));
            else
                Skillset.Add(skill);
        }
    }

    /// <summary>
    /// Returns true if <paramref name="other"/> is an ally of itself
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsAllyOf(BattleEntity other)
    {
        return IsAlly == other.IsAlly;
    }

    /// <summary>
    /// Returns true if <see cref="other"/> is an enemy of itself
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsEnemyOf(BattleEntity other)
    {
        return !IsAllyOf(other);
    }

    /// <summary>
    /// A co-routine that shows the UI and returns the selected skill as last return value
    /// </summary>
    /// <returns></returns>
    public IEnumerator ChoosePlayerSkill()
    {
        //TODO: implement ExecutePlayerTurn
        // Show UI
        // Wait for player to select a skill click
        //Return the chosen skill
        var result = Globals.Instance.Skills[Skillset.Random()];
        yield return result;
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Chooses a move that the AI will use
    /// </summary>
    /// <param name="intelligence"></param>
    /// <returns></returns>
    public IEnumerator ChooseEnemySkill()
    {
        //TODO: Implement Enemy AI
        var result = Globals.Instance.Skills[Skillset.Random()];
        yield return result;
    }

    /// <summary>
    /// Starts attack animation and only finishes when the animation is complete
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="hitAction">Action to take when hit event was triggered in the animation</param>
    /// <returns></returns>
    public IEnumerable Attack(Skill skill)
    {   //TODO: implement execution of attack
        // If not in skillset
        if (!Skillset.Contains(skill.Name))
            throw new ArgumentOutOfRangeException("skill");

        var animator = GetComponent<Animator>();
        // Start animation
        //animator.setString(skill.Name); // setString bestaat niet :(
        //yield return <something>; // <- wait for completion of animation
        yield return null; // Actually finish the coroutine (not sure if necessary, but probable)
    }

    /// <summary>
    /// The hit event, subscribed to by <see cref="BattleController"/> and executed by the event
    /// </summary>
    public void HitEvent()
    {
        if (Hit != null)
            Hit.Invoke();
    }

    /// <summary>
    /// When the entity needs to die
    /// </summary>
    public void Die()
    {
        //TODO: change to animation
        GetComponent<SpriteRenderer>().color = Color.black;
    }

    /// <summary>
    /// Handy identification via the debugger
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("{0} (Entity)", Name);
    }

    #endregion
}
