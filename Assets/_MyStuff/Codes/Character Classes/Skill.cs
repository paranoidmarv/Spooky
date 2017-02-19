using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {
    public enum SkillType { Active, Passive }
    public string name;
    public string description;
    public SkillType skillType;
    public int iD;
    public int associatedSpecializationID;
    public int associatedAttributeID;
    protected Character owner;
    public bool focus;
}
