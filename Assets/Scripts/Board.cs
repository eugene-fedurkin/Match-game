using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public enum TileKind {
    Breakable,
    Blank,
    Normal,
}

[System.Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject[] _dots;
    [SerializeField] FindMatches _findMatches;

    private bool[,] blankSpaces;
    public GameObject[,] allDots;
    public TileType[] boardLayout;
    public BackgroundTile[,] breakableTiles;
    public GameObject breakableTilePrefab;

    public GameState currentState = GameState.move;

    public int offset;
    public int height;
    public int width;
    public Dot currentDot;


    // Start is called before the first frame update
    void Start()
    {
        breakableTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        _findMatches = FindObjectOfType<FindMatches>();
        SetUp();
    }

    public void GenerateSpaces() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Blank) {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakbaleTiles() {
         for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Breakable) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
         }
    }

    // Update is called once per frame
    void SetUp() {
        GenerateSpaces();
        GenerateBreakbaleTiles();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    GameObject backgroundTile = Instantiate(_tilePrefab, tempPosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + ", " + j + ")";
                    int dotToUse = Random.Range(0, _dots.Length);

                    int maxIteration = 0;
                    while (MatchAt(i, j, _dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, _dots.Length);
                        maxIteration++;
                    }
                    maxIteration = 0;

                    GameObject dot = Instantiate(_dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().col = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + ", " + j + ")";
                    allDots[i, j] = dot;
                }
                }
        }
    }

    bool MatchAt(int col, int row, GameObject piece) {
        if (col > 1 && row > 1) {
            if (allDots[col - 1, row] != null && allDots[col - 2, row] != null) {
                if (allDots[col - 1, row].tag == piece.tag && allDots[col - 2, row].tag == piece.tag) {
                    return true;
                }
            }

            if (allDots[col, row - 1] != null && allDots[col, row - 2] != null) {
                if (allDots[col, row - 1].tag == piece.tag && allDots[col, row - 2].tag == piece.tag) {
                    return true;
                }
            }
        } else if (col <= 1 || row <= 1) {
            if (row > 1) {
                if (allDots[col, row - 1] != null && allDots[col, row - 2] != null) {
                    if (allDots[col, row - 1].tag == piece.tag && allDots[col, row - 2].tag == piece.tag) {
                        return true;
                    }
                }
            } else if (col > 1) {
                if (allDots[col - 1, row] != null && allDots[col - 2, row] != null) {
                    if (allDots[col - 1, row].tag == piece.tag && allDots[col - 2, row].tag == piece.tag) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow() {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = _findMatches.currentMatches[0].GetComponent<Dot>();

        if (firstPiece != null) {
            foreach (GameObject currentPiece in _findMatches.currentMatches) {
                Dot dot = currentDot.GetComponent<Dot>();
                if (dot.row == firstPiece.row) {
                    numberHorizontal++;
                }
                if (dot.col == firstPiece.col) {
                    numberVertical++;
                }
            }
        }

        return numberVertical == 5 || numberHorizontal == 5;
    }

    private void CheckToMakeBombs() {
        if (_findMatches.currentMatches.Count == 4 || _findMatches.currentMatches.Count == 7) {
            _findMatches.CheckBombs();
        }

        if (_findMatches.currentMatches.Count == 5 || _findMatches.currentMatches.Count == 8) {
            if (ColumnOrRow()) {
                // make color bomb
                if (currentDot != null) {
                    if (currentDot.isMatched) {
                        if (!currentDot.isColorBomb) {
                            currentDot.isMatched = false;
                            currentDot.makeColorBomb();
                        }
                    } else {
                        if (currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched) {
                                if (!otherDot.isColorBomb) {
                                    otherDot.isMatched = false;
                                    otherDot.makeColorBomb();
                                }
                            }
                        }
                    }
                }
            } else {
                // make adjacent bomb
                if (currentDot != null) {
                    if (currentDot.isMatched) {
                        if (!currentDot.isAdjacentBomb) {
                            currentDot.isMatched = false;
                            currentDot.makeAdjacentBomb();
                        }
                    } else {
                        if (currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched) {
                                if (!otherDot.isAdjacentBomb) {
                                    otherDot.isMatched = false;
                                    otherDot.makeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void DestroyMatchesAt(int col, int row)
    {
        if (allDots[col, row].GetComponent<Dot>().isMatched)
        {
            if (_findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            if (breakableTiles[col, row] != null) {
                breakableTiles[col, row].TakeDamage(1);
                if (breakableTiles[col, row].hitPoints <= 0) {
                    breakableTiles[col, row] = null;
                }
            }

            Destroy(allDots[col, row]);
            allDots[col, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        _findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

    IEnumerator DecreaseRowCo2() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && allDots[i, j] == null) {
                    for (int k = j + 1; k < height; k++) {
                        if (allDots[i, k] != null) {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, _dots.Length);
                    GameObject piece = Instantiate(_dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().col = i;
                }
            }
        }
    }

    bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched)
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        _findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;

    }
}
