using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
    public List<Character> characters;
    public Map sceneMap;
    public PlayerHandler playerHandler;
    public RuleSetEngine ruleSetEngine;
    private UIManager uiManager;
    public bool sceneReady;
	// Use this for initialization
	void Awake () {
        sceneReady = false;
        characters = new List<Character>();
        sceneMap = GameObject.Find("Map").GetComponent<Map>();
        playerHandler = GameObject.Find("Player Manager").GetComponent<PlayerHandler>();
        ruleSetEngine = GameObject.Find("Player Manager").GetComponent<RuleSetEngine>();
        uiManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        sceneMap.MakeMap();
        StartCoroutine(GetCharacters());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitializeNewCharacter(GameObject nChar) {
        Character newCharacter = nChar.GetComponent<Character>();
        newCharacter.PlayerHandler = playerHandler;
        newCharacter.InitializeCharacterAttributes(ruleSetEngine.primaryAttributes, ruleSetEngine.physicalAttributes, ruleSetEngine.ancillaryAttributes, ruleSetEngine.professions);
    }

    public void AddNewCharacter(GameObject nChar) {
        characters.Add(nChar.GetComponent<Character>());
    }

    private IEnumerator GetCharacters() {
        bool gotCharacters = false;
        while (!gotCharacters) {
            if (ruleSetEngine.isRuleSetLoaded) {
                GameObject[] characterGameObjects = GameObject.FindGameObjectsWithTag("Character");
                for (int i = 0; i < characterGameObjects.Length; i++) {
                    characters.Add(characterGameObjects[i].GetComponent<Character>());
                    characters[i].SetCell(sceneMap.GetCell(new Tuple<int, int>(characters[i].cellX, characters[i].cellY)));
                    characters[i].PlayerHandler = playerHandler;
                    characters[i].InitializeCharacterAttributes(ruleSetEngine.primaryAttributes, ruleSetEngine.physicalAttributes, ruleSetEngine.ancillaryAttributes, ruleSetEngine.professions);
                }
                gotCharacters = true;
            }
            else { yield return new WaitForSeconds(0.2f); }
        }
        sceneReady = true;
    }
}
