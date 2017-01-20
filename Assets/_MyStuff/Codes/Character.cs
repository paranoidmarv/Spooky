using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
    //--- Utility Variables
    public int cellX, cellY;
    private Cell currentCell;
    public Cell CurrentCell {
        get { return currentCell; }
    }
    private PlayerHandler playerHandler;
    public PlayerHandler PlayerHandler {
        get { return playerHandler; }
        set { playerHandler = value; }
    }
    public float moveAnimationSpeed;
    public float moveDuration;
    public bool moving;

    private List<Cell> movePaths;
    private bool movePathsDiscovered;
    public TextAsset characterTemplate;
    //--- hardcode temporary template
    int defaultPrimaryHP = 250;
    int defaultPrimaryAP = 50;
    int defaultPhysical = 20;
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    //--- Primary Attributes                                                                                                                        
    private Dictionary<int, Tuple<Attribute, int>> primaryMap;
    private List<string> primaryList;
    public int currentHealth, currentActionPoints;
    //--- Physical Attributes
    private Dictionary<int, Tuple<Attribute, int>> physicalMap;
    private List<string> physicalList;
    //--- Ancillary Attributes
    private Dictionary<int, Tuple<Attribute, int>> ancillaryMap;
    private List<string> ancillaryList;

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    void Awake() {
        primaryMap = new Dictionary<int, Tuple<Attribute, int>>();
        primaryList = new List<string>();
        physicalMap = new Dictionary<int, Tuple<Attribute, int>>();
        physicalList = new List<string>();
        ancillaryMap = new Dictionary<int, Tuple<Attribute, int>>();
        ancillaryList = new List<string>();
        movePaths = new List<Cell>();
        movePathsDiscovered = false;
        moving = false;

        //currentActionPoints = maxActionPoints;
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    //--- Actions

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    private int moveCost = 0;
    public void Move(List<Vector3> moveToPath, Cell newCurrentCell, int movCost) {
        moveCost = movCost;
        StartCoroutine("MoveTween", new Tuple<List<Vector3>, Cell>(moveToPath, newCurrentCell));
    }

    private IEnumerator MoveTween(Tuple<List<Vector3>, Cell> moveToArgs) {
        moving = true;
        //currentCell.isOccupied = false;
        //Add check for same direction movement to smooth animation and reduce calls
        for (int i = 0; i < moveToArgs.First.Count; i++) {
            iTween.MoveTo(gameObject, iTween.Hash(
                "position", moveToArgs.First[i],
                "time", moveAnimationSpeed
            ));
            yield return new WaitForSeconds(moveDuration);
        }
        SetCell(moveToArgs.Second);
        moving = false;
        ModifyActionPoints(moveCost);
        moveCost = 0;
        playerHandler.AcceptingInput = true;
    }
    //--- Primary Attribute methods
    private void ModifyHealthPoints(int changeHP) {
        if (currentHealth + changeHP > primaryMap[0].First.attributeRange.Second) { currentHealth = primaryMap[0].First.attributeRange.Second; }//make condition for buffed health points
        else if (currentHealth + changeHP <= 0) { currentHealth = 0; }// code for you're dead
        else { currentHealth += changeHP; }
    }
    private void ModifyActionPoints(int changeAP) {
        if (currentActionPoints + changeAP > primaryMap[1].First.attributeRange.Second) { currentActionPoints = primaryMap[1].First.attributeRange.Second; }//make condition for buffed action points
        else if (currentActionPoints + changeAP <= 0) { currentActionPoints = 0; }
        else { currentActionPoints += changeAP; }
        playerHandler.DisengageMoveContext();
        ResetMovePaths();
        playerHandler.EngageMoveContext();
    }
    //--- Context Methods
    public void SetMovePaths(List<Cell> movPaths) {
        movePaths = movPaths;
        movePathsDiscovered = true;
    }
    public List<Cell> GetMovePaths() {
        if (movePathsDiscovered == true) {
            return movePaths;
        }
        else { return new List<Cell>(); }
    }
    public void ResetMovePaths() {
        movePaths.Clear();
        movePathsDiscovered = false;
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    //--- Cell Methods

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    public void SetCell(Cell newCurrentCell) {
        if (currentCell != null) {
            currentCell.isOccupied = false;
        }
        currentCell = newCurrentCell;
        currentCell.isOccupied = true;
        SetToCellPosition();
    }
    private void SetToCellPosition() {
        transform.position = new Vector3(currentCell.Position.First, currentCell.Position.Second, -1f);
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------//

    //--- Attribute Methods

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    public void InitializeCharacterAttributes(List<Attribute> pAttributes, List<Attribute> phAttributes, List<Attribute> aAttributes) {
        foreach(Attribute att in pAttributes) {
            int defValue;
            if(att.ID == 0) { defValue = defaultPrimaryHP; }
            else { defValue = defaultPrimaryAP; }
            primaryMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            primaryList.Add(att.name);
        }
        currentHealth = defaultPrimaryHP;
        currentActionPoints = defaultPrimaryAP;
        foreach (Attribute att in phAttributes) {
            physicalMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            physicalList.Add(att.name);
        }
        foreach (Attribute att in aAttributes) {
            ancillaryMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            ancillaryMap[att.ID].Second = ComputeAncillaryValue(att.ID);
            ancillaryList.Add(att.name);
        }
        //PrintAttributes();
    }
    public void ModifyAttribute(int attID, Attribute.AttributeType type, int modValue) {
        if(type == Attribute.AttributeType.Physical) {
            physicalMap[attID].Second += modValue;
            foreach (int derivedAttID in GetDerivedAttributeIDs(attID)) {
                ancillaryMap[derivedAttID].Second = ComputeAncillaryValue(derivedAttID);
            }
        }
    }
    //-----------------------------------------------------------------------------
    public int GetPhysicalAttributeValue(int attID) {
        return physicalMap[attID].Second;
    }

    public float GetAncillayrAttributeValue(int attID) {
        return ancillaryMap[attID].Second;
    }
    public string GetPhysicalAttributeName(int attID) {
        return physicalList[attID];
    }
    /*public int GetAttributeID(Attribute.AttributeType attType, string attName) {
        if(attType == Attribute.AttributeType.Physical) { return physicalMap[attName].First.ID; }
        else { return -1; }
    }*/
    //-----------------------------------------------------------------------------
    public void PrintAttributes() {
        foreach (string att in primaryList) {
            Debug.Log(att + " : " + primaryMap[primaryList.IndexOf(att)].Second);
        }
        foreach (string att in physicalList) {
            Debug.Log(att + " : " + physicalMap[physicalList.IndexOf(att)].Second);
        }
        foreach (string att in ancillaryList) {
            Debug.Log(att + " : " + ancillaryMap[ancillaryList.IndexOf(att)].Second);
        }
    }
    private void ComputePrimaryValue() {

    }
    //--- Computes and returns the value of a derived ancillary attribute based on its parented physical attributes.
    //--- Feed ancillary attribute ID.
    private int ComputeAncillaryValue(int attID) {
        int phAttID = ancillaryMap[attID].First.parentedPhysicalAttribute;
        int mod = Mathf.FloorToInt((physicalMap[phAttID].Second - physicalMap[phAttID].First.defaultValue) / ancillaryMap[attID].First.attributeRatio);
        return ancillaryMap[attID].First.defaultValue + mod;
    }
    public int[] GetDerivedAttributeIDs(int attID) {
        List<int> derivedAttributes = new List<int>();
        //foreach(int primEffID in physicalMap[attID].First.primaryEffects) { derivedAttributes.Add(primEffID); }
        foreach(int aEffID in physicalMap[attID].First.ancillaryEffects) { derivedAttributes.Add(aEffID); }
        return derivedAttributes.ToArray();
    }
    public void DestroyThis() {
        Destroy(gameObject);
    }
}
