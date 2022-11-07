using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [SerializeField] float swipeAngle;
    int targetX;
    int targetY;
    public bool isMatched = false;

    [SerializeField] int col;
    [SerializeField] int row;
    public int prevRow;
    public int prevCol;
    GameObject otherDot;
    Board board;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        col = targetX;
        row = targetY;
        prevRow = row;
        prevCol = col;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0f, 0f, 0f, .2f);
        }

        targetX = col;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move toward
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .04f);
        } else
        {
            // Directly set
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[col, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move toward
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .04f);
        }
        else
        {
            // Directly set
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[col, row] = this.gameObject;
        }
    }

    void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalcAngle();
    }

    void CalcAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        MovePieces();
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && col < board._width - 1)
        {
            // Right swipe
            otherDot = board.allDots[col + 1, row];
            otherDot.GetComponent<Dot>().col -= 1;
            col += 1;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board._height - 1)
        {
            // Up swipe
            otherDot = board.allDots[col, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0)
        {
            // Left swipe
            otherDot = board.allDots[col - 1, row];
            otherDot.GetComponent<Dot>().col += 1;
            col -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            otherDot = board.allDots[col, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        if (col > 0 && col < board._width - 1) {
            GameObject letfDot = board.allDots[col - 1, row];
            GameObject rightDot = board.allDots[col + 1, row];
            if (letfDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag) {
                letfDot.GetComponent<Dot>().isMatched = true;
                rightDot.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }

        if (row > 0 && row < board._height - 1)
        {
            GameObject upDot = board.allDots[col, row + 1];
            GameObject bottomDot = board.allDots[col, row - 1];
            if (upDot.tag == this.gameObject.tag && bottomDot.tag == this.gameObject.tag)
            {
                upDot.GetComponent<Dot>().isMatched = true;
                bottomDot.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
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
            }

            otherDot = null;
        }
    }
}
