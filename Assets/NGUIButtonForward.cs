using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUIButtonForward : MonoBehaviour {
    public enum ButtonType { ModifyAttribute, PickProfression }
    public ButtonType buttonType;
    private CharacterCreator cC;
	// Use this for initialization
	void Start () {
        cC = GameObject.Find("CC Panel").GetComponent<CharacterCreator>();	
	}
    void OnClick() {
        switch (buttonType) {
            case ButtonType.ModifyAttribute:
                cC.ModifyPhysicalAttribute(gameObject.name);
                break;
            case ButtonType.PickProfression:
                int specID;
                int.TryParse(gameObject.name, out specID);
                if (!cC.SelectProfession(specID, true)) {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = "button_CQC";
                    temp.hoverSprite = "button_CQCPressed";
                    temp.pressedSprite = "button_CQCPressed";
                }
                else {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = "button_CQCPressed";
                    temp.hoverSprite = "button_CQC";
                    temp.pressedSprite = "button_CQC";
                }
                break;
        }
    }
    
    void OnRightClick() {
        switch (buttonType) {
            case ButtonType.PickProfression:
                int specID;
                int.TryParse(gameObject.name, out specID);
                if (!cC.SelectProfession(specID, false)) {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = "button_CQC";
                    temp.hoverSprite = "button_CQCPressed";
                    temp.pressedSprite = "button_CQCPressed";
                }
                else {
                    UIButton temp = gameObject.GetComponent<UIButton>();
                    temp.normalSprite = "button_CQCPressed";
                    temp.hoverSprite = "button_CQC";
                    temp.pressedSprite = "button_CQC";
                }
                break;
        }
    }
}
