using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class CharacterCreator : MonoBehaviour {
    public GameObject pAS;
    public GameObject dAP;
    //public GameObject ccButton;
    //public GameObject ccField;
    public UILabel attPointsField;
    private Sprite newCharSprite;
    private string newCharName;
    public GameObject characterPrefab;
    public Character newCharacter;
    private UIManager uiManager;
    private bool creatorInitialized;
    private UILabel[] phAttributeFields;
    private UILabel[] aAttributeFields;
    private List<string> aAttNames;

    public int allottedPoints;
    private int attributePoints;

    void Awake() {
        Debug.Log("awake");
        uiManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        aAttNames = new List<string>();
        creatorInitialized = InitializeCreatorPanel(); 
    }

	// Use this for initialization
	void OnEnable () {
        if (creatorInitialized) {
            attributePoints = allottedPoints;
            newCharacter = ((GameObject)Instantiate(characterPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Character>();
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
        }
    }

    public bool SelectProfession(int specID, bool isLeftClick) {
        if (isLeftClick) {
            if (newCharacter.professionMono.major == null) {
                newCharacter.professionMono.major = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second++;
                return true;
            }
            else if (specID == newCharacter.professionMono.major.iD) {
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.major.parentProfessionID].Second--;
                newCharacter.professionMono.major = null;
                return false;
            }
        }
        else {
            if (newCharacter.professionMono.minor == null) {
                newCharacter.professionMono.minor = uiManager.sceneManager.ruleSetEngine.specializations[specID];
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second++;
                return true;
            }
            else if (specID == newCharacter.professionMono.minor.iD) {
                newCharacter.professionMono.chosenProfessions[newCharacter.professionMono.minor.parentProfessionID].Second--;
                newCharacter.professionMono.minor = null;
                return false;
            }
        }
        return false;
    }

    public void PlaceCharacter() {
        uiManager.sceneManager.sceneMap.placingNewCharacter = true;
        
        uiManager.sceneManager.AddNewCharacter(newCharacter.gameObject);
        newCharacter.gameObject.AddComponent<PlaceNewCharacter>();
        newCharacter = null;
        uiManager.OpenCharacterCreator();
    }
    //=== Initialization ===================================================================================================================================
    private void InitializePanelValues() {
        List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
        foreach(Attribute att in attributes) {
            phAttributeFields[att.ID].text = att.name + "\n" + att.defaultValue.ToString();
        }
        attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
        foreach(Attribute att in attributes) {
            aAttributeFields[att.ID].text = att.name + ": " + att.defaultValue.ToString();
        }
    }

    public bool InitializeCreatorPanel() {
        if (uiManager.sceneManager.sceneReady) {
            List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
            phAttributeFields = new UILabel[attributes.Count];
            for(int i = 0; i < attributes.Count; i++) {
                phAttributeFields[i] = GameObject.Find(attributes[i].name).GetComponent<UILabel>();
                //=== UGUI CODE =================================================================================
                //=== Plus Button ===
                /*GameObject newMinusButton = Instantiate(ccButton);
                newMinusButton.transform.SetParent(pAS.transform);
                newMinusButton.transform.localScale = new Vector3(1, 1, 1);
                newMinusButton.name = "- " + attributes[i].ID;
                newMinusButton.GetComponentInChildren<Text>().text = "-";
                newMinusButton.GetComponent<Button>().onClick.AddListener(ModifyPhysicalAttribute);
                //=== Field ===
                GameObject newField = Instantiate(ccField);
                newField.transform.SetParent(pAS.transform);
                newField.transform.localScale = new Vector3(1, 1, 1);
                phAttributeFields[i] = newField.GetComponent<Text>();
                phAttributeFields[i].alignment = TextAnchor.UpperCenter;
                phAttributeFields[i].text = GetNameAfterReturn(attributes[i].name);
                //=== Minus Button ===
                GameObject newPlusButton = Instantiate(ccButton);
                newPlusButton.transform.SetParent(pAS.transform);
                newPlusButton.transform.localScale = new Vector3(1, 1, 1);
                newPlusButton.name = "+ " + attributes[i].ID;
                newPlusButton.GetComponentInChildren<Text>().text = "+";
                newPlusButton.GetComponent<Button>().onClick.AddListener(ModifyPhysicalAttribute);*/
                //=== UGUI CODE =================================================================================
            }
            attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
            aAttributeFields = new UILabel[attributes.Count];
            for(int i = 0; i < attributes.Count; i++) {
                aAttributeFields[i] = GameObject.Find(attributes[i].name).GetComponent<UILabel>();
                aAttNames.Add(attributes[i].name);
                //=== UGUI CODE =================================================================================
                /*GameObject newField = Instantiate(ccField);
                newField.transform.SetParent(dAP.transform);
                newField.transform.localScale = new Vector3(1, 1, 1);
                aAttNames.Add(GetNameAfterReturn(attributes[i].name));
                aAttributeFields[i] = newField.GetComponent<Text>();
                aAttributeFields[i].text = aAttNames[i] + ": ";*/
                //=== UGUI CODE =================================================================================
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
