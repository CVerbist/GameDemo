using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Hooks : AssetPostprocessor {

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        if (importedAssets.Any(file => file.EndsWith(EditorGlobals.EntityFile) || file.EndsWith(EditorGlobals.SkillFile)))
            MenuItems.PopulateGlobals();
    }


}
