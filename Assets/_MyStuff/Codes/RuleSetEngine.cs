using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RuleSetEngine : MonoBehaviour {
    public List<Attribute> primaryAttributes;
    public List<Attribute> physicalAttributes;
    public List<Attribute> ancillaryAttributes;
    private double[] twoDTenProbability = new double[] { 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.10, 0.09, 0.08, 0.07, 0.06, 0.05, 0.04, 0.03, 0.02, 0.01 };
    private double[] twoDTenProbabilityAtLeast = new double[] { 1, 0.99, 0.97, 0.94, 0.90, 0.85, 0.79, 0.72, 0.64, 0.55, 0.45, 0.36, 0.28, 0.21, 0.15, 0.1, 0.06, 0.03, 0.01 };
    private double[] twoDTenProbabilityAtMost = new double[] { 0.01, 0.03, 0.06, 0.1, 0.15, 0.21, 0.28, 0.36, 0.45, 0.55, 0.64, 0.72, 0.79, 0.85, 0.9, 0.94, 0.97, 0.99, 1 };

    public List<Profession> professions;
    public List<Specialization> specializations;
    public List<Skill> skill;
    public bool isRuleSetLoaded { get; private set; }
    public TextAsset ruleSet;
    //=== Turn Variables ==========================================
    private int elapsedTime;
    private int turnsTaken;
    public int TurnsTaken { get { return turnsTaken; } }
    public int timePerTurn;
    //=== References ==============================================
    private PlayerHandler pH;
    private SceneManager sC;
    void Awake() {
        primaryAttributes = new List<Attribute>();
        physicalAttributes = new List<Attribute>();
        ancillaryAttributes = new List<Attribute>();
        professions = new List<Profession>();
        specializations = new List<Specialization>();
        isRuleSetLoaded = false;
        LoadRuleSet();
        //=== Turn Variables
        elapsedTime = 0;
        turnsTaken = 0;
        //=== References
        pH = GetComponent<PlayerHandler>();
        sC = GetComponent<SceneManager>();
    }
    //=== Interaction
    public Tuple<double, double> ComputeHitInfo(Character attacker, int attackMod, Character defender, int defendMod) {
        int attack = attacker.attack + attackMod; int defense = defender.defense + defendMod;
        int combatDifference = attack - defense;
        double chanceToHit = 0;
        double chanceToMatch = 0;
        if(combatDifference < 0) {
            combatDifference = Mathf.Abs(combatDifference);
            if(combatDifference >= 20) { return new Tuple<double, double>(0, 0); }
            else {
                int j = 0;
                for(int i = combatDifference + 3; i < twoDTenProbability.Length; i++) {
                    chanceToHit += twoDTenProbabilityAtLeast[i] * twoDTenProbabilityAtMost[j];
                    chanceToMatch += twoDTenProbability[i - 1] * twoDTenProbability[j++];
                }
            }
        }
        else if(combatDifference > 0) {
            if (combatDifference >= 20) { return new Tuple<double, double>(1, 0); }
            else {
                int j = combatDifference + 3;
                double chanceToMiss = 0;
                for (int i = 0; i < twoDTenProbability.Length - j; i++) {
                    chanceToMiss += twoDTenProbabilityAtMost[i] * twoDTenProbabilityAtLeast[j];
                    chanceToMatch += twoDTenProbability[j++ - 1] * twoDTenProbability[i];
                }
                chanceToHit = 1 - chanceToHit;
            }
        }
        else {
            int j = 0;
            for(int i = 0; i < twoDTenProbability.Length - 1; i++) {
                chanceToHit += twoDTenProbabilityAtLeast[i + 1] * twoDTenProbabilityAtMost[j];
                chanceToMatch += twoDTenProbability[i] * twoDTenProbability[j];
            }
        }
        Debug.Log(chanceToHit);
        Debug.Log(chanceToMatch);
        return new Tuple<double, double>(chanceToHit, chanceToMatch);
    }
    public bool EndTurn() {
        foreach (Character character in sC.characters) {
            character.UpdateCharacter();
        }
        elapsedTime += timePerTurn;
        turnsTaken++;
        return true;
    }
    //=== Initialization Methods
    private void LoadRuleSet() {
        string ruleSetString = ruleSet.text;
        string[] sections = ruleSetString.Split('<');
        for (int i = 0; i < sections.Length; i++) {
            if (sections[i].Length > 0) {
                string[] lines = sections[i].Split('~');
                int type;
                int.TryParse(lines[0], out type);
                if (type == 0) { InitializeRuleSetAttributes(Attribute.AttributeType.Primary, lines); }
                else if (type == 1) { InitializeRuleSetAttributes(Attribute.AttributeType.Physical, lines); }
                else if (type == 2) { InitializeRuleSetAttributes(Attribute.AttributeType.Ancillary, lines); }
                else if (type == 3) { InitializeRuleSetProfessions(lines); }
                else if (type == 4) { InitializeRuleSetSpecializations(lines); }
            }
        }
        isRuleSetLoaded = true;
    }

    private void InitializeRuleSetAttributes(Attribute.AttributeType attributeType, string[] lines) {
        List<Attribute> temp = new List<Attribute>();
        for (int i = 1; i < lines.Length; i++) {
            string[] entries = lines[i].Split(';');
            temp.Add(new Attribute(attributeType, entries[0].Split('\n')[1], entries[1], entries));
            if (entries[entries.Length - 1] == "^") {
                temp[temp.Count - 1].active = false;
            }
        }
        if (attributeType == Attribute.AttributeType.Primary) { primaryAttributes = temp; }
        else if (attributeType == Attribute.AttributeType.Physical) { physicalAttributes = temp; }
        else if (attributeType == Attribute.AttributeType.Ancillary) { ancillaryAttributes = temp; }
    }

    private void InitializeRuleSetProfessions(string[] lines) {
        for (int i = 1; i < lines.Length; i++) {
            string[] entries = lines[i].Split(';');
            int id; int.TryParse(entries[3], out id);
            professions.Add(new Profession(entries[0].Split('\n')[1], entries[1], entries[2].Split(':'), id));
        }
    }

    private void InitializeRuleSetSpecializations(string[] lines) {
        for(int i = 1; i < lines.Length; i++) {
            string[] entries = lines[i].Split(';');
            int parProfID, assAttID, iD;
            int.TryParse(entries[2], out parProfID); int.TryParse(entries[3], out assAttID); int.TryParse(entries[4], out iD);
            specializations.Add(new Specialization(entries[0].Split('\n')[1], entries[1], parProfID, iD, new Skill(ancillaryAttributes[assAttID].name, ancillaryAttributes[assAttID].description, i - 1, iD, assAttID)));
        }
    }

    //methods constricting ruleset to ensure only working rulesets are loaded
}
