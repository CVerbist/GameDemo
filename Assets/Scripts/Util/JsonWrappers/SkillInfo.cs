using System;


[Serializable]
public class SkillInfo {
    public string Name;
    public Skill.Statistics Stats;

    public Skill Skill
    {
        get { return new Skill(Name, Stats); }
    }

    public override string ToString() {
        return "Skill " + Name;
    }
}
