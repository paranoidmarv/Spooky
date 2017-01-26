using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int maxEquippableWeapons;
    public List<Weapon> equippedWeapons;
    public Weapon defaultWeapon;
    public float meleeWeaponRange;
	// Use this for initialization
	void Awake () {
        equippedWeapons = new List<Weapon>();
        equippedWeapons.Add(defaultWeapon);
	}

	
	// Update is called once per frame
	void Update () {
		
	}

    public void EquipWeapon(Weapon newWeapon) {
        if(equippedWeapons.Count < maxEquippableWeapons) {
            equippedWeapons.Add(newWeapon);
        }
    }
    public float GetWeaponRange(int weaponIndex) {
        //Instead of returning range
        //have a collider set on weapon object
        //based on range, and is enabled when
        //targetting with weapon 
        Weapon wep = equippedWeapons[weaponIndex];
        if(wep.itemType == Item.ItemType.CQCWeapon) {
            return meleeWeaponRange;
        }
        else if(wep.itemType == Item.ItemType.BallisticWeapon) {
            return ((BallisticWeapon)wep).range;
        }
        else { return -1f; }
    }
}
