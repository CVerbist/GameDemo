using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BattleEntityContainer : IEnumerable<BattleEntityInfo>
{
    #region Fields
    /// <summary>
    /// The complete list of all entities in the game (might get broken up in parts later in development)
    /// </summary>
    [SerializeField]
    private List<BattleEntityInfo> _entities = new List<BattleEntityInfo>();

    #endregion

    #region Properties
    /// <summary>
    /// Get the <see cref="BattleEntityInfo"/> by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BattleEntityInfo this[string name]
    {
        get { return _entities.First(e => e.Name == name); }
    }

    /// <summary>
    /// Get the <see cref="BattleEntityInfo"/> by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public BattleEntityInfo this[int index]
    {
        get { return _entities[index]; }
    }

    /// <summary>
    /// Number of entities residing in <see cref="_entities"/>
    /// </summary>
    public int Count
    {
        get { return _entities.Count; }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Default constructor. Add entities with <see cref="Add"/>
    /// </summary>
    public BattleEntityContainer() {}

    /// <summary>
    /// Constructor to immediately add some entities
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="infos"></param>
    public BattleEntityContainer(IEnumerable<GameObject> entities, IEnumerable<BattleEntityInfo> infos)
    {
        List<BattleEntityInfo> copyStats = infos.ToList(); //copy so we don't alter the original list

        foreach (var entity in entities) //go through every entity
        {
            var stat = copyStats.FirstOrDefault(s => s.Name == entity.name); //find matching info
            if (stat == null) // if it does not exist
            {
                Debug.LogErrorFormat("Entity {0} does not have a matching info", entity.name);
                continue;
            }
            Add(stat, entity); // add to container
            copyStats.Remove(stat); // remove from copy
        }

        foreach (var stat in copyStats) //if too much info values
        {
            Debug.LogWarningFormat("Stat {0} does not have any matching entity", stat.Name);
        }
    }

    /// <summary>
    /// Private constructor, assumes all contained <see cref="BattleEntityInfo"/> are correct
    /// </summary>
    /// <param name="infos"></param>
    private BattleEntityContainer(IEnumerable<BattleEntityInfo> infos)
    {
        _entities = infos.ToList();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Adds a new entity to the list
    /// </summary>
    /// <param name="info"></param>
    /// <param name="prefab"></param>
    public void Add(BattleEntityInfo info, GameObject prefab)
    {
        if (prefab.name != info.Name)
            throw new ArgumentException("prefab and info need to have the same name");
        info.EditorPrefab = prefab;
        _entities.Add(info);
    }

    /// <summary>
    /// Get the list of entities residing in <see cref="area"/> of <see cref="region"/>
    /// </summary>
    /// <param name="region">The region in the game</param>
    /// <param name="area">(Optional) Further specify where to look</param>
    /// <returns></returns>
    public BattleEntityContainer GetFrom(Arena.ERegion region, string area =null)
    {
        var result = _entities.Where(e => (e.Region & region) != 0);
        if (!string.IsNullOrEmpty(area))
            result = result.Where(e => e.Area.Contains(area));
        return new BattleEntityContainer(result);
    }

    /// <summary>
    /// Instantiates the entity by name using the bound stats
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject Instantiate(string name) {
        return _entities.First(e => e.Name == name).Instantiate();
    }

    /// <summary>
    /// Instantiates the entity by index number using the bound stats
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject Instantiate(int index) {
        return _entities[index].Instantiate();
    }

    /// <summary>
    /// Returns the object enumerator of this container
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns the generic enumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator<BattleEntityInfo> GetEnumerator()
    {
        return _entities.GetEnumerator();
    }
    #endregion

}
