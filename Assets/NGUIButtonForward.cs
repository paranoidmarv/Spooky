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
                    gameObject.GetComponent<UIButton>().normalSprite2D = gameObject.GetComponent<UIButton>().pressedSprite2D;
                }
                else {
                    gameObject.GetComponent<UIButton>().normalSprite2D = gameObject.GetComponent<UIButton>().disabledSprite2D;
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
                    gameObject.GetComponent<UIButton>().normalSprite2D = gameObject.GetComponent<UIButton>().pressedSprite2D;
                }
                else {
                    gameObject.GetComponent<UIButton>().normalSprite2D = gameObject.GetComponent<UIButton>().disabledSprite2D;
                }
                break;
        }
    }
}
