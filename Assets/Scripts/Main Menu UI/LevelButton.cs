using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour {
    public TextMeshProUGUI currText;
    public string levelToLoad;

    GameData gameData;

    void Start() {
        gameData = FindObjectOfType<GameData>();
        int level = gameData ? gameData.saveData.level : 0;
        currText.text = "Level" + " " + level;
    }

    public void Play() {
        SceneManager.LoadScene(levelToLoad);
    }
}
