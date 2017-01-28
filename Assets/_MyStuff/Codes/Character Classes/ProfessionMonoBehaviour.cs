using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionMonoBehaviour : MonoBehaviour {
    private RuleSetEngine rSE;
    public List<Tuple<Profession, int>> chosenProfessions;
    public Specialization major;
    public Specialization minor;
    public List<Specialization> basic;
	// Use this for initialization
	void Awake () {
        chosenProfessions = new List<Tuple<Profession, int>>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
//======================================================
//=== Special Abilities
//======================================================
    //=== CQC
    void Power() {

    }
    //=== Ballistics
    //=== Courser
    //=== Detective
    //=== Shaman
    //=== Detective
}
