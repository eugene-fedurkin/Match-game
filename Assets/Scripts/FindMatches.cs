using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board _board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < _board.width; i++)
        {
            for (int j = 0; j < _board.height; j++)
            {
                GameObject currentDot = _board.allDots[i, j];
                if (currentDot != null)
                {
                    if (i > 0 && i < _board.width - 1)
                    {
                        GameObject leftDot = _board.allDots[i - 1, j];
                        GameObject rightDot = _board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }

                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }


                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < _board.height - 1)
                    {
                        GameObject upDot = _board.allDots[i, j + 1];
                        GameObject downDot = _board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColumnBomb || downDot.GetComponent<Dot>().isColumnBomb || upDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(j - 1));
                                }

                                if (upDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(j + 1));
                                }



                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downDot))
                                {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }

    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < _board.width; i++)
        {
            for (int j = 0; j < _board.height; j++)
            {
                if (_board.allDots[i, j] != null)
                {
                    if (_board.allDots[i, j].tag == color)
                    {
                        _board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < _board.height; i++)
        {
            if (_board.allDots[column, i] != null)
            {
                dots.Add(_board.allDots[column, i]);
                _board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
                
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < _board.width; i++)
        {
            if (_board.allDots[i, row] != null)
            {
                dots.Add(_board.allDots[i, row]);
                _board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs()
    {
        if (_board.currentDot != null)
        {
            if (_board.currentDot.isMatched)
            {
                _board.currentDot.isMatched = false;

                /*
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    _board.currentDot.makeRowBomb();
                } else if (typeOfBomb >= 50)
                {
                    _board.currentDot.makeColumnBomb();
                }*/
                bool isHorizontalSwipe = (_board.currentDot.swipeAngle > -45 && _board.currentDot.swipeAngle <= 45) || (_board.currentDot.swipeAngle < -135 || _board.currentDot.swipeAngle >= 135);
                if (isHorizontalSwipe)
                {
                    _board.currentDot.makeRowBomb();
                } else
                {
                    _board.currentDot.makeColumnBomb();
                }


            } else if (_board.currentDot.otherDot != null)
            {
                Dot othetDot = _board.currentDot.otherDot.GetComponent<Dot>();

                if (othetDot.isMatched)
                {
                    othetDot.isMatched = false;

                    bool isHorizontalSwipe = (_board.currentDot.swipeAngle > -45 && _board.currentDot.swipeAngle <= 45) || (_board.currentDot.swipeAngle < -135 || _board.currentDot.swipeAngle >= 135);
                    if (isHorizontalSwipe)
                    {
                        othetDot.makeRowBomb();
                    }
                    else
                    {
                        othetDot.makeColumnBomb();
                    }
                }
            }
        }
    }
}
