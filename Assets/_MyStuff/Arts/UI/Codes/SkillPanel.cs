using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour {
    public UIButton[] skillButtons;
    public Transform gridTransform;

    void Awake() {
        gridTransform = transform.FindChild("Grid");
    }

    public void InformSkillButton(Skill skill, int button) {
        AddButton(button);
        skillButtons[button].gameObject.GetComponent<NGUIButtonForward>().skillButtonValue = skill.iD;
        skillButtons[button].normalSprite = skill.skillSpriteName;
        skillButtons[button].hoverSprite = skill.skillSpriteName;
        skillButtons[button].pressedSprite = skill.skillSpriteName;
    }

    public void DisableSkillButton(int button) {
        AddButton(button);
        skillButtons[button].gameObject.GetComponent<NGUIButtonForward>().skillButtonValue = -1;
        skillButtons[button].normalSprite = "button_DisabledSkillSlot";
        skillButtons[button].hoverSprite = "button_DisabledSkillSlot";
        skillButtons[button].pressedSprite = "button_DisabledSkillSlot";
        skillButtons[button].disabledSprite = "button_DisabledSkillSlot";
        skillButtons[button].SetState(UIButtonColor.State.Disabled, true);
    }

    public void AddButton(int button) {
        skillButtons[button].gameObject.SetActive(true);
        skillButtons[button].transform.parent = gridTransform;
        gridTransform.GetComponent<UIGrid>().Reposition();
    }

    public void RemoveButton(int button) {
        skillButtons[button].transform.parent = transform;
        skillButtons[button].gameObject.SetActive(false);
        gridTransform.GetComponent<UIGrid>().Reposition();
    }
}
