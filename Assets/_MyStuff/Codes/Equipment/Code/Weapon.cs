using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {
    public int minDamage;
    public int maxDamage;
    public Tuple<int, int> damageRange;
    public int difficultyClass;
    private CircleCollider2D rangeCollider;
    public float range;
    public List<Character> inRangeTargets;
    public bool canHitDiagonal;
    //====================
    //====Test
    public bool rangeTargets;
    public override void Awake() {
        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.enabled = false;
        inRangeTargets = new List<Character>();
        canHitDiagonal = false;
        damageRange = new Tuple<int, int>(minDamage, maxDamage);
        //==================
        rangeTargets = false;
    }
    public override void Update() {
        /*if (rangeTargets) {
            rangeTargets = false;
            RangeTargets();
        }*/
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
        inRangeTargets.Add(collision.gameObject.GetComponent<Character>());
    }
}
