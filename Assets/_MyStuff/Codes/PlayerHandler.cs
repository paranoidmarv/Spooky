using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour {
    /*
    Player Handler takes player input, parses is, and tells affected entities (character/enemy/item)
    how to feel about that input. It is also (temporarily?) responsible for displaying context info
    to player.
    */
    public static PlayerHandler instance = null;
    public enum PlayerState { NothingSelected, InMenu, CharacterSelected, EnemySelected };
    //InWorld = navigating grid overworld
    //InMenu = events are being driven by GUI
    //SomethingSelected = currentSelection is not null
    //Character/Enemy/Item Selected
    public PlayerState currentState; //shouldn't be public
    public GameObject currentSelection; //shouldn't be public
    public Character currentSelectedCharacter;
    private bool acceptingInput;
    public bool AcceptingInput {
        set { acceptingInput = value; }
    }
    private Map map;

    private GameObject mainCamera;
    private int cameraSpeed = 5;

    void Awake() {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
        currentState = PlayerState.NothingSelected;
        GetMapReference();
        //currentSelection = null;
        map = GameObject.Find("Map").GetComponent<Map>();
        mainCamera = GameObject.Find("Main Camera");

        acceptingInput = true;
	}
    //--- Input Manager
    public static void Event(int mouseButton, GameObject clickedObject) {
        if (instance.acceptingInput && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            instance.acceptingInput = false;
            switch (instance.currentState) {
                case PlayerState.NothingSelected:
                    if (mouseButton == 0) {
                        //if (clickedObject.tag == "Tile") { /*Select character/enemy on tile?*/ }
                        if (clickedObject.tag == "Character") {
                            instance.currentState = PlayerState.CharacterSelected;
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = instance.currentSelection.GetComponent<Character>();
                            instance.EngageMoveContext();
                        }
                        else if (clickedObject.tag == "Enemy") {
                            instance.currentState = PlayerState.EnemySelected;
                            instance.SelectObject(clickedObject);
                            instance.ShowSelectionInfo();
                        }
                        else { instance.ClearSelection(); }
                    }
                    else if (mouseButton == 1) {
                        if (clickedObject.tag == "Character" || clickedObject.tag == "Enemy") {
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = clickedObject.GetComponent<Character>();
                            instance.ShowSelectionInfo();
                        }
                        else { instance.ClearSelection(); }
                    }
                    break;
                case PlayerState.CharacterSelected:
                    if (mouseButton == 0) {
                        if (clickedObject.tag == "Character" && clickedObject.GetInstanceID() == instance.currentSelection.GetInstanceID()) {
                            Debug.Log("yo");
                            break;
                        }
                        else if (clickedObject.tag == "Character" || clickedObject.tag == "Enemy") {
                            Debug.Log("no");

                            instance.DisengageMoveContext();
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = clickedObject.GetComponent<Character>();
                            instance.EngageMoveContext();
                        }
                        else {
                            instance.DisengageMoveContext();
                            instance.ClearSelection();
                        }
                    }
                    else if (mouseButton == 1) {
                        if (clickedObject.tag == "Tile" && clickedObject.GetComponent<Cell>().IsTraversable && !clickedObject.GetComponent<Cell>().isOccupied
                            && instance.currentSelectedCharacter.moving == false && instance.currentSelectedCharacter.GetMovePaths().Contains(clickedObject.GetComponent<Cell>())) {
                            instance.DisengageMoveContext();
                            instance.MoveTo(instance.currentSelection, clickedObject);
                        }
                        //if(clickedObject.tag == "Enemy") {}
                        //if(clickedObject.tag == "Character") {}
                    }
                    break;
            }
            instance.acceptingInput = true;
        }
    }
    public static void Event(Vector2 moveVector) {
        //in menu

        //in game
        instance.MoveCamera(new Vector3(moveVector.x, moveVector.y, 0f));
    }
    //
    private void SelectObject(GameObject clickedObject) {
        currentSelection = clickedObject;
        instance.currentState = PlayerState.CharacterSelected;
    }
    private void ClearSelection() {
        currentSelection = null;
        currentSelectedCharacter = null;
        instance.currentState = PlayerState.NothingSelected;
    }
    private void MoveCamera(Vector3 moveVector) {
        mainCamera.transform.Translate(moveVector * cameraSpeed * Time.deltaTime);
    }
    private void MoveTo(GameObject origin, GameObject destination) {
        List<Tuple<int, int>> pathList = AStar.PathFind(origin.GetComponent<Character>().CurrentCell.Position, destination.GetComponent<Cell>().Position);
        List<Vector3> positionList = new List<Vector3>();
        int moveCost = 0;
        for(int i = pathList.Count - 1; i >= 0; i--){
            positionList.Add(new Vector3(pathList[i].First, pathList[i].Second, -1f));
            moveCost += map.GetCellWeight(pathList[i]);
        }
        currentSelectedCharacter.Move(positionList, destination.GetComponent<Cell>(), moveCost * -1);
    }

    //--- Utility
    private void GetMapReference() {
        map = GameObject.Find("Map").GetComponent<Map>();
        ClearSelection();
    }
    public void PlacingNewCharacter(Character newCharacter) {
        instance.AcceptingInput = false;
        /*
        instance.currentSelection = newCharacter.gameObject;
        instance.currentSelectedCharacter = newCharacter;
        instance.currentState = PlayerState.NothingSelected;*/
    }
    public void FinishedPlacingNewCharacter() {
        //instance.currentSelection.GetComponent<PlaceNewCharacter>().PlaceCharacter();
        ClearSelection();
        StartCoroutine(EnableInput());
    }
    private IEnumerator EnableInput() {
        yield return new WaitForSeconds(0.5f);
        instance.acceptingInput = true;
    }
    //--- Context Methods
    public void EngageMoveContext() {
        if (currentSelectedCharacter != null) {
            List<Cell> cellsToHighLight;
            if (currentSelectedCharacter.GetMovePaths().Count == 0) {
                cellsToHighLight = map.HighlightAllPathsUnderWeight(currentSelectedCharacter.CurrentCell.Position, currentSelectedCharacter.currentActionPoints);
                currentSelectedCharacter.SetMovePaths(cellsToHighLight);
            }
            else {
                cellsToHighLight = currentSelectedCharacter.GetMovePaths();
            }
            foreach (Cell cell in cellsToHighLight) {
                map.HighLightCell(cell.Position);
            }
        }
        //ShowSelectionInfo();
    }

    public void DisengageMoveContext() {
        if (currentSelectedCharacter != null) {
            List<Cell> movPaths = currentSelectedCharacter.GetMovePaths();
            if (movPaths.Count > 0) {
                foreach (Cell moveCell in movPaths) {
                    moveCell.ClearHighLight();
                }
            }
        }
    }

    void ShowSelectionInfo() {
        if(currentState == PlayerState.CharacterSelected) {
            currentSelectedCharacter.PrintAttributes();
        }
    }
}
