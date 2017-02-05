using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	void Update () {
        //if (Input.anyKey) {
            Vector2 moveVector = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.W)) { moveVector += new Vector2(0, 1f); }
            if (Input.GetKey(KeyCode.S)) { moveVector += new Vector2(0, -1f); }
            if (Input.GetKey(KeyCode.A)) { moveVector += new Vector2(-1f, 0); }
            if (Input.GetKey(KeyCode.D)) { moveVector += new Vector2(1f, 0); }
            if (moveVector != Vector2.zero) { PlayerHandler.Event(moveVector); }
        }
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
            if (Input.GetKey(KeyCode.Escape)) {

            }
            else if (Input.GetKey(KeyCode.Alpha1)) { PlayerHandler.Event(1); }
            else if (Input.GetKey(KeyCode.Alpha2)) { PlayerHandler.Event(2); }
            else if (Input.GetKey(KeyCode.Alpha3)) { PlayerHandler.Event(3); }
            else if (Input.GetKey(KeyCode.Alpha4)) { PlayerHandler.Event(4); }
            else if (Input.GetKey(KeyCode.Alpha5)) { PlayerHandler.Event(5); }
            else if (Input.GetKey(KeyCode.Alpha6)) { PlayerHandler.Event(6); }
            else if (Input.GetKey(KeyCode.Alpha7)) { PlayerHandler.Event(7); }
            else if (Input.GetKey(KeyCode.Alpha8)) { PlayerHandler.Event(8); }
            else if (Input.GetKey(KeyCode.Space)) { PlayerHandler.ConfirmAction(); }
            else if (Input.GetKeyUp(KeyCode.Return)) { PlayerHandler.EndTurn(); }
        //}
    }

    GameObject GetClickedGameObject() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null) {
            return hit.transform.gameObject;
        }
        else return null;
    }
}
