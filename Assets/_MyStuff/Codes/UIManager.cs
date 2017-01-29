using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public GameObject characterCreationPanel;
    private CharacterCreator cc;
    public SceneManager sceneManager;
    private GameObject makeNewCharacter;
	// Use this for initialization
	void Awake () {
        //characterCreationPanel = GameObject.Find("Character Creation Panel");
        sceneManager = GameObject.Find("Player Manager").GetComponent<SceneManager>();
        makeNewCharacter = GameObject.Find("MakeNewCharacter");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PauseGame() {
        Time.timeScale = 0;
    }

    public void UnPauseGame() {
        Time.timeScale = 1;
    }

    public void OpenCharacterCreator() {
        if (Time.timeScale == 1) {
            if(cc == null) { cc = cc = characterCreationPanel.GetComponent<CharacterCreator>(); }
            PauseGame();
            characterCreationPanel.SetActive(true);
            makeNewCharacter.GetComponentInChildren<UILabel>().text = "Cancel";
            sceneManager.playerHandler.AcceptingInput = false;
            //cc.enabled = true;
        }
        else {
            UnPauseGame();
            characterCreationPanel.SetActive(false);
            makeNewCharacter.GetComponentInChildren<UILabel>().text = "Create New Character";
            sceneManager.playerHandler.AcceptingInput = false;
            //cc.enabled = false;
        }
        
    }
}
