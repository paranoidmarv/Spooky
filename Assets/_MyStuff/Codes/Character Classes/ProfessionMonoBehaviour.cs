using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionMonoBehaviour : MonoBehaviour {
    private RuleSetEngine rSE;
    public Specialization major;
    public Specialization minor;
    public Specialization[] basic;
	// Use this for initialization
	void Awake () {
        rSE = GetComponent<RuleSetEngine>();
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
