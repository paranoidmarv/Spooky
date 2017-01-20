using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
    private Color highlight = new Color32(130, 255, 255, 255);
    private Color noHighlight = new Color32(255, 255, 255, 255);
    private SpriteRenderer cellRenderer;
    private bool isBorder;
    public bool IsBorder {
        get { return isBorder; }
    }
    private bool isTraversable;
    public bool IsTraversable {
        get { return isTraversable; }
    }
    public bool isOccupied;
    //
    //private bool isOccupied
    //
    private Tuple<int, int> position;
    public Tuple<int, int> Position {
        get { return position; }
    }
    private Dictionary<PathDirection, Cell> paths; //maybe list so if(path is in pathsList) { traverse(); }
    public List<Tuple<int, int>> adjCellPosList; //Add adjacency traversable list to cut down on Astar PathFind lag
    public List<Tuple<int, int>> traversableAdjCellPosList;
    public enum PathDirection { NORTH, SOUTH, EAST, WEST, DOWN, UP, PORTAL };
    public Map parentMap;

    void Awake() {
        cellRenderer = GetComponent<SpriteRenderer>();
        paths = new Dictionary<PathDirection, Cell>();
        adjCellPosList = new List<Tuple<int, int>>();
        traversableAdjCellPosList = new List<Tuple<int, int>>();
        isOccupied = false;
    }

    public void SetCell(bool iB, bool iT, Tuple<int, int> pos, Sprite cellSprite) {
        isBorder = iB;
        isTraversable = iT;
        position = new Tuple<int, int>(pos.First, pos.Second);
        cellRenderer.sprite = cellSprite;
    }

    //Interactions
    int clickReturn = -1;
    void OnMouseEnter() {
        if (parentMap.placingNewCharacter == true && IsTraversable && !isOccupied) {
            cellRenderer.color = highlight;
            parentMap.PlaceNewCharacterHighLight(this);
        }
    }
    void OnMouseExit() {
    }

    void OnMouseUp() {
        if(parentMap.placingNewCharacter == true && IsTraversable && !isOccupied) {
            parentMap.newCharacter.GetComponent<PlaceNewCharacter>().PlaceCharacter();
        }
    }

    public void AddPath(PathDirection pathDirection, Cell newPath) {
        paths.Add(pathDirection, newPath);
        adjCellPosList.Add(newPath.position);
        if (newPath.isTraversable) { traversableAdjCellPosList.Add(newPath.position); }
        //adjacentCells.Add(newPath);
    }

    public bool PathExists(PathDirection pathDirection) {
        Cell stFuBuSh;
        return paths.TryGetValue(pathDirection, out stFuBuSh);
    }

    public void PrintPaths() {
        Cell value;
        if (paths.TryGetValue(PathDirection.EAST, out value)) { Debug.Log("EAST " + value.Position.First + " " + value.Position.Second); }
        if (paths.TryGetValue(PathDirection.NORTH, out value)) { Debug.Log("NORTH " + value.Position.First + " " + value.Position.Second); }
        if (paths.TryGetValue(PathDirection.SOUTH, out value)) { Debug.Log("SOUTH " + value.Position.First + " " + value.Position.Second); }
        if (paths.TryGetValue(PathDirection.WEST, out value)) { Debug.Log("WEST " + value.Position.First + " " + value.Position.Second); }
    }

    public void HighLightCell() {
        cellRenderer.color = highlight;
    }
    public void ClearHighLight() {
        cellRenderer.color = noHighlight;
    }
}
