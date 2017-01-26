using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public string name;
    public int iD;
    public enum ItemType { CQCWeapon, BallisticWeapon, Projectile, Garment };
    public ItemType itemType;
    // Use this for initialization
    public virtual void Awake () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}
}
