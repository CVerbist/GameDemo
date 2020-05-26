using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global variables for everyone! Use as Globals.Instance.&lt;var&gt;
/// </summary>
[Serializable]
public class Globals : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The only Globals object there is
    /// </summary>
    private static Globals _self;

    /// <summary>
    /// The controller of the game
    /// </summary>
    public Controller Controller;

    /// <summary>
    /// The controller for the battle
    /// </summary>
    public BattleController BattleController;

    /// <summary>
    /// The controller for the platform
    /// </summary>
    //public PlatformController PlatformController;

    /// <summary>
    /// The battle UI that is attached to any entity when created. Currenty used as a workaround since static variables cannot be serialized :(
    /// </summary>
    public GameObject BattleUI;

    /// <summary>
    /// The complete list of all levels in the game
    /// </summary>
    [Header("Containers")]
    public List<GameObject> Levels;

    /// <summary>
    /// The complete list of all arena's in the game
    /// </summary>
    public List<GameObject> Arenas;

    /// <summary>
    /// All skills in the game in a handy container
    /// </summary>
    public SkillContainer Skills;

    /// <summary>
    /// List of all BattleEntities in the game in a handy container
    /// </summary>
    public BattleEntityContainer Entities;

    #endregion

    #region Properties
    /// <summary>
    /// Get the global instance exisiting in the scene, or create one if not it does not yet exist
    /// </summary>
    public static Globals Instance
    {
        get
        {
            if (_self != null)
                return _self;
            _self = FindObjectOfType<Globals>();
            if (_self != null)
                return _self;
            var controller = FindObjectOfType<Controller>();
            if (controller != null)
            {
                _self = controller.gameObject.AddComponent<Globals>();
                _self.Controller = controller;
                _self.BattleController = FindObjectOfType<BattleController>();
               // _self.PlatformController = FindObjectOfType<PlatformController>();
            }
            var newObj = new GameObject("Globals");
            return _self = newObj.AddComponent<Globals>();
            //return _self ?? (_self = FindObjectOfType<Globals>());
        }
    }

    #endregion

}