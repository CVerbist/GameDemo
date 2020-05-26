using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItems : Editor {

    #region Auto/Auto-populate Globals on hierarchy change
    [MenuItem("Auto/Auto-populate Globals on hierarchy change")]
    public static void AutoPopulateGlobals()
    {
        EditorApplication.hierarchyWindowChanged += PopulateGlobals;
    }
    #endregion

    #region Prefabs/Populate Globals
    [MenuItem("Prefabs/Populate Globals")]
    public static void PopulateGlobals() {
        GetLevels();
        GetArenas();
        GetSkills();
        GetEntities();
    }


    private static void GetLevels() {
        // nope
    }

    private static void GetArenas() {
        var files = Directory.GetFiles(EditorGlobals.ArenaFolder, "*.prefab", SearchOption.TopDirectoryOnly);
        Globals.Instance.Arenas = files.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToList();
    }


    private static void GetSkills() {
        Globals.Instance.Skills = EditorUtil.FromJsonFile<SkillContainer>(EditorGlobals.SkillFile);
    }

    private static void GetEntities()
    {
        // Get the battle UI
        Globals.Instance.BattleUI = AssetDatabase.LoadAssetAtPath<GameObject>(EditorGlobals.BattleUIPrefab);

        // Get all prepared editor entity prefabs
        var files = Directory.GetFiles(EditorGlobals.EntityFolder, "*.prefab", SearchOption.TopDirectoryOnly);
        var entities = files.Select(AssetDatabase.LoadAssetAtPath<GameObject>);
        // Load entities.json
        var stats = EditorUtil.FromJsonFile<BattleEntityContainer>(EditorGlobals.EntityFile);
        // Load up the entities
        Globals.Instance.Entities = new BattleEntityContainer(entities, stats);
    }
    #endregion

    #region Prefabs/Create prefabs
    [MenuItem("Prefabs/Create prefabs")]
    private static void CreateEntityPrefabs() {
        GetSkills();
        GetEntities();
        Directory.CreateDirectory(EditorGlobals.EntityPrefabFolder);
        foreach (var entity in Globals.Instance.Entities) {
            var clone = entity.Instantiate(Vector3.zero, Quaternion.identity);
            PrefabUtility.CreatePrefab(EditorGlobals.EntityPrefabFolder + "/" + entity.Name + ".prefab", clone);
            DestroyImmediate(clone);
        }
    }

    #endregion
}
