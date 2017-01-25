using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public GameObject characterCreationPanel;
    private CharacterCreator cc;
    public SceneManager sceneManager;
	// Use this for initialization
	void Awake () {
        //characterCreationPanel = GameObject.Find("Character Creation Panel");
        sceneManager = GameObject.Find("Player Manager").GetComponent<SceneManager>();
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
            //cc.enabled = true;
        }
        else {
            UnPauseGame();
            characterCreationPanel.SetActive(false);
            //cc.enabled = false;
        }
        
    }
}
