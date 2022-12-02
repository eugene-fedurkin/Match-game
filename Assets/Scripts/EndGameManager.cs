using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameType {
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements {
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour {
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject youLosePanel;
    public TextMeshProUGUI counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    
    float timesSeconds;
    Board board;

    void Start() {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    }

    void Update() {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0) {
            timesSeconds -= Time.deltaTime;
            if (timesSeconds <= 0) {
                DecreaseCounter();
                timesSeconds = 1;
            }
        }
    }

    void SetGameType() {
        if (board.world != null) {
            if (board.world.levels[board.level] != null) {
                requirements = board.world.levels[board.level].endGameRequirements;
            }
        }
    }

    void SetupGame() {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves) {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        } else {
            timesSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + currentCounterValue;
    }

    public void WinGame() {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame() {
        youLosePanel.SetActive(true);
        board.currentState = GameState.lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void DecreaseCounter() {
        if (board.currentState != GameState.pause) {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;

            if (currentCounterValue <= 0) {
                LoseGame();
            }
        }
    }
}
