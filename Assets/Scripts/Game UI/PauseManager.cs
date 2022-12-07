using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    public GameObject pausePanel;
    public Board board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    void Start() {
        if (PlayerPrefs.HasKey("Sound")) {
            if (PlayerPrefs.GetInt("Sound") == 0) {
                soundButton.sprite = musicOffSprite;
            } else {
                soundButton.sprite = musicOnSprite;
            }
        } else {
            soundButton.sprite = musicOffSprite;
        }
        pausePanel.SetActive(false);
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update() {
        if (paused && !pausePanel.activeInHierarchy) {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }

        if (!paused && pausePanel.activeInHierarchy) {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    public void SoundButton() {
        if (PlayerPrefs.HasKey("Sound")) {
            if (PlayerPrefs.GetInt("Sound") == 0) {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
            } else {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
            }
        } else {
            soundButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
        }
    }

    public void PauseGame() {
        paused = !paused;
    }

    public void ExitGame() {
        SceneManager.LoadScene("Main Menu");
    }
}
