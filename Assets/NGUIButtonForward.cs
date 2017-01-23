using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUIButtonForward : MonoBehaviour {
    private CharacterCreator cC;
	// Use this for initialization
	void Start () {
        cC = GameObject.Find("CC Panel").GetComponent<CharacterCreator>();	
	}
    void OnClick() {
        cC.ModifyPhysicalAttribute(gameObject.name);
    }
}
