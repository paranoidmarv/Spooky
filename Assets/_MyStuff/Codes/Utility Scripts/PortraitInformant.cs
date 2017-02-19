using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitInformant : MonoBehaviour {
    private Character subject;
    private UILabel hpLabel;
    private UILabel apLabel;
    private UILabel gritLabel;
    
    void Awake() {
        Transform labelParent = transform.FindChild("LabelParent");
        hpLabel = labelParent.FindChild("0").GetComponent<UILabel>();
        apLabel = labelParent.FindChild("1").GetComponent<UILabel>();
        gritLabel = labelParent.FindChild("2").GetComponent<UILabel>();
    }

    public void SetSubject(Character character) {
        subject = character;
        SetValues();
    }

    public void SetValues() {
        hpLabel.text = subject.currentHealth + "/" + subject.GetPrimaryAttributeValue(0);
        apLabel.text = subject.currentActionPoints + "/" + subject.GetPrimaryAttributeValue(1);
        gritLabel.text = subject.GetPrimaryAttributeValue(2).ToString();
    }
}
