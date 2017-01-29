using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
    public string name;
    public string description;
    public int iD;
    public int associatedSpecializationID;
    public int associatedAttributeID; 

    public Skill(string name, string description, int iD, int assSpecID, int assAttID) {
        this.name = name;
        this.description = description;
        this.iD = iD;
        associatedSpecializationID = assSpecID;
        associatedAttributeID = assAttID;
    }
}
