using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceNewCharacter : MonoBehaviour {
    public Character newCharacter;
    Map sceneMap;
	// Use this for initialization
	void Awake () {
        newCharacter = gameObject.GetComponent<Character>();
        newCharacter.gameObject.layer = 2;
        newCharacter.PlayerHandler.PlacingNewCharacter(newCharacter);
        sceneMap = GameObject.Find("Map").GetComponent<Map>();
        sceneMap.newCharacter = newCharacter.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneMap.highLightedCell != null && !sceneMap.switchingHighLight) {
            transform.position = new Vector3(sceneMap.highLightedCell.Position.First, sceneMap.highLightedCell.Position.Second);
        }
        else {
            if (sceneMap.highLightedCell != null) {
                sceneMap.ClearHighLight(sceneMap.highLightedCell.Position);
            }
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    public void PlaceCharacter() {
        newCharacter.SetCell(sceneMap.highLightedCell);
        newCharacter.gameObject.layer = 0;
        sceneMap.ClearHighLight(sceneMap.highLightedCell.Position);
        sceneMap.placingNewCharacter = false;
        sceneMap.newCharacter = null;
        newCharacter.PlayerHandler.FinishedPlacingNewCharacter();
        Destroy(this);
    }
}
