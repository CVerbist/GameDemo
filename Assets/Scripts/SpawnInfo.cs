using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInfo : MonoBehaviour
{
    /// <summary>
    /// Is the spot reserved for allies?
    /// </summary>
    public bool IsAllySpawn;

    /// <summary>
    /// How quickly should this spot be filled? Priority 0 is the highest.
    /// </summary>
    [Range(0, 6)] public int Priority;

    /// <summary>
    /// The entity currently occupying the spot
    /// </summary>
    [SerializeField]
    private GameObject _entity;

    /// <summary>
    /// Is something already occupying this location?
    /// </summary>
    public bool IsEmpty
    {
        get { return Entity == null; }
    }

    /// <summary>
    /// Is the spot reserved for enemies?
    /// </summary>
    public bool IsEnemySpawn
    {
        get { return !IsAllySpawn; }
        set { IsAllySpawn = !value; }
    }

    /// <summary>
    /// The entity currently occupying the spot
    /// </summary>
    public GameObject Entity
    {
        get { return _entity; }
        set { _entity = value; }
    }
}
