using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour {
    /*
    Player Handler takes player input, parses it, and tells affected entities (character/enemy/item)
    how to feel about that input. It is also (temporarily?) responsible for displaying context info
    to player.
    */
    public static PlayerHandler instance = null;
    public enum PlayerState { NothingSelected, InMenu, CharacterSelected, TargetingEnemies, EnemySelected, EnemyTargeted };
    //InWorld = navigating grid overworld
    //InMenu = events are being driven by GUI
    //SomethingSelected = currentSelection is not null
    //Character/Enemy/Item Selected
    public PlayerState currentState; //shouldn't be public
    public GameObject currentSelection; //shouldn't be public
    public Character currentSelectedCharacter;
    public Character currentTarget;
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
    //==================================================================================================================
    //=== Input Manager
    //==================================================================================================================
    public static void Event(int mouseButton, GameObject clickedObject) {
        if (instance.acceptingInput && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            instance.acceptingInput = false;
            switch (instance.currentState) {
                case PlayerState.NothingSelected:
                    Debug.Log("n/a");
                    if (mouseButton == 0) {
                        Debug.Log("n/a|m0");
                        //if (clickedObject.tag == "Tile") { /*Select character/enemy on tile?*/ }
                        if (clickedObject.tag == "Character") {
                            Debug.Log("n/a|m0|char");
                            instance.currentState = PlayerState.CharacterSelected;
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = instance.currentSelection.GetComponent<Character>();
                            instance.EngageMoveContext();
                            instance.acceptingInput = true;
                        }
                        /*else if (clickedObject.tag == "Enemy") {
                            Debug.Log("n/a|m0|nme");
                            instance.currentState = PlayerState.EnemySelected;
                            instance.SelectObject(clickedObject);
                            //instance.ShowSelectionInfo();
                            instance.acceptingInput = true;
                        }*/
                        else {
                            Debug.Log("n/a|m0|def");
                            instance.ClearSelection();
                            instance.acceptingInput = true;
                        }
                    }
                    /*else if (mouseButton == 1) {
                        Debug.Log("n/a|m1");
                        instance.ClearSelection(); instance.acceptingInput = true;
                        if (clickedObject.tag == "Character" || clickedObject.tag == "Enemy") {
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = clickedObject.GetComponent<Character>();
                            //instance.ShowSelectionInfo();
                            instance.acceptingInput = true;
                        }
                        else { instance.ClearSelection(); instance.acceptingInput = true; }
                    }*/
                    break;
                case PlayerState.CharacterSelected:
                    Debug.Log("charSel");
                    if (mouseButton == 0) {
                        Debug.Log("charSel|m0");
                        if (clickedObject.tag == "Character" && clickedObject.GetInstanceID() == instance.currentSelection.GetInstanceID()) {
                            Debug.Log("charSel|m0|char:Self");
                            instance.acceptingInput = true;
                        }
                        else if (clickedObject.tag == "Character") {
                            Debug.Log("charSel|m0|char");
                            instance.DisengageMoveContext();
                            instance.SelectObject(clickedObject);
                            instance.currentSelectedCharacter = clickedObject.GetComponent<Character>();
                            instance.EngageMoveContext();
                            instance.acceptingInput = true;
                        }
                        else {
                            Debug.Log("charSel|m0|def");
                            instance.DisengageMoveContext();
                            instance.ClearSelection();
                            instance.acceptingInput = true;
                        }
                    }
                    else if (mouseButton == 1) {
                        Debug.Log("charSel|m1");
                        if (clickedObject.tag == "Tile" && clickedObject.GetComponent<Cell>().IsTraversable && !clickedObject.GetComponent<Cell>().isOccupied
                            && instance.currentSelectedCharacter.moving == false && instance.currentSelectedCharacter.GetMovePaths().Contains(clickedObject.GetComponent<Cell>())) {
                            Debug.Log("charSel|m1|tile");
                            instance.DisengageMoveContext();
                            instance.MoveTo(instance.currentSelection, clickedObject);
                        }
                        else if (clickedObject.tag == "Enemy") {
                            Debug.Log("charSel|m1|nme");
                            instance.currentTarget = clickedObject.GetComponent<Character>();
                            instance.acceptingInput = false;
                            instance.DisengageMoveContext();
                            instance.EngageTargetContext();
                        }
                        else {
                            Debug.Log("charSel|m1|def");
                            instance.DisengageMoveContext();
                            instance.ClearSelection();
                            instance.acceptingInput = true;
                        }
                        //if(clickedObject.tag == "Enemy") {}
                        //if(clickedObject.tag == "Character") {}
                    }
                    break;
                case PlayerState.TargetingEnemies:
                    Debug.Log("tarNME");
                    if(mouseButton == 0) {
                        Debug.Log("tarNME|m0");
                        if (clickedObject.tag == "Enemy" && instance.targets.Contains(clickedObject.GetComponent<Character>())) {
                            Debug.Log("tarNME|m0|NME");
                            instance.DisengageTargetEnemyContext(true);
                            instance.currentTarget = clickedObject.GetComponent<Character>();
                            instance.currentTarget.transform.GetComponentInChildren<SpriteRenderer>().color = instance.highlight;
                            instance.acceptingInput = true;
                            //show attack vs defense info
                        }
                        else {
                            Debug.Log("tarNME|m0|def");
                            instance.DisengageTargetEnemyContext(false);
                            instance.ClearSelection();
                            instance.acceptingInput = true;
                        }
                        //if(clickedObject.tag == "Enemy") { }//attack coroutine
                    }
                    else if(mouseButton == 1) {
                        Debug.Log("tarNME|m1|def");
                        instance.DisengageTargetEnemyContext(false);
                        instance.ClearSelection();
                        instance.acceptingInput = true;
                    }
                    break;
            }

        }
    }
    public static void Event(Vector2 moveVector) {
        //in menu

        //in game
        instance.MoveCamera(new Vector3(moveVector.x, moveVector.y, 0f));
    }
    public static void Event() {

    }
    public static void EndTurn() {
        if (instance.acceptingInput) {
            instance.acceptingInput = false;
            if (instance.GetComponent<RuleSetEngine>().EndTurn()) { instance.StartCoroutine(instance.EnableInput()); }
        }
    }
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
    public void FinishedPlacingNewCharacter() {
        ClearSelection();
        StartCoroutine(EnableInput());
    }
    private IEnumerator EnableInput() {
        yield return new WaitForSeconds(0.5f);
        instance.acceptingInput = true;
    }
    void ShowSelectionInfo() {
        if (currentState == PlayerState.CharacterSelected) { currentSelectedCharacter.PrintAttributes(); }
    }
    //================================================================================================================================================
    //=== Context Methods
    //================================================================================================================================================
    public void EngageMoveContext() {
        if (currentSelectedCharacter != null) {
            List<Cell> cellsToHighLight = currentSelectedCharacter.GetMovePaths();
            //Check that list is populated.
            if (cellsToHighLight.Count > 0) {
                bool arePathsStillTraversable = true;
                //Check that cells in list are still traversable and unoccupied.
                foreach (Cell cell in cellsToHighLight) {
                    if(cell.isOccupied || !cell.IsTraversable) { arePathsStillTraversable = false; }
                }
                //If a cell is found to be no longer traversable or occupied then repath using map.
                if (!arePathsStillTraversable) {
                    cellsToHighLight = map.HighlightAllPathsUnderWeight(currentSelectedCharacter.CurrentCell.Position, currentSelectedCharacter.currentActionPoints);
                    currentSelectedCharacter.SetMovePaths(cellsToHighLight);
                }
            }
            else {
                //If list is unpouplated, get paths from map;
                cellsToHighLight = map.HighlightAllPathsUnderWeight(currentSelectedCharacter.CurrentCell.Position, currentSelectedCharacter.currentActionPoints);
                currentSelectedCharacter.SetMovePaths(cellsToHighLight);
            }
            //Highlight path.
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
    public void EngageTargetContext() {
        Debug.Log("EngageTargetContext");
        if (currentSelectedCharacter != null && currentTarget != null) {
            switch (currentTarget.Type) {
                case Character.CharacterType.Friendly:
                    acceptingInput = true; ;
                    break;
                case Character.CharacterType.Enemy:
                    instance.currentState = PlayerState.TargetingEnemies;
                    StartCoroutine(currentSelectedCharacter.inventory.GetWeaponRange());
                    break;
            }
        }
    }
    public void DisengageTargetContext() {

    }
    private Color highlight = new Color32(255, 130, 130, 255);
    private Color noHighlight = new Color32(255, 255, 255, 255);
    private List<Character> targets;
    public void EngageTargetEnemyContext(List<Character> targs) {
        if (targets == null || targets != targs) {
            if (currentSelectedCharacter.inventory.currentWeapon.canHitDiagonal) { targets = targs; }
            else {
                targets = new List<Character>();
                foreach (Character targ in targs) {
                    if(map.GetAdjList(currentSelectedCharacter.CurrentCell.Position, false).Contains(targ.CurrentCell.Position)){
                        targets.Add(targ);
                    }
                }
            }
        }

        //do something else, highlight for now
        foreach (Character targ in targets) {
            //if can hit diagonal target or target not diagonal
            if (currentSelectedCharacter.inventory.currentWeapon.canHitDiagonal || map.GetAdjList(currentSelectedCharacter.CurrentCell.Position, false).Contains(targ.CurrentCell.Position)){
                targ.gameObject.GetComponentInChildren<SpriteRenderer>().color = highlight;
                map.HighLightCell(targ.CurrentCell.Position);
            }
        }
        acceptingInput = true;
    }
    public void DisengageTargetEnemyContext(bool keepTargets) {
        foreach(Character targ in targets) {
            targ.gameObject.GetComponentInChildren<SpriteRenderer>().color = noHighlight;
            map.ClearHighLight(targ.CurrentCell.Position);
        }
        if (!keepTargets) { targets = null; }
        acceptingInput = true;
    }
    public void EngagePowerContext() {

    }
}
