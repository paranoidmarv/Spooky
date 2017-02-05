using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
    public enum CharacterType { Friendly, Neutral, Enemy }
    public bool isPlayerControlled;

    public CharacterType type;
    public CharacterType Type { get { return type; } }
    public void SetCharacterType(CharacterType cT) {
        //%%%%%%%%%%%%%%%%%%%%%Change Tag
        type = cT;
    }
    //==== Utility Variables
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
    //private AIHandler aH;
    public TextAsset characterTemplate;
    //===============================================================================
    //=== Action Variables
    //===============================================================================
    //=== Move
    public float moveAnimationSpeed;
    public float moveDuration;
    public bool moving;
    private List<Cell> movePaths;
    private bool movePathsDiscovered;

    public Inventory inventory;
    public ProfessionMonoBehaviour professionMono;

    //===============================================================================
    //=== Attribute Maps
    //===============================================================================
    //--- Primary Attributes                                                                                                                        
    protected Dictionary<int, Tuple<Attribute, int>> primaryMap;
    protected List<string> primaryList;
    public int currentHealth, currentActionPoints, currentGrit;
    //--- Physical Attributes
    protected Dictionary<int, Tuple<Attribute, int>> physicalMap;
    protected List<string> physicalList;
    //--- Ancillary Attributes
    protected Dictionary<int, Tuple<Attribute, int>> ancillaryMap;
    protected List<string> ancillaryList;

    //hard coded
    public int attack;
    public int defense;

    //----------------------------------------------------------------------------------------------------------------------------------------------//
    void Awake() {
        //if (isPlayerControlled) {
            playerHandler = GameObject.Find("Player Manager").GetComponent<PlayerHandler>();
        //}
        primaryMap = new Dictionary<int, Tuple<Attribute, int>>();
        primaryList = new List<string>();
        physicalMap = new Dictionary<int, Tuple<Attribute, int>>();
        physicalList = new List<string>();
        ancillaryMap = new Dictionary<int, Tuple<Attribute, int>>();
        ancillaryList = new List<string>();
        inventory = transform.GetComponentInChildren<Inventory>();
        professionMono = GetComponent<ProfessionMonoBehaviour>();
        movePaths = new List<Cell>();
        movePathsDiscovered = false;
        moving = false;
    }
    //=================================================================================================================================================
    //=== Actions
    //=================================================================================================================================================
    private int moveCost = 0;
    public void Move(List<Vector3> moveToPath, Cell newCurrentCell, int movCost) {
        moveCost = movCost;
        StartCoroutine("MoveTween", new Tuple<List<Vector3>, Cell>(moveToPath, newCurrentCell));
    }
    public void UpdateCharacter() {
        //check all end of turn modifiers
        if (currentActionPoints < primaryMap[1].Second) {
            ModifyActionPoints(ancillaryMap[ancillaryList.IndexOf("AP Regeneration")].Second);
        }
    }
    public void UseWeapon(int weaponIndex) {
        if(inventory.equippedWeapons[weaponIndex].itemType == Item.ItemType.CQCWeapon) {
            CQCWeapon wep = (CQCWeapon)inventory.equippedWeapons[weaponIndex];
            wep.Strike();
        }
    }
    //=================================================================================================================================================
    //=== Primary Attribute methods
    //=================================================================================================================================================
    private void ModifyHealthPoints(int changeHP) {
        if (currentHealth + changeHP > primaryMap[0].Second) { currentHealth = primaryMap[0].Second; }//make condition for buffed health points
        else if (currentHealth + changeHP <= 0) { currentHealth = 0; }// code for you're dead
        else { currentHealth += changeHP; }
    }
    private void ModifyActionPoints(int changeAP) {
        if (currentActionPoints + changeAP > primaryMap[1].Second) { currentActionPoints = primaryMap[1].Second; }//make condition for buffed action points
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
    //===============================================================================================================================================
    //=== Cell Methods
    //===============================================================================================================================================
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
    //===============================================================================================================================================
    //=== Attribute Methods
    //===============================================================================================================================================
    public void InitializeCharacterAttributes(List<Attribute> pAttributes, List<Attribute> phAttributes, List<Attribute> aAttributes, List<Profession> professions) {
        foreach(Attribute att in pAttributes) {
            primaryMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            primaryList.Add(att.name);
        }
        currentHealth = primaryMap[0].Second;
        currentActionPoints = primaryMap[1].Second;
        currentGrit = primaryMap[2].Second;
        foreach (Attribute att in phAttributes) {
            physicalMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            physicalList.Add(att.name);
        }
        foreach (Attribute att in aAttributes) {
            ancillaryMap.Add(att.ID, new Tuple<Attribute, int>(att, att.defaultValue));
            ancillaryMap[att.ID].Second = ComputeAncillaryValue(att.ID);
            ancillaryList.Add(att.name);
        }
        //HARDCODED ATTRIBUTE ID
        attack = ancillaryMap[12].Second;
        defense = ancillaryMap[15].Second;
        foreach (Profession prof in professions) {
            professionMono.chosenProfessions.Add(new Tuple<Profession, int>(prof, 0));
        }
        //PrintAttributes();
    }
    public void ModifyAttribute(int attID, Attribute.AttributeType type, int modValue) {
        if (type == Attribute.AttributeType.Physical) {
            physicalMap[attID].Second += modValue;
            foreach(int pAttID in physicalMap[attID].First.primaryEffects) {
                primaryMap[pAttID].Second = ComputePrimaryValue(pAttID);
            }
            foreach (int derivedAttID in GetDerivedAttributeIDs(attID)) {
                ancillaryMap[derivedAttID].Second = ComputeAncillaryValue(derivedAttID);
            }
            attack = ancillaryMap[12].Second;
            defense = ancillaryMap[15].Second;
        }
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
    //--- Computes and returns the value of a derived ancillary attribute based on its parented physical attributes.
    //--- Feed ancillary attribute ID.
    private int ComputeAncillaryValue(int attID) {
        int phAttID = ancillaryMap[attID].First.parentedPhysicalAttribute;
        int mod = Mathf.FloorToInt((physicalMap[phAttID].Second - physicalMap[phAttID].First.defaultValue) / ancillaryMap[attID].First.attributeRatio);
        return ancillaryMap[attID].First.defaultValue + mod;
    }
    private int ComputePrimaryValue(int attID) {
        int newValue = primaryMap[attID].First.defaultValue;
        //Needs check if primary attribute is directly modified by buff/debuff
        if (physicalMap[primaryMap[attID].First.fullPhysicalEffect].Second >= physicalMap[primaryMap[attID].First.fullPhysicalEffect].First.defaultValue) {
            newValue += physicalMap[primaryMap[attID].First.fullPhysicalEffect].Second - physicalMap[primaryMap[attID].First.fullPhysicalEffect].First.defaultValue;
        }
        foreach (int attHalfID in primaryMap[attID].First.halfPhysicalEffect) {
            if (physicalMap[attHalfID].Second >= physicalMap[attHalfID].First.defaultValue) {
                newValue += Mathf.FloorToInt((physicalMap[attHalfID].Second - physicalMap[attHalfID].First.defaultValue) / 2);
            }
        }
        return newValue;
    }
    
    //=================================================================================================================================================
    //=== Utitility
    //=================================================================================================================================================
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
    public int[] GetPrimaryAttributeIDs(int attID) {
        List<int> primAtts = new List<int>();
        foreach(int primEffID in physicalMap[attID].First.primaryEffects) { primAtts.Add(primEffID); }
        return primAtts.ToArray();
    }
    public int[] GetDerivedAttributeIDs(int attID) {
        List<int> derivedAttributes = new List<int>();
        //foreach(int primEffID in physicalMap[attID].First.primaryEffects) { derivedAttributes.Add(primEffID); }
        foreach (int aEffID in physicalMap[attID].First.ancillaryEffects) { derivedAttributes.Add(aEffID); }
        return derivedAttributes.ToArray();
    }
    public void DestroyThis() {
        Destroy(gameObject);
    }
    private void ApplyNewMax(int attID) {
        if (attID == 0) { currentHealth = primaryMap[0].Second; }
        else if (attID == 1) { currentActionPoints = primaryMap[2].Second; }
        else { currentGrit = primaryMap[3].Second; }
    }
    public void ResetPrimaryAttributes() {
        currentHealth = primaryMap[0].Second;
        currentActionPoints = primaryMap[1].Second;
    }
    public int GetPrimaryAttributeValue(int attID) {
        return primaryMap[attID].Second;
    }
    public int GetPhysicalAttributeValue(int attID) {
        return physicalMap[attID].Second;
    }
    public int GetAncillayrAttributeValue(int attID) {
        return ancillaryMap[attID].Second;
    }
    public string GetPhysicalAttributeName(int attID) {
        return physicalList[attID];
    }
}
