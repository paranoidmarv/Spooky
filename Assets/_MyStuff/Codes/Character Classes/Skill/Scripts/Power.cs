using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : Skill {
    private int currentPowerLevel;
    void Awake() {
        owner = transform.parent.gameObject.GetComponent<Character>();
        currentPowerLevel = 0;
    }
    public int FocusPower(int modPower) {
        if(modPower == 1) {
            if((modPower + 1) <= owner.GetAncillayrAttributeValue(associatedSpecializationID)) { return ++currentPowerLevel; }
            else { return currentPowerLevel; }
        }
        else if(modPower == -1) {
            if((modPower - 1) >= 0) { return --currentPowerLevel; }
            else { return 0; }
        }
        else {
            Debug.Log(modPower);
            return -1;
        }
    }

    public void ResetFocus() {
        currentPowerLevel = 0;
    }
}
