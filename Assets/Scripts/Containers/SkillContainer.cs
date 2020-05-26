using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class SkillContainer : IEnumerable<Skill>
{
    /// <summary>
    /// A copy of all the skills
    /// </summary>
    [SerializeField]
    private List<Skill> _skills;

    /// <summary>
    /// Default way of getting a skill: by name
    /// </summary>
    /// <param name="name">The name of the skill</param>
    /// <returns>The skill object</returns>
    public Skill this[string name]
    {
        get { return _skills.FirstOrDefault(s => s.Name == name); }
    }

    /// <summary>
    /// Use to enumerate through all the skills
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Use to enumerate through all the skills
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Skill> GetEnumerator()
    {
        return _skills.GetEnumerator();
    }

    /// <summary>
    /// Default constructor, holds a reference to <paramref name="skills"/>
    /// </summary>
    /// <param name="skills"></param>
    public SkillContainer(IEnumerable<Skill> skills)
    {
        _skills = skills.ToList();
    }

}