using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoPanel : MonoBehaviour {
    public GameObject physicalAttSelector;
    public GameObject derivedAttPanel;
    public GameObject professionPanel;
    public GameObject characterIDPanel;
    public GameObject skillPanel;

    private UIManager uiManager;
    private List<string> aAttNames;
    private UILabel[] pAttributeFields;
    private UILabel[] phAttributeFields;
    private UILabel[] aAttributeFields;
    private UIButton majorSpecializationSprite;
    private UILabel majorSpecialization;
    private UIButton minorSpecializationSprite;
    private UILabel minorSpecialization;

    private bool characterInfoPanelInitialized;
    // Use this for initialization
    void Awake () {
        physicalAttSelector = transform.FindChild("PA Panel").gameObject;
        derivedAttPanel = transform.FindChild("DA Panel").gameObject;
        professionPanel = transform.FindChild("Profession Panel").gameObject;
        characterIDPanel = transform.FindChild("Character ID Panel").gameObject;
        skillPanel = transform.FindChild("Skill Panel").gameObject;

        uiManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        aAttNames = new List<string>();
        majorSpecializationSprite = professionPanel.transform.FindChild("0").GetComponent<UIButton>();
        majorSpecialization = characterIDPanel.transform.FindChild("Major").GetComponent<UILabel>();
        minorSpecializationSprite = professionPanel.transform.FindChild("1").GetComponent<UIButton>();
        minorSpecialization = characterIDPanel.transform.FindChild("Minor").GetComponent<UILabel>();
        characterInfoPanelInitialized = InitializeCharacterInfoPanel();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeCIPanelValues(Character currentCharacter) {
        if (currentCharacter != null) {
            List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.primaryAttributes;
            pAttributeFields[0].text = "HP: " + currentCharacter.GetPrimaryAttributeValue(attributes[0].ID);
            pAttributeFields[1].text = "AP: " + currentCharacter.GetPrimaryAttributeValue(attributes[1].ID);
            pAttributeFields[2].text = "Grit: " + currentCharacter.GetPrimaryAttributeValue(attributes[2].ID);
            attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
            foreach (Attribute att in attributes) {
                phAttributeFields[att.ID].text = att.name + "\n" + currentCharacter.GetPhysicalAttributeValue(att.ID).ToString();
            }
            attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
            foreach (Attribute att in attributes) {
                aAttributeFields[att.ID].text = att.name + ": " + currentCharacter.GetAncillayrAttributeValue(att.ID).ToString();
            }
            majorSpecialization.text = currentCharacter.professionMono.major.name;
            majorSpecializationSprite.normalSprite = "button_" + majorSpecialization.text;
            majorSpecializationSprite.hoverSprite = "button_" + majorSpecialization.text + "Pressed";
            majorSpecializationSprite.pressedSprite = "button_" + majorSpecialization.text + "Pressed";
            majorSpecializationSprite.disabledSprite = "button_" + majorSpecialization.text;

            minorSpecialization.text = currentCharacter.professionMono.minor.name;
            minorSpecializationSprite.normalSprite = "button_" + minorSpecialization.text;
            minorSpecializationSprite.hoverSprite = "button_" + minorSpecialization.text + "Pressed";
            minorSpecializationSprite.pressedSprite = "button_" + minorSpecialization.text + "Pressed";
            minorSpecializationSprite.disabledSprite = "button_" + minorSpecialization.text;
        }
    }

    public bool InitializeCharacterInfoPanel() {
        if (uiManager.sceneManager.sceneReady) {
            List<Attribute> attributes = uiManager.sceneManager.ruleSetEngine.primaryAttributes;
            pAttributeFields = new UILabel[attributes.Count];
            for (int i = 0; i < attributes.Count; i++) {
                pAttributeFields[i] = characterIDPanel.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
            }
            attributes = uiManager.sceneManager.ruleSetEngine.physicalAttributes;
            phAttributeFields = new UILabel[attributes.Count];
            for (int i = 0; i < attributes.Count; i++) {
                phAttributeFields[i] = physicalAttSelector.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
            }
            attributes = uiManager.sceneManager.ruleSetEngine.ancillaryAttributes;
            aAttributeFields = new UILabel[attributes.Count];
            for (int i = 0; i < attributes.Count; i++) {
                aAttributeFields[i] = derivedAttPanel.transform.FindChild(attributes[i].name).GetComponent<UILabel>();
                aAttNames.Add(attributes[i].name);
            }
            return true;
        }
        else { return false; }
    }
}
