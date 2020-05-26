using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class EditorGlobals
{
    // Base folders
    public const string PrefabFolder = "Assets/Prefabs";
    public const string ResourceFolder = "Assets/Resources";

    // Entity files and folders
    public const string EntityFolder = PrefabFolder + "/Entities";
    public const string EntityFile = PrefabFolder + "/Entities.json";
    public const string EntityPrefabFolder = EntityFolder + "/Complete";
    public const string BattleUIPrefab = EntityFolder + "/Other/BattleUI.prefab";

    // Skill files and folders
    public const string SkillFile = PrefabFolder + "/Skills.json";

    // Other files or folders
    public const string ArenaFolder = PrefabFolder + "/Arenas";
}
