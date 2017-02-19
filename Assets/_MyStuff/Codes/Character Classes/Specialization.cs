using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specialization {
    public string name;
    public string description;
    public int parentProfessionID;
    public int iD;
    public GameObject baseSkillPrefab;
    public Skill baseSkill;

    public Specialization(string name, string description, int parentProfID, int iD, GameObject baseSkillPrefab) {
        this.name = name;
        this.description = description;
        parentProfessionID = parentProfID;
        this.iD = iD;
        this.baseSkillPrefab = baseSkillPrefab;
        baseSkill = baseSkillPrefab.GetComponent<Skill>();
    }
}
