using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RuleSetEngine : MonoBehaviour {
    public List<Attribute> primaryAttributes;
    public List<Attribute> physicalAttributes;
    public List<Attribute> ancillaryAttributes;
    public bool isRuleSetLoaded { get; private set; }

    public TextAsset ruleSet;
    // Use this for initialization
    void Awake () {
        primaryAttributes = new List<Attribute>();
        physicalAttributes = new List<Attribute>();
        ancillaryAttributes = new List<Attribute>();
        isRuleSetLoaded = false;
        LoadRuleSet();
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
        /*foreach (Attribute att in primaryAttributes) {
            Debug.Log(att.name + " : " + att.range.x + "-" + att.range.y + " " + att.active);
        }
        foreach (Attribute att in physicalAttributes) {
            Debug.Log(att.name + " : " + att.range.x + "-" + att.range.y + " " + att.active);
        }
        foreach (Attribute att in ancillaryAttributes) {
            Debug.Log(att.name + " : " + att.range.x + "-" + att.range.y + " " + att.active);
        }*/

        
        /*foreach (int i in primaryAttributes[2].inherentEffects) {
            Debug.Log(i);
        }*/
    }

    private void InitializeRuleSetAttributes(Attribute.AttributeType attributeType, string[] lines) {
        List<Attribute> temp = new List<Attribute>();
        for (int i = 1; i < lines.Length; i++) {
            string[] entries = lines[i].Split(';');
            temp.Add(new Attribute(attributeType, entries[0], entries[1], entries));
            if(entries[entries.Length - 1] == "^") {
                temp[temp.Count - 1].active = false;
            }
        }
        if(attributeType == Attribute.AttributeType.Primary) { primaryAttributes = temp; }
        else if(attributeType == Attribute.AttributeType.Physical) { physicalAttributes = temp; }
        else if(attributeType == Attribute.AttributeType.Ancillary) { ancillaryAttributes = temp; }
    }
}
