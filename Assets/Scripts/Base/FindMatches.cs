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

    private List<GameObject> isAdjacentBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot1.col, dot1.row));
        }

        if (dot2.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot2.col, dot2.row));
        }

        if (dot3.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot3.col, dot3.row));
        }

        return currentDots;
    }


    private List<GameObject> isRowBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb) {
            currentMatches.Union(GetRowPieces(dot1.row));
            _board.BombRow(dot1.row);
        }

        if (dot2.isRowBomb) {
            currentMatches.Union(GetRowPieces(dot2.row));
            _board.BombRow(dot2.row);
        }

        if (dot3.isRowBomb) {
            currentMatches.Union(GetRowPieces(dot3.row));
            _board.BombRow(dot3.row);
        }

        return currentDots;
    }

    private List<GameObject> isColBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb) {
            currentMatches.Union(GetColumnPieces(dot1.col));
            _board.BombRow(dot1.col);

        }

        if (dot2.isColumnBomb) {
            currentMatches.Union(GetColumnPieces(dot2.col));
            _board.BombRow(dot2.col);

        }

        if (dot3.isColumnBomb) {
            currentMatches.Union(GetColumnPieces(dot3.col));
            _board.BombRow(dot3.col);

        }

        return currentDots;
    }

private void AddToListAndMatch(GameObject dot) {
    if (!currentMatches.Contains(dot))
    {
        currentMatches.Add(dot);
    }
    dot.GetComponent<Dot>().isMatched = true;
}

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo() {
        // yield return new WaitForSeconds(.2f);
        yield return null; // wait to next frame
        for(int i = 0; i < _board.width; i++) {
            for (int j = 0; j < _board.height; j++) {
                GameObject currentDot = _board.allDots[i, j];
                if (currentDot != null) {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < _board.width - 1) {
                        GameObject leftDot = _board.allDots[i - 1, j];
                        GameObject rightDot = _board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null) {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {
                                currentMatches.Union(isRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(isColBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(isAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));
                                
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < _board.height - 1) {
                        GameObject upDot = _board.allDots[i, j + 1];
                        GameObject downDot = _board.allDots[i, j - 1];

                        if (upDot != null && downDot != null) {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) {
                                currentMatches.Union(isColBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(isRowBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(isAdjacentBomb(upDotDot, currentDotDot, downDotDot));
                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }

    }

    public void MatchPiecesOfColor(string color) {
        for (int i = 0; i < _board.width; i++) {
            for (int j = 0; j < _board.height; j++) {
                if (_board.allDots[i, j] != null) {
                    if (_board.allDots[i, j].tag == color) {
                        _board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

     List<GameObject> GetAdjacentPieces(int column, int row) {
        List<GameObject> dots = new List<GameObject>();

        for (int i = column - 1; i <= column + 1; i++) {
            for (int j = row - 1; j <= row + 1; j++) {
                if (i >= 0 && i < _board.width && j >= 0 && j <_board.height) {
                    if (_board.allDots[i, j] != null) {
                        dots.Add(_board.allDots[i, j]);
                        _board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }

        return dots;
     }

    List<GameObject> GetColumnPieces(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < _board.height; i++) {
            if (_board.allDots[column, i] != null) {
                Dot dot = _board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb) {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(_board.allDots[column, i]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < _board.width; i++) {
            if (_board.allDots[i, row] != null) {
                Dot dot = _board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb) {
                    dots.Union(GetColumnPieces(i)).ToList();
                }

                dots.Add(_board.allDots[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs(MatchType matchType) {
        if (_board.currentDot != null) {
            if (_board.currentDot.isMatched && _board.currentDot.tag == matchType.color) {
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
                if (isHorizontalSwipe) {
                    _board.currentDot.makeRowBomb();
                } else {
                    _board.currentDot.makeColumnBomb();
                }
            } else if (_board.currentDot.otherDot != null) {
                Dot othetDot = _board.currentDot.otherDot.GetComponent<Dot>();

                if (othetDot.isMatched && _board.currentDot.tag == matchType.color) {
                    othetDot.isMatched = false;

                    bool isHorizontalSwipe = (_board.currentDot.swipeAngle > -45 && _board.currentDot.swipeAngle <= 45) || (_board.currentDot.swipeAngle < -135 || _board.currentDot.swipeAngle >= 135);
                    if (isHorizontalSwipe) {
                        othetDot.makeRowBomb();
                    } else {
                        othetDot.makeColumnBomb();
                    }
                }
            }
        }
    }
}
