using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionMonoBehaviour : MonoBehaviour {
    public List<Tuple<Profession, int>> chosenProfessions;
    public Specialization major;
    public Specialization minor;
    public List<Skill> knownSkills;
    public Skill[] equippedSkills;
    public Skill useItemSKill;
    private Character owner;

	void Awake () {
        chosenProfessions = new List<Tuple<Profession, int>>();
        knownSkills = new List<Skill>();
        owner = GetComponent<Character>();
        useItemSKill = owner.skillPool.transform.Find("UseItem").GetComponent<Skill>();
    }

    public void ConfirmSpecializations() {
        knownSkills.Add(Instantiate(owner.professionMono.major.baseSkillPrefab, owner.skillPool.transform).GetComponent<Skill>());
        knownSkills.Add(Instantiate(owner.professionMono.minor.baseSkillPrefab, owner.skillPool.transform).GetComponent<Skill>());
        equippedSkills = new Skill[owner.GetAncillayrAttributeValue("Skill Slots")];
        if(equippedSkills.Length >= 2) {
            if(knownSkills[0].skillType == Skill.SkillType.Active) { equippedSkills[0] = knownSkills[0]; }
            if (knownSkills[1].skillType == Skill.SkillType.Active) { equippedSkills[1] = knownSkills[1]; }
        }
        else if (equippedSkills.Length >= 1) {
            if (knownSkills[0].skillType == Skill.SkillType.Active) { equippedSkills[0] = knownSkills[0]; }
        }
    }

    public void EquipSkill(int skillSlot) {

    }
}
