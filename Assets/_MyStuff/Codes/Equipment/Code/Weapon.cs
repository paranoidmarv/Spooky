using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {
    public enum WeaponType { CQC, Ballistic, NA }
    public WeaponType weaponType;
    public int minDamage;
    public int maxDamage;
    public Tuple<int, int> damageRange;
    public int difficultyClass;
    private CircleCollider2D rangeCollider;
    public float range;
    public List<Character> inRangeTargets;
    public bool canHitDiagonal;
    public string attackSprite; 
    public override void Awake() {
        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.enabled = false;
        inRangeTargets = new List<Character>();
        canHitDiagonal = false;
        damageRange = new Tuple<int, int>(minDamage, maxDamage);
    }
    public override void Update() {
    }
    public void RangeTargets() {
        rangeCollider.enabled = true;
        rangeCollider.radius = range;
    }
    public void ClearTargets() {
        rangeCollider.radius = 0.1f;
        rangeCollider.enabled = false;
        inRangeTargets.Clear();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        //needs raycast to check LOS
        //if collision is enemy or friendly?
        inRangeTargets.Add(collision.gameObject.GetComponent<Character>());
    }
}
