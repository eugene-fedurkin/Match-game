using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator paneAnimation;
    public Animator gameInfoAnimation;

    void Start()
    {
        
    }

    public void GameOver()
    {
        paneAnimation.SetBool("Out", false);
        paneAnimation.SetBool("Game Over", true);
    }

    public void ok() {
        if (paneAnimation && gameInfoAnimation) {
            paneAnimation.SetBool("Out", true);
            gameInfoAnimation.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
    }

    IEnumerator GameStartCo() {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
