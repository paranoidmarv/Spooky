using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public Character owner;
    public List<Item> inventory;
    public int maxEquippableWeapons;
    public List<Weapon> equippedWeapons;
    public Weapon defaultWeapon;
    public Weapon currentWeapon;
	// Use this for initialization
	void Awake () {
        owner = transform.parent.gameObject.GetComponent<Character>();
        equippedWeapons = new List<Weapon>();
        defaultWeapon = transform.FindChild("CQC_HandToHand").GetComponent<Weapon>();
        EquipWeapon(defaultWeapon);
        currentWeapon = equippedWeapons[0];
	}

	void Update () {
		
	}

    public void EquipWeapon(Weapon newWeapon) {
        if(equippedWeapons.Count < maxEquippableWeapons) {
            equippedWeapons.Add(newWeapon);
        }
    }
    public IEnumerator GetWeaponRange() {
        //Instead of returning range
        //have a collider set on weapon object
        //based on range, and is enabled when
        //targetting with weapon
        currentWeapon.RangeTargets();
        yield return new WaitForFixedUpdate();
        owner.PlayerHandler.EngageTargetEnemyContext(currentWeapon.inRangeTargets);
        currentWeapon.ClearTargets();
    }
}
