using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    public GameObject characterCreationPanel;
    private CharacterCreator cc;
    private GameObject makeNewCharacter;

    public SceneManager sceneManager;

    public GameObject attackPanel;
    private AttackPanel attkP;
    public GameObject characterInfoPanel;
    public GameObject mainPanel;
    public GameObject skillPanel;
    private SkillPanel skllP;
    public GameObject portraitPanel;
    //mainMenuPanel, inventoryPanel
    private List<PortraitInformant> portraitInformants;

	void Awake () {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
        //characterCreationPanel = GameObject.Find("Character Creation Panel");
        sceneManager = GameObject.Find("Player Manager").GetComponent<SceneManager>();
        makeNewCharacter = GameObject.Find("MakeNewCharacter");

        attackPanel = transform.FindChild("Attack Panel").gameObject;
        attkP = attackPanel.GetComponent<AttackPanel>();
        characterInfoPanel = transform.FindChild("Character Panel").gameObject;
        mainPanel = transform.FindChild("Main Panel").gameObject;
        skillPanel = mainPanel.transform.FindChild("SkillPanel").gameObject;
        skllP = skillPanel.GetComponent<SkillPanel>();
        portraitPanel = mainPanel.transform.FindChild("Portrait Panel").gameObject;

        portraitInformants = new List<PortraitInformant>();
        GetInformantReferences();
    }

    public void PauseGame() {
        Time.timeScale = 0;
    }

    public void UnPauseGame() {
        Time.timeScale = 1;
    }
    //=== Key Listener ==================================================================================
    void OnKey(KeyCode key) {
        Debug.Log(key);
    }
    //=== Toggle Panels =================================================================================
    public void OpenCharacterCreator() {
        if (Time.timeScale == 1) {
            if(cc == null) { cc = cc = characterCreationPanel.GetComponent<CharacterCreator>(); }
            PauseGame();
            characterCreationPanel.SetActive(true);
            makeNewCharacter.GetComponentInChildren<UILabel>().text = "Cancel";
            sceneManager.playerHandler.AcceptingInput = false;
            sceneManager.playerHandler.ClearSelection();
        }
        else {
            UnPauseGame();
            characterCreationPanel.SetActive(false);
            makeNewCharacter.GetComponentInChildren<UILabel>().text = "Create New Character";
        }     
    }

    public void ToggleMenuPanel(Menu.MenuPanelType menuPanelType, string args) {
        switch (menuPanelType) {
            case Menu.MenuPanelType.CharacterInfo:
                if (!characterInfoPanel.activeSelf) {
                    int selectedParty = 0;
                    if (!string.IsNullOrEmpty(args)) { int.TryParse(args, out selectedParty); }
                    PauseGame();
                    sceneManager.playerHandler.AcceptingInput = false;
                    characterInfoPanel.SetActive(true);
                    characterInfoPanel.GetComponent<CharacterInfoPanel>().InitializeCIPanelValues(sceneManager.playerHandler.friendlyParty[selectedParty]);
                }
                else {
                    UnPauseGame();
                    characterInfoPanel.SetActive(false);
                    sceneManager.playerHandler.AcceptingInput = true;
                }
                break;
        }
        //inventory, characterInfo, mainMenu
    }

    public bool isSkillPanelToggled = false;
    public void ToggleSkillPanel() {
        if (!isSkillPanelToggled) {
            skillPanel.SetActive(true);
            isSkillPanelToggled = true;
        }
        else {
            skillPanel.SetActive(false);
            isSkillPanelToggled = false;
        }
    }

    public bool isAttackPanelToggled = false;
    public void ToggleAttackPanel() {
        if (!isAttackPanelToggled) {
            Debug.Log("toggle");
            attackPanel.SetActive(true);
            isAttackPanelToggled = true;
        }
        else {
            attackPanel.SetActive(false);
            isAttackPanelToggled = false;
        }
    }
    public void SwitchAttackPanel() {
        Debug.Log("switch");
    }
    //=== Inform Panels =================================================================================
    public void EngageInformant(Character character) {
        portraitInformants[sceneManager.playerHandler.friendlyParty.IndexOf(character)].SetSubject(character);
    }
    public static void InformPortraitValues(Character character) {
        instance.portraitInformants[instance.sceneManager.playerHandler.friendlyParty.IndexOf(character)].SetValues();
    }
    public static void InformPortraitValues() {
        foreach(PortraitInformant pI in instance.portraitInformants) {
            pI.SetValues();
        }
    }
    public void InformAttackPanel(Character character, Skill skill, Character target) {
        if(skill.focus) { attkP.EnableFocusPanel(skill); }
        attkP.SetAttackPanel(skill.skillSpriteName, 0.1, 1, 2);
    }
    public void InformSkillPanel(Character character) {
        for(int i = 0; i < skllP.skillButtons.Length - 1; i++) {
            if (i < character.professionMono.equippedSkills.Length) {
                if (character.professionMono.equippedSkills[i] != null) { skllP.InformSkillButton(character.professionMono.equippedSkills[i], i); }
                else { skllP.DisableSkillButton(i); }
            }
            else {
                skllP.RemoveButton(i);
            }
        }
        skllP.InformSkillButton(character.professionMono.useItemSKill, 8);
    }
    private void GetInformantReferences() {
        foreach(Transform child in portraitPanel.transform) {
            portraitInformants.Add(child.GetComponent<PortraitInformant>());
        }
    }
}
