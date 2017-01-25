using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour {
    /*
    Takes image input and reads pixel to make map composed of cells. Currently also applies textures.
    Takes orders from Player Handler. 
    Tells A* to return paths to relay to Player Handler.
    Main source for returning cell info or the cells themselves using Tuple<int, int> args.
    */
    public Color NOCELL, BORDER, TRAVERSABLE, UNTRAVERSABLE;
    public Sprite borderSprite, traversableSprite, untravserableSprite;

    public Texture2D colorMap;
    private int width;
    private int height;
    private Color[] pixels;

    private string mapName;
    public string MapName {
        get { return mapName; }
    }
    private Cell[,] cellGrid;
    public GameObject cellPrefab;

    //--- Special Fields
    public bool placingNewCharacter;
    public Cell highLightedCell;
    public bool switchingHighLight = true;
    public GameObject newCharacter;

    public void PlaceNewCharacterHighLight(Cell cell) {
        switchingHighLight = true;
        if (highLightedCell != null) {
            highLightedCell.ClearHighLight();
        }
        highLightedCell = cell;
        switchingHighLight = false;
    }

    //--- Initialization
    public void MakeMap() {
        placingNewCharacter = false;
        width = colorMap.width;
        height = colorMap.height;
        pixels = new Color[width * height];
        pixels = colorMap.GetPixels();
        cellGrid = new Cell[width, height];

        Tuple<int, int>[] aStarTuples = new Tuple<int, int>[width * height];
        for (int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                if(pixels[(y * width) + x] != NOCELL) {
                    GameObject newCell = (GameObject)Instantiate(cellPrefab, new Vector2(x, y), Quaternion.identity);
                    newCell.transform.parent = transform;
                    newCell.GetComponent<Cell>().parentMap = this;
                    cellGrid[x, y] = newCell.GetComponent<Cell>();
                    if(MatchColorMap(new Tuple<int, int>(x, y), cellGrid[x, y], pixels[(y * width) + x])) {
                        FindPaths(x, y);
                        aStarTuples[((y * width) + x)] = new Tuple<int, int>(x, y);
                    }
                }
            }
        }
        AStar.InitializeAStar(this, aStarTuples);
    }
    private bool MatchColorMap(Tuple<int, int> pos, Cell currentCell, Color check) {
        if(check == BORDER) {
            currentCell.SetCell(true, false, pos, borderSprite);
        }
        else if(check == TRAVERSABLE) {
            currentCell.SetCell(false, true, pos, traversableSprite);
        }
        else if(check == UNTRAVERSABLE) {
            currentCell.SetCell(false, false, pos, untravserableSprite);
        }
        return true;
    }
    private void FindPaths(int x, int y) {
        if(!(cellGrid[x, y].PathExists(Cell.PathDirection.WEST)) && (x - 1) >= 0 && cellGrid[x - 1, y] != null) {
            //if(cellGrid[x - 1, y].IsTraversable) {
                cellGrid[x, y].AddPath(Cell.PathDirection.WEST, cellGrid[x - 1, y]);
                cellGrid[x - 1, y].AddPath(Cell.PathDirection.EAST, cellGrid[x, y]);
            //}
        }
        if(!(cellGrid[x, y].PathExists(Cell.PathDirection.EAST)) && (x + 1) < width && cellGrid[x + 1, y] != null) {
            //if(cellGrid[x + 1, y].IsTraversable) {
                cellGrid[x, y].AddPath(Cell.PathDirection.EAST, cellGrid[x + 1, y]);
                cellGrid[x + 1, y].AddPath(Cell.PathDirection.WEST, cellGrid[x, y]);
            //}
        }
        if(!(cellGrid[x, y].PathExists(Cell.PathDirection.SOUTH)) && (y - 1) >= 0 && cellGrid[x, y - 1] != null) {
            //if (cellGrid[x, y - 1].IsTraversable) {
                cellGrid[x, y].AddPath(Cell.PathDirection.SOUTH, cellGrid[x, y - 1]);
                cellGrid[x, y - 1].AddPath(Cell.PathDirection.NORTH, cellGrid[x, y]);
            //}
        }
        if(!(cellGrid[x, y].PathExists(Cell.PathDirection.NORTH)) && (y + 1) < height && cellGrid[x, y + 1] != null) {
            //if (cellGrid[x , y + 1].IsTraversable) {
                cellGrid[x, y].AddPath(Cell.PathDirection.NORTH, cellGrid[x, y + 1]);
                cellGrid[x, y + 1].AddPath(Cell.PathDirection.SOUTH, cellGrid[x, y]);
            //}
        }
    }
    //--- Path Utilities
    public List<Cell> HighlightAllPathsUnderWeight(Tuple<int, int> sourceCellPosition, float pointPool) {
        List<Tuple<Cell, float>> openCells = new List<Tuple<Cell, float>>();
        List<Cell> closedCells = new List<Cell>();
        foreach(Tuple<int, int> cellPosition in GetAdjList(sourceCellPosition, true)) {
            if((pointPool - GetCellWeight(cellPosition)) >= 0) { openCells.Add(new Tuple<Cell, float>(GetCell(cellPosition), GetCellWeight(cellPosition))); }
        }
        closedCells.Add(GetCell(sourceCellPosition));
        while(openCells.Count > 0) {
            Tuple<Cell, float> cell = openCells[0];
            foreach(Tuple<int, int> cellPosition in GetAdjList(cell.First.Position, true)) {
                if((pointPool - (cell.Second + GetCellWeight(cellPosition))) >= 0 && !closedCells.Contains(GetCell(cellPosition))) { openCells.Add(new Tuple<Cell, float>(GetCell(cellPosition), GetCellWeight(cellPosition) + cell.Second)); }
            }
            openCells.Remove(cell);
            if (cell.First.IsTraversable && !cell.First.isOccupied  && !closedCells.Contains(cell.First)) {
                closedCells.Add(cell.First);
            }
        }
        return closedCells;
    }

    //--- AStar methods
    public List<Tuple<int, int>> GetAdjList(Tuple<int, int> cellPosition, bool onlyTraversable) {
        if (onlyTraversable) {
            //===HERE IS WHERE YOU FILTER OUT OCCUPIED CELLS===//
            //Also, you will need to be able to edit cellGrid to remove tiles if they become !traversable
            return cellGrid[cellPosition.First, cellPosition.Second].traversableAdjCellPosList;
        }
        else { return cellGrid[cellPosition.First, cellPosition.Second].adjCellPosList; }
    }

    public int NodeIndex(Tuple<int, int> cellPosition) {
        return ((cellPosition.Second * width) + cellPosition.First);
    }

    public int GetCellWeight(Tuple<int, int> cellPos) {
        //Implement move cost on tiles
        return 5;
        //Implement move cost on tiles
    }

    //--- Cell Talk
    public Cell GetCell(Tuple<int, int> cellPosition) {
        return cellGrid[cellPosition.First, cellPosition.Second];
    }
    public void HighLightCell(Tuple<int, int> cellPos) {
        cellGrid[cellPos.First, cellPos.Second].HighLightCell();
    }
    public void ClearHighLight(Tuple<int, int> cellPos) {
        cellGrid[cellPos.First, cellPos.Second].ClearHighLight();
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------//
    //--------------------------------------------------------------------------------------------------------------------------------------------------------//
    //--------------------------------------------------------------------------------------------------------------------------------------------------------//
    /*private bool isCardinalHeuristicInitialized = false;
    private Tuple<List<Cell.PathDirection>, List<Cell.PathDirection>> directionLists;

    private enum Direction : sbyte { Negative = -1, None = 0, Positive = 1 };
    private Tuple<List<Cell.PathDirection>, List<Cell.PathDirection>> CardinalHeuristicInitialization(Tuple<int, int> origin, Tuple<int, int> destination) {      
        List<Cell.PathDirection> cellPathDirectionsAlpha = new List<Cell.PathDirection>();
        List<Cell.PathDirection> cellPathDirectionsBeta = new List<Cell.PathDirection>();
        Direction xDirection, yDirection;

        xDirection = FindDirection(origin.First, destination.First);
        yDirection = FindDirection(origin.Second, destination.Second);

        if(xDirection == Direction.Negative) {
            cellPathDirectionsAlpha.Add(Cell.PathDirection.WEST);
            cellPathDirectionsBeta.Add(Cell.PathDirection.EAST);
        }
        else if(xDirection == Direction.Positive) {
            cellPathDirectionsAlpha.Add(Cell.PathDirection.EAST);
            cellPathDirectionsBeta.Add(Cell.PathDirection.WEST);
        }
        if (yDirection == Direction.Negative) {
            cellPathDirectionsAlpha.Add(Cell.PathDirection.SOUTH);
            cellPathDirectionsBeta.Add(Cell.PathDirection.NORTH);
        }
        else if (yDirection == Direction.Positive) {
            cellPathDirectionsAlpha.Add(Cell.PathDirection.NORTH);
            cellPathDirectionsBeta.Add(Cell.PathDirection.SOUTH);
        }
        directionLists = new Tuple<List<Cell.PathDirection>, List<Cell.PathDirection>>(cellPathDirectionsAlpha, cellPathDirectionsBeta);
        return directionLists;
    }

    private Direction FindDirection(int origin, int destination) {
        if(origin < destination) { return Direction.Positive; }
        else if(origin > destination) { return Direction.Negative; }
        else { return Direction.None; }
    }*/
}
