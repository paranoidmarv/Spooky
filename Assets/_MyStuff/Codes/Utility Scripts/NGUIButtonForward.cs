using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUIButtonForward : MonoBehaviour {
    public enum ButtonType { ModifyAttribute, PickProfression, FocusAbility, CharacterInfo, CharacterPortrait }
    public ButtonType buttonType;
    private CharacterCreator cC;
    private UIManager uiManager;
    private GameObject professionPanel;
    List<string> button;
    List<string> buttonPressed;
    public int skillButtonValue;
	// Use this for initialization
    void Awake() {
        uiManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        professionPanel = transform.parent.gameObject;
        if(uiManager.characterCreationPanel.activeSelf) {
            cC = uiManager.characterCreationPanel.GetComponent<CharacterCreator>();
            button = new List<string>();
            buttonPressed = new List<string>();
            foreach (Specialization spec in uiManager.sceneManager.ruleSetEngine.specializations) {
                button.Add("button_" + spec.name);
                buttonPressed.Add("button_" + spec.name + "Pressed");
            }
        }
    }
	void OnEnable () {
        if (buttonType == ButtonType.PickProfression) {
            UIButton temp = GetComponent<UIButton>();
            int buttonID; int.TryParse(gameObject.name, out buttonID);
            temp.normalSprite = buttonPressed[buttonID];
            temp.hoverSprite = button[buttonID];
            temp.pressedSprite = button[buttonID];
        }
    }
    void OnClick() {
        if (UICamera.currentTouchID == -1 || buttonType == ButtonType.CharacterInfo) {
            switch (buttonType) {
                case ButtonType.ModifyAttribute:
                    cC.ModifyPhysicalAttribute(gameObject.name);
                    break;
                case ButtonType.PickProfression:
                    int specID;
                    int.TryParse(gameObject.name, out specID);
                    if (cC.newCharacter.professionMono.major != null && (cC.newCharacter.professionMono.minor == null || specID != cC.newCharacter.professionMono.minor.iD)) { ResetButton(cC.newCharacter.professionMono.major.iD); }
                    if (cC.SelectProfession(specID, true)) {
                        UIButton temp = gameObject.GetComponent<UIButton>();
                        temp.normalSprite = button[specID];
                        temp.hoverSprite = buttonPressed[specID];
                        temp.pressedSprite = buttonPressed[specID];
                    }
                    else {
                        UIButton temp = gameObject.GetComponent<UIButton>();
                        temp.normalSprite = buttonPressed[specID];
                        temp.hoverSprite = button[specID];
                        temp.pressedSprite = button[specID];
                    }
                    break;
                case ButtonType.FocusAbility:
                    //uiManager.
                    break;
                case ButtonType.CharacterInfo:
                    uiManager.ToggleMenuPanel(Menu.MenuPanelType.CharacterInfo, string.Empty);
                    break;
                case ButtonType.CharacterPortrait:
                    uiManager.ToggleMenuPanel(Menu.MenuPanelType.CharacterInfo, gameObject.name);
                    break;
            }
        }
        else if (UICamera.currentTouchID == -2) {
            OnRightClick();
        }
    }
    
    void OnRightClick() {
        switch (buttonType) {
            case ButtonType.PickProfression:
                int specID;
                int.TryParse(gameObject.name, out specID);
                if (cC.newCharacter.professionMono.minor != null && (cC.newCharacter.professionMono.major == null || specID != cC.newCharacter.professionMono.major.iD)) { ResetButton(cC.newCharacter.professionMono.minor.iD); }
                if (cC.SelectProfession(specID, false)) {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = button[specID];
                    temp.hoverSprite = buttonPressed[specID];
                    temp.pressedSprite = buttonPressed[specID];
                }
                else {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = buttonPressed[specID];
                    temp.hoverSprite = button[specID];
                    temp.pressedSprite = button[specID];
                }
                break;
        }
    }

    void ResetButton(int buttonID) {
        UIButton temp = professionPanel.transform.FindChild(buttonID.ToString()).GetComponent<UIButton>();
        temp.normalSprite = buttonPressed[buttonID];
        temp.hoverSprite = button[buttonID];
        temp.pressedSprite = button[buttonID];
    }
}
