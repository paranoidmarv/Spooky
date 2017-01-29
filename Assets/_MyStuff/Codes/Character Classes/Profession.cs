using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profession {
    public string name;
    public string description;
    public int iD;
    public int[] childSpecializationIDs;

    public Profession(string name, string description, string[] childSpecIDs, int iD) {
        this.name = name;
        this.description = description;
        this.iD = iD;
        childSpecializationIDs = new int[childSpecIDs.Length];
        for(int i = 0; i < childSpecIDs.Length; i++) {
            int.TryParse(childSpecIDs[i], out childSpecializationIDs[i]);
        }
    }
}
