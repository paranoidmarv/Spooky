using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {
    public int baseDamage;
    public int difficultyClass;
    private CircleCollider2D rangeCollider;
    public float range;
    //====================
    //====Test
    public bool rangeTargets;
    public override void Awake() {
        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.enabled = false;
        //==================
        rangeTargets = false;
    }
    public override void Update() {
        if (rangeTargets) {
            rangeTargets = false;
            RangeTargets();
        }
    }
    public void RangeTargets() {
        rangeCollider.enabled = true;
        rangeCollider.radius = 1f;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision.gameObject);
    }
}
