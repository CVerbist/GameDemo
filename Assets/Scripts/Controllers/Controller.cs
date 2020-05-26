using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Util;
using Random = UnityEngine.Random;

/// <summary>
/// The controller controls the flow of the game and keeps all necessary info with him,
/// for example info to pass to the <see cref="BattleArenaManger"/>.
/// </summary>
public class Controller : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// State of the game
    /// </summary>
    public enum EGameState
    {
        Battle
    }
    #endregion

    #region Fields
    /// <summary>
    /// The battle controller
    /// </summary>
    [Header("States")]
    public BattleController BattleController;

    /// <summary>
    /// The current game state
    /// </summary>
    [SerializeField] private EGameState _gameState = EGameState.Battle;
    /// <summary>
    /// If the game is running or not
    /// </summary>
    public bool _running = true;

    /// <summary>
    /// The player object
    /// </summary>
    [Header("Entities")]
    public GameObject Player;

    /// <summary>
    /// The current region the game is in
    /// </summary>
    [Header("Location")]
    public Arena.ERegion Region;
    /// <summary>
    /// The current area of the region the game is in
    /// </summary>
    public string Area;
    #endregion

    #region Properties
    /// <summary>
    /// Change the state of the game. Also starts the transition.
    /// </summary>
    public EGameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Make sure the object isn't removed when switching scenes and disabled all other controllers
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        BattleController.gameObject.SetActive(false);
    }

    /// <summary>
    /// Contains the whole logic of GameController
    /// </summary>
    IEnumerator Start()
    {
        // Initialize stuff
        BattleController.BattleEndWin += OnBattleWin;
        BattleController.BattleEndLose += OnBattleLose;
        yield return null; // Wait a frame so the others can initialize first

        // Make sure the correct controller is enabled
        BattleController.gameObject.SetActive(_gameState == EGameState.Battle);
        InitBattle();
    }
    #endregion

    #region State changes

    /// <summary>
    /// Initialisation for battle, should be invisible, but isn't yet
    /// </summary>
    private void InitBattle()
    {
        // Turn off gravity
        //Physics2D.gravity = Vector2.zero;
        BattleController.Reset();

        SpawnArena();
        SpawnAllies();
        SpawnEnemies();
    }

    protected virtual void SpawnArena()
    {
        BattleController.Arena = Instantiate(Globals.Instance.Arenas.Random()).GetComponent<Arena>();

    }

    protected virtual void SpawnAllies()
    {
        //TODO: change to reposition existing player from platform
        Player = BattleController.SpawnAlly(Player.name);
        Player.GetComponent<BattleEntity>().IsAlly = true; // make sure it's an ally
        Player.name = "Joshua";
    }

    protected virtual void SpawnEnemies()
    {
        var spawned = new Dictionary<string, int>(); // holds how many times an entity was spawned for naming
        // Get the possible spawns for this arena
        var spawnable = Globals.Instance.Entities.GetFrom(BattleController.Arena.Region, BattleController.Arena.Area);
        int n = Random.Range(0, BattleController.Arena.EnemySpawns.Count);
        for (int i = 0; i <= n; i++) {
            var chosen = spawnable.Random();
            var enemy = BattleController.SpawnEnemy(chosen.Name);
            if (!spawned.ContainsKey(chosen.Name)) {
                spawned[chosen.Name] = 1;
            } else {
                spawned[chosen.Name]++;
            }
            enemy.name = chosen.Name + spawned[chosen.Name];
        }

    }
    #endregion

    #region Events
    /// <summary>
    /// Code to execute when the battle was won
    /// </summary>
    public IEnumerator OnBattleWin()
    {
        Application.Quit();
        yield return null;
    }

    /// <summary>
    /// Code to execute when the battle was lost :(
    /// </summary>
    public IEnumerator OnBattleLose()
    {
        Application.Quit();
        yield return null;
    }
    #endregion
}
