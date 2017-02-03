using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 moveVector = Vector2.zero;
        int mouseButton = -1;
        //Mouse Click Event
        if (Input.GetMouseButtonUp(0)) {
            mouseButton = 0;
            //PlayerHandler.instance.MouseClickEvent(0, GetClickedGameObject());
        }
        else if (Input.GetMouseButtonUp(1)) {
            mouseButton = 1;
            //PlayerHandler.instance.MouseClickEvent(1, GetClickedGameObject());
        }
        if (mouseButton != -1) {
            GameObject clickedGO = GetClickedGameObject();
            if (clickedGO != null) { PlayerHandler.Event(mouseButton, clickedGO); }
        }
        //Button Event
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.W)) { moveVector += new Vector2(0, 1f); }
            if (Input.GetKey(KeyCode.S)) { moveVector += new Vector2(0, -1f); }
            if (Input.GetKey(KeyCode.A)) { moveVector += new Vector2(-1f, 0); }
            if (Input.GetKey(KeyCode.D)) { moveVector += new Vector2(1f, 0); }
            if (moveVector != Vector2.zero) { PlayerHandler.Event(moveVector); }
        }
        else if (Input.GetKeyUp(KeyCode.Return)) { PlayerHandler.EndTurn(); }
    }

    GameObject GetClickedGameObject() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null) {
            return hit.transform.gameObject;
        }
        else return null;
    }
}
