using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    int targetX;
    int targetY;
    public bool isMatched = false;

    public int col;
    public int row;
    public int prevRow;
    public int prevCol;
    public GameObject otherDot;
    Board board;


    EndGameManager endGameManager;
    HintManager hintManager;
    FindMatches _findMatches;

    Vector2 firstTouchPosition = Vector2.zero;
    Vector2 finalTouchPosition = Vector2.zero;
    Vector2 _tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle;
    [SerializeField] float swipeResist = 0.5f;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    [SerializeField] GameObject adjacentBomb;
    [SerializeField] GameObject rowArrow;
    [SerializeField] GameObject columnArrow;
    [SerializeField] GameObject colorBomb;


    // Start is called before the first frame update
    void Start() {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        _findMatches = FindObjectOfType<FindMatches>();
         endGameManager = FindObjectOfType<EndGameManager>();
        /*col = targetX;
        row = targetY;
        prevRow = row;
        prevCol = col;*/
    }

    // Update is called once per frame
    void Update() {
        /*if (isMatched)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0f, 0f, 0f, .2f);
        }
        */
        targetX = col;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) {
            // Move toward
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, .04f);

            if(board.allDots[col, row] != this.gameObject) {
                board.allDots[col, row] = this.gameObject;
                _findMatches.FindAllMatches();
            }
        } else {
            // Directly set
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = _tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1) {
            // Move toward
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, .04f);
            if (board.allDots[col, row] != this.gameObject) {
                board.allDots[col, row] = this.gameObject;
                _findMatches.FindAllMatches();
            }
        }
        else {
            // Directly set
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = _tempPosition;
        }
    }

    void OnMouseDown() {
        if (hintManager != null) {
            hintManager.DestroyHint();
        }
        if (board.currentState == GameState.move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            makeColorBomb();
        }
    }


    void OnMouseUp() {
        if (board.currentState == GameState.move) {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalcAngle();
        } else {
            board.currentState = GameState.move;
        }
        Debug.Log(this);
    }

    void CalcAngle() {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        } else {
            board.currentState = GameState.move;
        }
       
    }

    void MovePiecesActual(Vector2 direction) {
        otherDot = board.allDots[col + (int)direction.x, row + (int)direction.y];
        prevRow = row;
        prevCol = col;

        if (board.lockTiles[col, row] == null && board.lockTiles[col + (int)direction.x, row + (int)direction.y] == null) {
            if (otherDot != null) {
                otherDot.GetComponent<Dot>().col += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                col += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            } else {
                board.currentState = GameState.move;
            }
        } else {
            board.currentState = GameState.move;
        }
    }

    void MovePieces() {
        if (swipeAngle > -45 && swipeAngle <= 45 && col < board.width - 1) {
            MovePiecesActual(Vector2.right);
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0) {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            MovePiecesActual(Vector2.down);
        }

        board.currentState = GameState.move;
    }

    IEnumerator CheckMoveCo() {
        if (isColorBomb) {
            _findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        } else if (otherDot.GetComponent<Dot>().isColorBomb) {
            _findMatches.MatchPiecesOfColor(gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }

        yield return new WaitForSeconds(.5f);
        if (otherDot != null) {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().col = col;
                row = prevRow;
                col = prevCol;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            } else {
                if (endGameManager != null) {
                    if (endGameManager.requirements.gameType == GameType.Moves) {
                        endGameManager.DecreaseCounter();
                    }
                }

                board.DestroyMatches();
            }

            otherDot = null;
        }
    }

    public void makeRowBomb() {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb) {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void makeColumnBomb() {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb) {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void makeColorBomb() {
        if (!isRowBomb && !isColumnBomb && !isAdjacentBomb) {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            gameObject.tag = "Color";
        }
    }

    public void makeAdjacentBomb() {
        if (!isRowBomb && !isColumnBomb && !isColorBomb) {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }
}
