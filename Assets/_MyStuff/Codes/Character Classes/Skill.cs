using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {
    public bool focus;
    public string focusValueName;
    public string focusCostName;

    public enum SkillType { Active, Passive }
    public string skillName;
    public string description;
    public SkillType skillType;
    public int iD;
    public int associatedSpecializationID;
    public int associatedAttributeID;
    public int challengedAttributeID;
    public Item.ItemType requiredItemType;
    public Weapon.WeaponType requiredWeaponType;
    //value with which this skill performs (e.g. attack, detect, etc.)
    //value which defends against this skill (e.g. defense, resilience, etc.)
    protected Character owner;
    public string skillSpriteName;

    private void Awake() {
        skillName = gameObject.name;   
    }
}
