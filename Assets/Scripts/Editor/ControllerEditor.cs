using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    private const string Prefix = "Assets/Prefabs/";
    private const string AllyFolder = "Allies";
    private const string EnemyFolder = "Enemies";


    private Controller _self;

    /// <summary>
    /// Immediately gets the contents of the file and serializes it to the given type with checks in-between
    /// </summary>
    /// <typeparam name="T">The type to serialize to</typeparam>
    /// <param name="file">Path to the textfile containing the json text</param>
    /// <param name="exceptions">Should exceptions be thrown of default(T) returned</param>
    /// <returns>The serialized object</returns>
    private static T FromJsonFile<T>(string file, bool exceptions =true)
    {
        // First load the text file
        var json = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
        if (json == null) // check if succeeded
        {
            var msg = string.Format("Could not find {0}", file);
            if (exceptions)
                throw new FileLoadException(msg);
            Debug.LogError(msg);
            return default(T);
        }
        // Serialize the json string
        var obj = JsonUtility.FromJson<T>(json.text);
        if (obj == null) // check if succeeded
        {
            var msg = string.Format("Could not deserialize {0}", file);
            if (exceptions)
                throw new NullReferenceException(msg);
            Debug.LogError(msg);
        }
        return obj;
    }
}