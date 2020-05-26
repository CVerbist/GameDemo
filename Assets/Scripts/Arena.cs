using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Util;

/// <summary>
/// Holds the positions of the spawn locations of the 3 allies and 3 enemies on the arena
/// </summary>
public class Arena : MonoBehaviour
{
    #region Enums

    [Flags] public enum ERegion
    {
        None   = 0,
        Ice    = 1 << 0,
        Desert = 1 << 1,
        Forest = 1 << 2 //...
    }
    #endregion

    #region Fields
    /// <summary>
    /// Where the Arena is located.
    /// </summary>
    public ERegion Region;

    /// <summary>
    /// Area inside the region to distinguish in-between a region
    /// </summary>
    public string Area;

    /// <summary>
    /// A list of the <see cref="SpawnInfo"/> of the arena that guarantees it is ordered by priority
    /// </summary>
    private readonly SortedList<SpawnInfo> _spawns =
        new SortedList<SpawnInfo>(ComparerUtil<SpawnInfo>.Create((s1, s2) => s1.Priority.CompareTo(s2.Priority)));
    #endregion

    #region Properties
    /// <summary>
    /// All possible spawn locations, sorted by priority
    /// </summary>
    public SortedList<SpawnInfo> Spawns
    {
        get { return _spawns; }
    }

    /// <summary>
    /// Returns a list of all allied spawn points
    /// </summary>
    public List<SpawnInfo> AllySpawns
    {
        get { return _spawns.Where(s => s.IsAllySpawn).ToList(); }
    }

    /// <summary>
    /// Returns a list of all enemy spawn points
    /// </summary>
    public List<SpawnInfo> EnemySpawns
    {
        get { return _spawns.Where(s => !s.IsAllySpawn).ToList(); }
    }

    /// <summary>
    /// Returns the highest priority spawn for allies
    /// </summary>
    public SpawnInfo NextAllySpawn
    {
        get { return _spawns.FirstOrDefault(s => s.IsEmpty && s.IsAllySpawn); }
    }

    /// <summary>
    /// Returns the highest priority spawn for enemies
    /// </summary>
    public SpawnInfo NextEnemySpawn
    {
        get { return _spawns.FirstOrDefault(s => s.IsEmpty && !s.IsAllySpawn); }
    }

    /// <summary>
    /// All entities currently in the Arena
    /// </summary>
    public List<GameObject> Entities
    {
        get { return _spawns.Where(s => !s.IsEmpty).Select(s => s.Entity).ToList(); }
    }

    /// <summary>
    /// All allies currently in the Arena
    /// </summary>
    public List<GameObject> Allies
    {
        get
        {
            return
                _spawns.Where(s => !s.IsEmpty && s.Entity.GetComponent<BattleEntity>().IsAlly)
                       .Select(s => s.Entity)
                       .ToList();
        }
    }

    /// <summary>
    /// All enemies currently in the Arena
    /// </summary>
    public List<GameObject> Enemies
    {
        get
        {
            return
                _spawns.Where(s => !s.IsEmpty && !s.Entity.GetComponent<BattleEntity>().IsAlly)
                       .Select(s => s.Entity)
                       .ToList();
        }
    }
    #endregion

    #region Initialization
    public void Awake()
    {
        _spawns.AddRange(GetComponentsInChildren<SpawnInfo>());
    }
    #endregion
}