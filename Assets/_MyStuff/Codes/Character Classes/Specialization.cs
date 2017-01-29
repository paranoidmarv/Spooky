using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specialization {
    public string name;
    public string description;
    public int parentProfessionID;
    public int iD;
    public Skill mainSkill;

    public Specialization(string name, string description, int parentProfID, int iD, Skill mainSkill) {
        this.name = name;
        this.description = description;
        parentProfessionID = parentProfID;
        this.iD = iD;
        this.mainSkill = mainSkill;
    }
}
