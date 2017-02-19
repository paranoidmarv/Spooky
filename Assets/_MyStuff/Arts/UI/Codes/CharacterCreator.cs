using UnityEngine;
using System.Collections.Generic;

public class CharacterCreator : MonoBehaviour {
    public GameObject physicalAttSelector;
    public GameObject derivedAttPanel;
    public GameObject professionPanel;
    public GameObject characterIDPanel;
    //public GameObject ccButton;
    //public GameObject ccField;
    public UILabel attPointsField;
    private Sprite newCharSprite;
    private string newCharName;
    public GameObject characterPrefab;
    public Character newCharacter;
    private UIManager uiManager;
    private bool creatorInitialized;
    private UILabel[] pAttributeFields;
    private UILabel[] phAttributeFields;
    private UILabel[] aAttributeFields;
    private List<string> aAttNames;
    private UILabel majorSpecialization;
    private UILabel minorSpecialization;

    public int allottedPoints;
    private int attributePoints;

    void Awake() {
        physicalAttSelector = transform.FindChild("PA Panel").gameObject;
        derivedAttPanel = transform.FindChild("DA Panel").gameObject;
        professionPanel = transform.FindChild("Profession Panel").gameObject;
        characterIDPanel = transform.FindChild("Character ID Panel").gameObject;

        uiManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        aAttNames = new List<string>();
        majorSpecialization = characterIDPanel.transform.FindChild("Major").GetComponent<UILabel>();
        minorSpecialization = characterIDPanel.transform.FindChild("Minor").GetComponent<UILabel>();
        creatorInitialized = InitializeCreatorPanel(); 
    }

	// Use this for initialization
	void OnEnable () {
        if (creatorInitialized) {
            attributePoints = allottedPoints;
            newCharacter = (Instantiate(characterPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Character>();
            uiManager.sceneManager.InitializeNewCharacter(newCharacter.gameObject);
            InitializePanelValues();
            attPointsField.text = "Attribute Points: " + attributePoints.ToString();
        }
        else { uiManager.OpenCharacterCreator(); }
    }

    void OnDisable() {
        if (newCharacter != null) {
            newCharacter.DestroyThis();
        }
    }
	
    public void ModifyPhysicalAttribute(string bN) {  
        string[] buttonName = bN.Split(' ');
        if (buttonName.Length == 2) {
            int id;
            int.TryParse(buttonName[1], out id);
            if (buttonName[0] == "-") {
                if (newCharacter.GetPhysicalAttributeValue(id) < 31) {
                    if (newCharacter.GetPhysicalAttributeValue(id) - 1 >= 0) {
                        newCharacter.ModifyAttribute(id, Attribute.AttributeType.Physical, -1);
                        attributePoints++;
                    }
                }
                else {
                    newCharacter.ModifyAttribute(id, Attribute.AttributeType.Physical, -1);
                    attributePoints += 2;
                }
            }
            else if(buttonName[0] == "+") {
                if (newCharacter.GetPhysicalAttributeValue(id) < 30) {
                    if (newCharacter.GetPhysicalAttributeValue(id) + 1 <= uiManager.sceneManager.ruleSetEngine.physicalAttributes[id].attributeRange.Second && attributePoints - 1 >= 0) {
                        newCharacter.ModifyAttribute(id, Attribute.AttributeType.Physical, 1);
                        attributePoints--;
                    }
                }
                else {
                    if (newCharacter.GetPhysicalAttributeValue(id) + 1 <= uiManager.sceneManager.ruleSetEngine.physicalAttributes[id].attributeRange.Second && attributePoints - 2 >= 0) {
                        newCharacter.ModifyAttribute(id, Attribute.AttributeType.Physical, 1);
                        attributePoints -= 2;
                    }
                }
            }
            string[] attName = phAttributeFields[id].text.Split('\n');
            phAttributeFields[id].text = attName[0] + "\n" + newCharacter.GetPhysicalAttributeValue(id);
            int[] derivedAttIDs = newCharacter.GetDerivedAttributeIDs(id);
            foreach (int derivedAttID in derivedAttIDs) {
                //string[] field = aAttributeFields[derivedAttID].text.Split(' ');
                aAttributeFields[derivedAttID].text = aAttNames[derivedAttID] + ": " + newCharacter.GetAncillayrAttributeValue(derivedAttID);
                //aAttributeFields[derivedAttID].text 
            }
            newCharacter.ResetPrimaryAttributes();
            attPointsField.text = "Attribute Points: " + attributePoints.ToString();
            int[] primaryAttIDs = newCharacter.GetPrimaryAttributeIDs(id);
            foreach (int primEffID in primaryAttIDs) {
                string[] temp = pAttributeFields[primEffID].text.Split(':');
                pAttributeFields[primEffID].text = temp[0] + ": " + newCharacter.GetPrimaryAttributeValue(primEffID);
            }
        }
    }
    public bool SelectProfession(int specID, bool isLeftClick) {
        if (isLeftClick) {
            if (newCharacter.professionMono.major == null) {
                if (newCharacter.professionMono.minor != null && specID == newCharacter.professionMono.minor.iD) {
                    newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second--;
                    newCharacter.professionMono.minor = null;
                    minorSpecialization.text = "";
                }
                newCharacter.professionMono.major = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second++;
                majorSpecialization.text = newCharacter.professionMono.major.name;
                return true;
            }
            else if (specID == newCharacter.professionMono.major.iD) {
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second--;
                newCharacter.professionMono.major = null;
                majorSpecialization.text = "";
                return false;
            }
            else if (newCharacter.professionMono.minor != null && specID == newCharacter.professionMono.minor.iD) {
                Specialization temp = newCharacter.professionMono.major;
                newCharacter.professionMono.major = newCharacter.professionMono.minor;
                newCharacter.professionMono.minor = temp;
                majorSpecialization.text = newCharacter.professionMono.major.name;
                minorSpecialization.text = newCharacter.professionMono.minor.name;
                return true;
            }
            else {
                newCharacter.professionMono.major = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second++;
                majorSpecialization.text = newCharacter.professionMono.major.name;
                return true;
            }
        }
        else {
            if (newCharacter.professionMono.minor == null) {
                if(newCharacter.professionMono.major != null && specID == newCharacter.professionMono.major.iD) {
                    newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second--;
                    newCharacter.professionMono.major = null;
                    majorSpecialization.text = "";
                }
                newCharacter.professionMono.minor = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second++;
                minorSpecialization.text = newCharacter.professionMono.minor.name;
                return true;
            }
            else if (specID == newCharacter.professionMono.minor.iD) {
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second--;
                newCharacter.professionMono.minor = null;
                minorSpecialization.text = "";
                return false;
            }
            else if (newCharacter.professionMono.major != null && specID == newCharacter.professionMono.major.iD) {
                Specialization temp = newCharacter.professionMono.major;
                newCharacter.professionMono.major = newCharacter.professionMono.minor;
                newCharacter.professionMono.minor = temp;
                majorSpecialization.text = newCharacter.professionMono.major.name;
                minorSpecialization.text = newCharacter.professionMono.minor.name;
                return true;
            }
            else {
                newCharacter.professionMono.minor = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second++;
                minorSpecialization.text = newCharacter.professionMono.minor.name;
                return true;
            }
        }
    }

    public void PlaceCharacter() {
        newCharacter.professionMono.ConfirmSpecializations();
        uiManager.sceneManager.sceneMap.placingNewCharacter = true;
        uiManager.sceneManager.AddNewCharacter(newCharacter.gameObject);
        newCharacter.gameObject.AddComponent<PlaceNewCharacter>();
        newCharacter = null;
        uiManager.OpenCharacterCreator();
    }
    public void ToggleEnemy() {
        if(newCharacter.tag == "Character") {
            newCharacter.tag = "Enemy";
            newCharacter.type = Character.CharacterType.Enemy;
            newCharacter.isPlayerControlled = false;
        }
        else if (newCharacter.tag == "Enemy") {
            newCharacter.tag = "Character";
            newCharacter.type = Character.CharacterType.Party;
            newCharacter.isPlayerControlled = true;
        }
        //Third and fourth statements for friendly and neutral for debug, etc.
    }
    //=== Initialization ===================================================================================================================================
    private void InitializePanelValues() {
        List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.primaryAttributes;
        pAttributeFields[0].text = "HP: " + attributes[0].defaultValue;
        pAttributeFields[1].text = "AP: " + attributes[1].defaultValue;
        pAttributeFields[2].text = "Grit: " + attributes[2].defaultValue;
        /*foreach (Attribute att in attributes) {
            pAttributeFields[att.ID].text = att.name + ": " + att.defaultValue.ToString();
        }*/
        attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
        foreach(Attribute att in attributes) {
            phAttributeFields[att.ID].text = att.name + "\n" + att.defaultValue.ToString();
        }
        attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
        foreach(Attribute att in attributes) {
            aAttributeFields[att.ID].text = att.name + ": " + att.defaultValue.ToString();
        }
        majorSpecialization.text = "";
        minorSpecialization.text = "";
    }

    public bool InitializeCreatorPanel() {
        if (uiManager.sceneManager.sceneReady) {
            List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.primaryAttributes;
            pAttributeFields = new UILabel[attributes.Count];
            for(int i = 0; i < attributes.Count; i++) {
                pAttributeFields[i] = characterIDPanel.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
            }
            attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
            phAttributeFields = new UILabel[attributes.Count];
            for(int i = 0; i < attributes.Count; i++) {
                phAttributeFields[i] = physicalAttSelector.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
            }
            attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
            aAttributeFields = new UILabel[attributes.Count];
            for(int i = 0; i < attributes.Count; i++) {
                aAttributeFields[i] = derivedAttPanel.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
                aAttNames.Add(attributes[i].name);
            }
            return true;
        }
        else { return false; }
    }
    //=== Utility/Workaround ===================================================================================================================================
    private string GetNameAfterReturn(string attName) {
        string[] temp = attName.Split('\n');
        return temp[1];
    }
}
