using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPanel : MonoBehaviour {
    private UISprite attackIcon;
    private UILabel hitChance;
    private UILabel damageRange;
    public GameObject focusPanel;
    private UILabel focusValueLabel;
    private UILabel focusCostLabel;
    private bool isFocusPanelActive;

    void Awake() {
        attackIcon = transform.FindChild("AttackIcon").GetComponent<UISprite>();
        hitChance = transform.FindChild("0").GetComponent<UILabel>();
        damageRange = transform.FindChild("1").GetComponent<UILabel>();
        focusPanel = transform.FindChild("FocusPanel").gameObject;
        isFocusPanelActive = false;
    }

    void OnEnable() {
        isFocusPanelActive = false;
    }

    void OnDisable() {
        DisableFocusPanel();
    }

    public void EnableFocusPanel(Skill skill) {
        focusPanel.SetActive(true);
        if (focusValueLabel == null) { focusValueLabel = focusPanel.transform.FindChild("FocusValue").GetComponent<UILabel>(); }
        if (focusCostLabel == null) { focusCostLabel = focusPanel.transform.FindChild("FocusCost").GetComponent<UILabel>(); }
        focusValueLabel.text = skill.focusValueName + "\n+0";
        focusCostLabel.text = skill.focusCostName + "\n-0";
    }

    public void SetAttackPanel(string attackSprite, double attackProbability, int dmgMin, int dmgMax) {
        attackIcon.spriteName = attackSprite;
        hitChance.text = attackProbability.ToString() + "%";
        damageRange.text = dmgMin.ToString() + "-" + dmgMax.ToString();
    }

    public void DisableFocusPanel() {
        focusPanel.SetActive(false);
    }
}
