using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.UI;
using Util;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class BattleController : MonoBehaviour {
    #region Types
    /// <summary>
    /// Helper struct to bind a <see cref="Skill"/> and target <see cref="BattleEntity"/> to a <see cref="BattleEntity"/>
    /// </summary>
    public class BattleEntityMoveInfo {
        public BattleEntity Source;
        public Skill Skill;
        public BattleEntity Target;

        public BattleEntityMoveInfo(BattleEntity entity) {
            Source = entity;
            Skill = null;
            Target = null;
        }
    }
    #endregion

    #region Fields
    /// <summary>
    /// The battle ends at the end of a turn if this is set to true
    /// </summary>
    public bool BattleEnd = false;

    /// <summary>
    /// The current arena
    /// </summary>
    private Arena _arena;

    /// <summary>
    /// List of all entities and the move they will perform
    /// </summary>
    private List<BattleEntityMoveInfo> _entityBattleInfos = new List<BattleEntityMoveInfo>();

    /// <summary>
    /// Maximum amount of allies this arena can hold
    /// </summary>
    public int MaxAllies;

    /// <summary>
    /// Maximum amount of enemies this arena can hold
    /// </summary>
    public int MaxEnemies;

    /// <summary>
    /// Maximum amount of total entities this arena can hold
    /// </summary>
    public int MaxEntities;

    /// <summary>
    /// The current turn number (used in debug messages)
    /// </summary>
    private int _turnNumber = 0;

    /// <summary>
    /// What happens when the battle was won?
    /// </summary>
    public event Func<IEnumerator> BattleEndWin;

    /// <summary>
    /// What happens when the battle was lost
    /// </summary>
    public event Func<IEnumerator> BattleEndLose;
    #endregion

    #region Properties
    /// <summary>
    /// The arena holding important values
    /// </summary>
    public Arena Arena {
        get { return _arena; }
        set { _arena = value; }
    }

    /// <summary>
    /// All entities currently in the arena (including dead ones)
    /// </summary>
    private List<BattleEntityMoveInfo> Entities {
        get { return _entityBattleInfos; }
    }

    /// <summary>
    /// The list of all allies currently in the arena
    /// </summary>
    /// <remarks>Should be exactly the same as scanning the Are</remarks>
    private List<BattleEntityMoveInfo> Allies {
        get { return _entityBattleInfos.Where(e => e.Source.IsAlly).ToList(); }
    }

    /// <summary>
    /// The list of all enemies currently in the arena
    /// </summary>
    private List<BattleEntityMoveInfo> Enemies {
        get { return _entityBattleInfos.Where(e => !e.Source.IsAlly).ToList(); }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Used to initialize some fields. Should maybe be moved to OnEnable?
    /// </summary>
    IEnumerator Start() {
        // First instantiate some stuff
        var spawnInfos = GetComponentsInChildren<SpawnInfo>();
        MaxEntities = spawnInfos.Length;
        MaxAllies = spawnInfos.Count(info => info.IsAllySpawn);
        MaxEnemies = spawnInfos.Count(info => info.IsEnemySpawn);

        // Set other important stuff
        SetTurnorder();
        //ShowTurnOder();
        BattleAI.BattleInfo = _entityBattleInfos;

        yield return null; // wait a frame to make sure the rest is initialized

        // Start of coroutines
        while (!BattleEnd)
        {
            // TODO: Put button to confirm your moves
            yield return PrepTurn();
            yield return ExecuteTurn();
            yield return EndTurn();
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void ShowTurnOder() {
        for (int i = 0; i < _entityBattleInfos.Count; i++) {
            _entityBattleInfos[i].Source.GetComponentInChildren<Text>().text = "(" + i + ")";
        }
    }

    /// <summary>
    /// Makes sure all coroutines are stopped when this script is disabled
    /// </summary>
    void OnDisable() {
        StopAllCoroutines();
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// All entities choose their skill and target
    /// </summary>
    /// <returns></returns>
    private IEnumerator PrepTurn() {
        Debug.Log(string.Format("===== TURN {0} =====", _turnNumber++));

        // To check if damage and dpp was correctly handled. TODO: Need to remove after!
        foreach (BattleEntityMoveInfo currEnt in _entityBattleInfos)
        {
            Debug.Log("The Current entity is: " + currEnt.Source + ", his health is: " + currEnt.Source.Hp + ", his DPP is " + currEnt.Source.Dpp);
        }

        foreach (BattleEntityMoveInfo selecEnemy in Enemies)
        {
            if (!selecEnemy.Source.IsDead)
            {
                // Choose the skill you for the enemy
                var selecSkillEnemy = new CoroutineData<Skill>(this, selecEnemy.Source.ChooseEnemySkill());
                yield return selecSkillEnemy.Coroutine;
                selecEnemy.Skill = selecSkillEnemy.Result;

                // Choose target for skill
                selecEnemy.Target = Allies.Random().Source;
                Debug.Log("The selected Enemy: " + selecEnemy.Source + " , the selected Skill is: " + selecEnemy.Skill +
                          " , the selected target is: " + selecEnemy.Target);
            }
        }

        while (_entityBattleInfos.Any(e => e.Source.IsAlly && e.Target == null))
        {
            var cdTarget = new CoroutineData<BattleEntityMoveInfo>(this, SelectOrigin());
            yield return cdTarget.Coroutine;
            var selecAlly = cdTarget.Result;
            Debug.Log("The selected Ally: " + selecAlly.Source + " , the selected Skill is: " + selecAlly.Skill +
                      " , the selected target is: " + selecAlly.Target);
        }

        Debug.Log("All entities have a target.");
    }

    /// <summary>
    /// Executes a whole turn: animate, calculate damage etc
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecuteTurn() {
        foreach (BattleEntityMoveInfo currEnt in _entityBattleInfos)
        {
            if (!currEnt.Source.IsDead)
            {
                Damage.ResolveHealth(currEnt);
                Damage.ResolveDpp(currEnt);
         //       Debug.Log(ebi.Source.name + " is fighting for his life");
         //       continue;
            }
         //   ebi.Source.Highlight = true;
         //   Action onHit = () => Damage.ResolveHealth(ebi);
         //   ebi.Source.Hit += onHit;
         //   yield return ebi.Source.Attack(ebi.Skill);
         //   ebi.Source.Hit -= onHit;
         //   CheckForDeads();
         //   yield return new WaitForSeconds(.2f);
         //   ebi.Source.Highlight = false;
        }
        yield return null;
    }

    /// <summary>
    /// Check if battle is over
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndTurn() {
        // Check if all allies are dead
        if (Entities.Where(e => e.Source.IsAlly).All(e => e.Source.IsDead)) {
            yield return ShowLoseScreen();
            yield return BattleEndLose.Invoke();
            BattleEnd = true;
        }
        // Check if all enemies are dead
        else if (Entities.Where(e => e.Source.IsEnemy).All(e => e.Source.IsDead)) {
            yield return ShowWinScreen();
            yield return BattleEndWin.Invoke();
            // leave 'Possible NullReferenceException'. These events MUST be subscribed to by Controller
            BattleEnd = true;
        }

        // Resetting everyones target and selected skill.
        foreach (BattleEntityMoveInfo allEntities in _entityBattleInfos)
        {
            allEntities.Skill = null;
            allEntities.Target = null;
        }
    }

    /// <summary>
    /// Let the user select a target
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelectOrigin()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                var hitCollider = Physics2D.OverlapPoint(mousePosition);
                if (hitCollider && hitCollider.tag == "Player")
                {
                    var selec = _entityBattleInfos.First(e => e.Source.Name == hitCollider.gameObject.name);

                    //Choose the skill you want to execute
                    var selecSkill = new CoroutineData<Skill>(this, selec.Source.ChoosePlayerSkill());
                    yield return selecSkill.Coroutine;
                    selec.Skill = selecSkill.Result;

                    // Choose the target for your skill
                    yield return SelecTarget(selec);

                    yield return selec;
                    
                    yield break;
                }
            }
            
            yield return null;
        }
    }

    /// <summary>
    /// Choose a target for your skill
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelecTarget(BattleEntityMoveInfo sourceSelec)
    {
        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                var hitCollider = Physics2D.OverlapPoint(mousePosition);
                if (hitCollider && hitCollider.tag == "Enemy") // TODO: Change depending on whether it is healing or potion then ally should be targeted
                {
                    if (!_entityBattleInfos.First(e => e.Source.Name == hitCollider.gameObject.name).Source.IsDead)
                    {
                        var selecEn = _entityBattleInfos.First(e => e.Source.Name == hitCollider.gameObject.name);
                        sourceSelec.Target = selecEn.Source;
                        yield break;
                    }
                    else
                    {
                        Debug.Log("You choose a dead enemy, choose another target.");
                    }
                }
            }
            yield return null;
        }
    }


    /// <summary>
    /// Show stats/xp gain/... until user clicks next
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowWinScreen() {
        var obj = new GameObject("WinText", typeof(SpriteRenderer));
        obj.transform.position = Vector3.up * 4f;
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("YouWin");
        yield return new WaitForSeconds(1.5f);
    }

    /// <summary>
    /// Tells the player he sucks
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowLoseScreen() {
        var obj = new GameObject("LoseText", typeof(SpriteRenderer));
        obj.transform.position = Vector3.up * 4f;
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("YouLose");
        yield return new WaitForSeconds(1.5f);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Resets all values to its initial ones
    /// </summary>
    public void Reset() {
        _arena = null; //TODO: change to destroy
        _entityBattleInfos = new List<BattleEntityMoveInfo>();
    }

    /// <summary>
    /// Spawns the entity with name <paramref name="entityName"/> at <paramref name="spawnInfo"/>
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="spawnInfo"></param>
    /// <returns></returns>
    public GameObject Spawn(string entityName, SpawnInfo spawnInfo) {
        return Spawn(Globals.Instance.Entities[entityName], spawnInfo);
    }

    /// <summary>
    /// Spawns the entity <paramref name="entity"/> at <paramref name="spawnInfo"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="spawnInfo"></param>
    /// <returns></returns>
    public GameObject Spawn(BattleEntityInfo entity, SpawnInfo spawnInfo)
    {
        return Configure(entity.Instantiate(), spawnInfo);
    }

    /// <summary>
    /// Spawn a single ally
    /// </summary>
    /// <param name="ally"></param>
    public GameObject SpawnAlly(string ally) {
        return Spawn(Globals.Instance.Entities[ally], _arena.NextAllySpawn);
    }

    /// <summary>
    /// Spawns a single ally
    /// </summary>
    /// <param name="ally"></param>
    /// <returns></returns>
    public GameObject SpawnAlly(BattleEntityInfo ally)
    {
        return Spawn(ally, _arena.NextAllySpawn);
    }

    /// <summary>
    /// Spawns some allies, placed according to the Arena's rules.
    /// </summary>
    /// <param name="allies"></param>
    public IEnumerable<GameObject> SpawnAllies(IEnumerable<string> allies)
    {
        return SpawnAllies(allies.Select(a => Globals.Instance.Entities[a]));
    }

    /// <summary>
    /// Spawns some allies, placed according to the Arena's rules
    /// </summary>
    /// <param name="allies"></param>
    /// <returns></returns>
    public IEnumerable<GameObject> SpawnAllies(IEnumerable<BattleEntityInfo> allies)
    {
        var result = new List<GameObject>();
        foreach (var ally in allies)
            result.Add(SpawnAlly(ally));
        return result;
    }

    /// <summary>
    /// Spawn a single enemy by name <paramref name="enemy"/>
    /// </summary>
    /// <param name="enemy"></param>
    public GameObject SpawnEnemy(string enemy) {
        return Spawn(Globals.Instance.Entities[enemy], _arena.NextEnemySpawn);
    }

    /// <summary>
    /// Spawns a single enemy
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public GameObject SpawnEnemy(BattleEntityInfo enemy)
    {
        return Spawn(enemy, _arena.NextEnemySpawn);
    }

    /// <summary>
    /// Spawns some enemies, placed according to the Arena's rules
    /// </summary>
    /// <param name="enemies"></param>
    public IEnumerable<GameObject> SpawnEnemies(IEnumerable<string> enemies)
    {
        return SpawnEnemies(enemies.Select(e => Globals.Instance.Entities[e]));
    }

    /// <summary>
    /// Spawns some enemies, placed according to the Arena's rules
    /// </summary>
    /// <param name="enemies"></param>
    /// <returns></returns>
    public IEnumerable<GameObject> SpawnEnemies(IEnumerable<BattleEntityInfo> enemies)
    {
        var result = new List<GameObject>();
        foreach (var enemy in enemies)
            result.Add(SpawnEnemy(enemy));
        return result;
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// Spawn a <see cref="GameObject"/> with transform <paramref name="spawnInfo"/>
    /// </summary>
    /// <param name="spawn"></param>
    /// <param name="spawnInfo"></param>
    private GameObject Configure(GameObject spawn, SpawnInfo spawnInfo) {
        if (spawnInfo == null)
            throw new NullReferenceException("SpawnInfo is null. There is probably no space left to spawn this entity.");

        spawn.transform.position = spawnInfo.transform.position;

        // If enemy spawn, flip all X
        //TODO: improve the flipping
        if (!spawnInfo.IsAllySpawn) {
           // spawn.GetComponent<SpriteRenderer>().flipX = !spawn.GetComponent<SpriteRenderer>().flipX;
           // var battleUi = spawn.GetComponentInChildren<BattleUI>();
           // battleUi.HealthBar.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(180, Vector3.up);
           // battleUi.DppBar.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(180, Vector3.up);
           // battleUi.UiBarsBackgroundObject.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(180,
           //     Vector3.up); battleUi.SelectableObject.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(180,
           //      Vector3.up);

            spawn.GetComponent<BattleEntity>().IsEnemy = true;
        } else
            spawn.GetComponent<BattleEntity>().IsAlly = true;

        _entityBattleInfos.Add(new BattleEntityMoveInfo(spawn.GetComponent<BattleEntity>()));
        spawnInfo.Entity = spawn;
        return spawn;
    }

    /// <summary>
    /// Checks if anyone died and starts the animation
    /// </summary>
    private void CheckForDeads()
    {
        for (int i = 0; i < _entityBattleInfos.Count; i++)
        {
            var entity = _entityBattleInfos[i].Source;
            if (entity.IsDead)
                entity.Die();
        }
    }

    /// <summary>
    /// Changes the order of the elements of _entities to represent the turn order
    /// </summary>
    private void SetTurnorder()
    {
        // Failsafe against zero speeds
        if (_entityBattleInfos.Any(e => e.Source.Stats.Speed == 0f))
        {
            Debug.LogWarning("Not all speeds are set, skipping SetTurnorder()");
            return;
        }

        var result = new List<BattleEntityMoveInfo>();

        // Get total sum of speeds
        var sum = _entityBattleInfos.Sum(e => e.Source.Stats.Speed);
        if (sum == 0f)
            return; // Skip if no speeds are set
        // Copy dudes to new list so we can remove elements
        var leftovers = new List<BattleEntityMoveInfo>(_entityBattleInfos);
        // Keep iterating until list is empty
        while (leftovers.Count > 0)
        {
            // Choose a value in [0,1]
            var r = Random.value;
            // See in what interval it lies
            var lo = 0f;
            for (int j = 0; j < leftovers.Count; j++)
            {
                var hi = lo + leftovers[j].Source.Stats.Speed / sum;
                if (lo < r && r < hi)
                {
                    // Entity j was chosen
                    result.Add(leftovers[j]); // Add to result as next entity
                    sum -= leftovers[j].Source.Stats.Speed; // Correct sum to the sum without him
                    leftovers.RemoveAt(j); // Remove entity from pool
                    break;
                }
                lo = hi; // Search next region
            }
        }
        _entityBattleInfos = result;
    }
    #endregion
}
