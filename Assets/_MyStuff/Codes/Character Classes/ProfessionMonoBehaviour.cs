using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionMonoBehaviour : MonoBehaviour {
    public List<Tuple<Profession, int>> chosenProfessions;
    public Specialization major;
    public Specialization minor;
    public List<Skill> knownSkills;
    public Skill[] equippedSkills;
    private Character owner;
	// Use this for initialization
	void Awake () {
        chosenProfessions = new List<Tuple<Profession, int>>();
        knownSkills = new List<Skill>();
        owner = GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ConfirmSpecializations() {
        knownSkills.Add(Instantiate(owner.professionMono.major.baseSkillPrefab, owner.skillPool.transform).GetComponent<Skill>());
        knownSkills.Add(Instantiate(owner.professionMono.minor.baseSkillPrefab, owner.skillPool.transform).GetComponent<Skill>());
        equippedSkills = new Skill[owner.GetAncillayrAttributeValue("Skill Slots") + 1];
        if(equippedSkills.Length >= 2) {
            if(knownSkills[0].skillType == Skill.SkillType.Active) { equippedSkills[0] = knownSkills[0]; }
            if (knownSkills[1].skillType == Skill.SkillType.Active) { equippedSkills[1] = knownSkills[1]; }
        }
        else {
            if (knownSkills[0].skillType == Skill.SkillType.Active) { equippedSkills[0] = knownSkills[0]; }
        }
    }
    public void EquipSkill(int skillID, int skillSlot) {

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
