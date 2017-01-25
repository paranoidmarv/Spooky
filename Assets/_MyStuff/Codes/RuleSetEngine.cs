using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RuleSetEngine : MonoBehaviour {
    public List<Attribute> primaryAttributes;
    public List<Attribute> physicalAttributes;
    public List<Attribute> ancillaryAttributes;
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
    void Awake () {
        primaryAttributes = new List<Attribute>();
        physicalAttributes = new List<Attribute>();
        ancillaryAttributes = new List<Attribute>();
        isRuleSetLoaded = false;
        LoadRuleSet();
        //=== Turn Variables
        elapsedTime = 0;
        turnsTaken = 0;
        //=== References
        pH = GetComponent<PlayerHandler>();
        sC = GetComponent<SceneManager>();
    }

    public bool EndTurn() {
        foreach(Character character in sC.characters){
            character.UpdateCharacter();
        }
        elapsedTime += timePerTurn;
        turnsTaken++;
        return true;
    }

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
            }
        }
        isRuleSetLoaded = true;
    }

    private void InitializeRuleSetAttributes(Attribute.AttributeType attributeType, string[] lines) {
        List<Attribute> temp = new List<Attribute>();
        for (int i = 1; i < lines.Length; i++) {
            string[] entries = lines[i].Split(';');
            temp.Add(new Attribute(attributeType, entries[0].Split('\n')[1], entries[1], entries));
            if(entries[entries.Length - 1] == "^") {
                temp[temp.Count - 1].active = false;
            }
        }
        if(attributeType == Attribute.AttributeType.Primary) { primaryAttributes = temp; }
        else if(attributeType == Attribute.AttributeType.Physical) { physicalAttributes = temp; }
        else if(attributeType == Attribute.AttributeType.Ancillary) { ancillaryAttributes = temp; }
    }
}
