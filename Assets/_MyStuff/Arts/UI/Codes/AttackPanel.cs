using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPanel : MonoBehaviour {
    private UISprite attackIcon;
    private UILabel hitChance;
    private UILabel damageRange;
    public GameObject focusPanel;

    void Awake() {
        attackIcon = transform.FindChild("AttackIcon").GetComponent<UISprite>();
        hitChance = transform.FindChild("0").GetComponent<UILabel>();
        damageRange = transform.FindChild("1").GetComponent<UILabel>();
        focusPanel = transform.FindChild("FocusPanel").gameObject;
    }

    public void SetAttackPanel(string attackSprite, double attackProbability, int dmgMin, int dmgMax) {
        attackIcon.spriteName = attackSprite;
        hitChance.text = attackProbability.ToString() + "%";
        damageRange.text = dmgMin.ToString() + "-" + dmgMax.ToString();
    }
}
