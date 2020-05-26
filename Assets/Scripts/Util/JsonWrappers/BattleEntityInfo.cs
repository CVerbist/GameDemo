using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class BattleEntityInfo
{
    #region Fields
    /// <summary>
    /// The name of this entity
    /// </summary>
    public string Name;

    /// <summary>
    /// The complete set of stats of this entity. Is loaded up from Entities.json.
    /// </summary>
    public BattleEntity.Statistics Stats;

    /// <summary>
    /// The skillset this entity has (primarily used for enemies)
    /// </summary>
    public string[] SkillSet;

    /// <summary>
    /// Where this entity resides
    /// </summary>
    public Arena.ERegion Region;

    /// <summary>
    /// In what area of said region it resides (if applicable)
    /// </summary>
    public string Area;

    /// <summary>
    /// The basic prefab residing in Assets/Prefabs/Entities
    /// </summary>
    public GameObject EditorPrefab;
    #endregion

    #region Methods
    /// <summary>
    /// Instantiates the object onto the scene
    /// </summary>
    /// <returns></returns>
    public GameObject Instantiate()
    {
        if (EditorPrefab == null)
            throw new FieldAccessException("EditorPrefab is not yet assigned");

        return Configure(Object.Instantiate(EditorPrefab));
    }

    /// <summary>
    /// Instantiates the object onto the scene at <paramref name="position"/>
    /// </summary>
    /// <param name="position">Where the object is spawned</param>
    /// <param name="worldPositionStays"></param>
    /// <returns></returns>
    public GameObject Instantiate(Transform position, bool worldPositionStays = false)
    {
        if (EditorPrefab == null)
            throw new FieldAccessException("EditorPrefab is not yet assigned");

        return Configure(Object.Instantiate(EditorPrefab, position, worldPositionStays));
    }

    /// <summary>
    /// Instantiates the object onto the scene at <paramref name="position"/> with rotation given by <paramref name="quaternion"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    public GameObject Instantiate(Vector3 position, Quaternion quaternion)
    {
        if (EditorPrefab == null)
            throw new FieldAccessException("EditorPrefab is not yet assigned");

        return Configure(Object.Instantiate(EditorPrefab, position, quaternion));
    }

    /// <summary>
    /// Instantiates the object onto the scene at <paramref name="position"/> with rotation <paramref name="quaternion"/> as child of <paramref name="parent"/>
    /// </summary>
    /// <param name="position">Where the object needs to be spawned</param>
    /// <param name="quaternion">The rotational values of the object</param>
    /// <param name="parent">The transform parent of the spawning object</param>
    /// <returns></returns>
    public GameObject Instantiate(Vector3 position, Quaternion quaternion, Transform parent)
    {
        if (EditorPrefab == null)
            throw new FieldAccessException("EditorPrefab is not yet assigned");

        return Configure(Object.Instantiate(EditorPrefab, position, quaternion, parent));
    }

    /// <summary>
    /// Used for debugging purposes
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Entity " + Name;
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// Configures the just spawned object to contain the correct values
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private GameObject Configure(GameObject entity)
    {
        //Rigidbody2D
        //if (entity.GetComponent<Rigidbody2D>() == null)
        //{
        //    var rb = entity.AddComponent<Rigidbody2D>();
        //    rb.bodyType = RigidbodyType2D.Kinematic;
        //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //}

        //ClickableOnEnable
        //if (entity.GetComponent<ClickableOnEnable>() == null)
        //    entity.AddComponent<ClickableOnEnable>();

        //BattleEntity
        if (entity.GetComponent<BattleEntity>() == null)
        {
            var be = entity.AddComponent<BattleEntity>();
            be.AssignStats(Stats, SkillSet);
        }

        //Add the battle UI
        if (entity.GetComponentInChildren<BattleUI>() == null)
        {
            var center = entity.GetComponent<SpriteRenderer>().bounds.center;
            Object.Instantiate(Globals.Instance.BattleUI, center, Quaternion.identity, entity.transform);
        }

        //Make sure the battle ui is referenced in BattleEntity
        if (entity.GetComponent<BattleEntity>().BattleUI == null)
        {
            entity.GetComponent<BattleEntity>().BattleUI = entity.GetComponentInChildren<BattleUI>();
        }

        return entity;
    }
    #endregion
}
