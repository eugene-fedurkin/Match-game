using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Swipe Stuff")]
    public float swipeAngle;
    [SerializeField] float swipeResist = 0.5f;

    [Header("Powerup Stuff")]
    public bool isColumnBomb;
    public bool isRowBomb;
    [SerializeField] GameObject rowArraw;
    [SerializeField] GameObject colomnArrow;

    int targetX;
    int targetY;
    public bool isMatched = false;

    public int col;
    public int row;
    public int prevRow;
    public int prevCol;
    public GameObject otherDot;
    Board board;
    FindMatches _findMatches;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 _tempPosition;

    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        _findMatches = FindObjectOfType<FindMatches>();
        /*col = targetX;
        row = targetY;
        prevRow = row;
        prevCol = col;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isMatched)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0f, 0f, 0f, .2f);
        }
        */
        targetX = col;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move toward
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, .04f);

            if(board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
            _findMatches.FindAllMatches();
        } else
        {
            // Directly set
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = _tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move toward
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, .04f);
            if (board.allDots[col, row] != this.gameObject)
            {
                board.allDots[col, row] = this.gameObject;
            }
            _findMatches.FindAllMatches();
        }
        else
        {
            // Directly set
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = _tempPosition;
        }
    }

    void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalcAngle();
        } else
        {
            board.currentState = GameState.move;
        }
    }

    void CalcAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
       
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && col < board.width - 1)
        {
            // Right swipe
            otherDot = board.allDots[col + 1, row];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().col -= 1;
            col += 1;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up swipe
            otherDot = board.allDots[col, row + 1];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0)
        {
            // Left swipe
            otherDot = board.allDots[col - 1, row];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().col += 1;
            col -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            otherDot = board.allDots[col, row - 1];
            prevRow = row;
            prevCol = col;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMoveCo());
    }

    IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().col = col;
                row = prevRow;
                col = prevCol;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            } else
            {
                board.DestroyMatches();
            }

            otherDot = null;
        }
    }

    public void makeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArraw, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void makeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(colomnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}
