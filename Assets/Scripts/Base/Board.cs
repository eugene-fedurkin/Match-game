using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind {
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal,
}

[System.Serializable]
public class MatchType {
    public int type;
    public string color;
}



[System.Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour {
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject[] _dots;
    [SerializeField] FindMatches _findMatches;

    private bool[,] blankSpaces;
    public GameObject[,] allDots;
    public TileType[] boardLayout;
    public BackgroundTile[,] breakableTiles;
    public GameObject breakableTilePrefab;
    public GameObject concreteTilePrefab;
    public BackgroundTile[,] concreteTiles;
    public GameObject lockTilePrefab;
    public BackgroundTile[,] lockTiles;

    public GameState currentState = GameState.move;

    public MatchType matchType;

    public int offset;
    public int height;
    public int width;
    public Dot currentDot;
    public int basePieceValue = 20;
    public int streakValue = 1;
    
    private ScoreManager scoreManager;
    public float refillDelay = 0.5f;
    private SoundManager soundManager;
    private GoalManger goalManger;
    
    public World world;
    public int level;

    void Awake() {
        if (PlayerPrefs.HasKey("Current Level")) {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if (world != null) {
            if (world.levels[level] != null) {
                width = world.levels[level].width;
                height = world.levels[level].height;
                _dots = world.levels[level].dots;
                // scoreG = world.levels[level].scoreGoals;
                boardLayout = world.levels[level].boardLayout;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        goalManger = FindObjectOfType<GoalManger>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        _findMatches = FindObjectOfType<FindMatches>();
        SetUp();

        currentState = GameState.pause;
    }

    public void GenerateBlankSpaces() {
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

    private void GenerateLockTiles() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Lock) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    public void GenerateConcreteTiles() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.Concrete) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    // Update is called once per frame
    void SetUp() {
        GenerateBlankSpaces();
        GenerateBreakbaleTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && !concreteTiles[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(_tilePrefab, tilePosition, Quaternion.identity);
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

    private MatchType ColumnOrRow() {
        List<GameObject> matchCopy = _findMatches.currentMatches as List<GameObject>;
        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matchCopy.Count; i++) {
            Dot dot = matchCopy[i].GetComponent<Dot>();

            string color = matchCopy[i].tag;
            int col = dot.col;
            int row = dot.row;
            int colMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++) {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();

                if (nextDot == dot) {
                    continue;
                }

                if (nextDot.col == dot.col && nextDot.tag == color) {
                    colMatch++;
                }

                if (nextDot.row == dot.row && nextDot.tag == color) {
                    rowMatch++;
                }
            }

            if (colMatch == 4 || rowMatch == 4) {
                matchType.type = 1;
                matchType.color = color;

                return matchType; // Color bomb
            } else if (colMatch == 2 && rowMatch == 2) {
                matchType.type = 2;
                matchType.color = color;

                return matchType;; // Adjacent bomb
            } else if (colMatch == 3 || rowMatch == 3) {
                matchType.type = 3;
                matchType.color = color;

                return matchType;; // Column or row bomb
            }
        }

        matchType.type = 0;
        matchType.color = "";

        return matchType;


        // int numberHorizontal = 0;
        // int numberVertical = 0;
        // Dot firstPiece = _findMatches.currentMatches[0].GetComponent<Dot>();

        // if (firstPiece != null) {
        //     foreach (GameObject currentPiece in _findMatches.currentMatches) {
        //         Dot dot = currentDot.GetComponent<Dot>();
        //         if (dot.row == firstPiece.row) {
        //             numberHorizontal++;
        //         }
        //         if (dot.col == firstPiece.col) {
        //             numberVertical++;
        //         }
        //     }
        // }

        // return numberVertical == 5 || numberHorizontal == 5;
    }

    private void CheckToMakeBombs() {
        if (_findMatches.currentMatches.Count > 3) {
            MatchType typeofMatch = ColumnOrRow();

            if (typeofMatch.type == 1) {
                if (currentDot != null) {
                    if (currentDot.isMatched && currentDot.tag == typeofMatch.color) {
                        currentDot.isMatched = false;
                        currentDot.makeColorBomb();
                    } else {
                        if (currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched && otherDot.tag == typeofMatch.color) {
                                otherDot.isMatched = false;
                                otherDot.makeColorBomb();
                            }
                        }
                    }
                }
            } else if (typeofMatch.type == 2) {
                if (currentDot != null) {
                    if (currentDot.isMatched && currentDot.tag == typeofMatch.color) {
                        currentDot.isMatched = false;
                        currentDot.makeAdjacentBomb();
                    } else {
                        if (currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched && otherDot.tag == typeofMatch.color) {
                                otherDot.isMatched = false;
                                otherDot.makeAdjacentBomb();
                            }
                        }
                    }
                }
            } else if (typeofMatch.type == 3) {
                _findMatches.CheckBombs(typeofMatch);
            }
        }
    }

    public void BombRow(int row) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (concreteTiles[i, j]) {
                    concreteTiles[i, row].TakeDamage(1);
                    if (concreteTiles[i, row].hitPoints <= 0) {
                        concreteTiles[i, row] = null;
                    }
                }
            }
        }
    }

    public void BombCol(int col) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (concreteTiles[i, j]) {
                    concreteTiles[col, i].TakeDamage(1);
                    if (concreteTiles[col, i].hitPoints <= 0) {
                        concreteTiles[col, i] = null;
                    }
                }
            }
        }
    }

    void DestroyMatchesAt(int col, int row) {
        if (allDots[col, row].GetComponent<Dot>().isMatched) {
            if (breakableTiles[col, row] != null) {
                breakableTiles[col, row].TakeDamage(1);
                if (breakableTiles[col, row].hitPoints <= 0) {
                    breakableTiles[col, row] = null;
                }
            }

            if (lockTiles[col, row] != null) {
                lockTiles[col, row].TakeDamage(1);
                if (lockTiles[col, row].hitPoints <= 0) {
                    lockTiles[col, row] = null;
                }
            }

            DamageConcrete(col, row);

            if (goalManger != null) {
                goalManger.CompareGoal(allDots[col, row].tag.ToString());
                goalManger.UpdateGoals();
            }

            if (soundManager != null) {
                soundManager.PlayDestroyNoise();
            }

            Destroy(allDots[col, row]);
            Debug.Log("col " + col + " row " + row);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[col, row] = null;
        }
    }

    public void DestroyMatches() {
        if (_findMatches.currentMatches.Count >= 4) {
            CheckToMakeBombs();
        }
        _findMatches.currentMatches.Clear();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo2());
    }

    void DamageConcrete(int col, int row) {
        if (col > 0) {
            if (concreteTiles[col - 1, row]) {
                concreteTiles[col - 1, row].TakeDamage(1);
                if (concreteTiles[col - 1, row].hitPoints <= 0) {
                    concreteTiles[col - 1, row] = null;
                }
            }
        }

        if (col < width - 1) {
            if (concreteTiles[col + 1, row]) {
                concreteTiles[col + 1, row].TakeDamage(1);
                if (concreteTiles[col + 1, row].hitPoints <= 0) {
                    concreteTiles[col + 1, row] = null;
                }
            }
        }

        if (row < 0) {
            if (concreteTiles[col, row - 1]) {
                concreteTiles[col, row - 1].TakeDamage(1);
                if (concreteTiles[col, row - 1].hitPoints <= 0) {
                    concreteTiles[col, row - 1] = null;
                }
            }
        }

        if (row < height - 1) {
            if (concreteTiles[col, row + 1]) {
                concreteTiles[col, row + 1].TakeDamage(1);
                if (concreteTiles[col, row + 1].hitPoints <= 0) {
                    concreteTiles[col, row + 1] = null;
                }
            }
        }
    }

    IEnumerator DecreaseRowCo2() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j]) {
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

        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    void RefillBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i,j]) {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, _dots.Length);
                    int maxIteration = 0;

                    while(MatchAt(i, j, _dots[dotToUse]) && maxIteration < 100) {
                        maxIteration++;
                        dotToUse = Random.Range(0, _dots.Length);
                    }
                    maxIteration = 0;

                    GameObject piece = Instantiate(_dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().col = i;
                }
            }
        }
    }

    bool MatchesOnBoard() {
        _findMatches.FindAllMatches();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched) {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator FillBoardCo() {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while(MatchesOnBoard()) {
            streakValue++;
            DestroyMatches();
            yield break;
            // yield return new WaitForSeconds(2 * refillDelay);
        }
        currentDot = null;
        yield return new WaitForSeconds(2 * refillDelay);

        if (isDeadlocked()) {
            ShuffleBoard();
        }
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction) {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null) {
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y];
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            allDots[column, row] = holder;
        }
    }

    private bool CheckForMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {
                if (allDots[i, j] != null) {

                    if (i < width - 2) {
                        //Check if the dots to the right and two to the right exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                               && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }

                    }
                    if (j < height - 2) {
                        //Check if the dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                               && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    // if (i < width - 2) {
                    //     if (i < width - 2 && allDots[i + 1, j] != null && allDots[i + 2, j] != null) {
                    //         if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag) {
                    //             return true;
                    //         }
                    //     }
                    // }
                    

                    // if (i < height - 2) {
                    //     if (j < height - 2 && allDots[i, j + 1] != null && allDots[i, j + 1] != null) {
                    //         if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag) {
                    //             return true;
                    //         }
                    //     }
                    // }
                }
            }
        }

        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction) {
        SwitchPieces(column, row, direction);
        if (CheckForMatches()) {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool isDeadlocked() {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    if (i < width - 1) {
                        if (SwitchAndCheck(i, j , Vector2.right)) {
                            return false;
                        }
                    }

                    if (j < height - 1) {
                        if (SwitchAndCheck(i, j, Vector2.up)) {
                            return false;
                        }
                    }

                }
            }
        }
        return true;
    }

    private void ShuffleBoard() {
        List<GameObject> newBoard = new List<GameObject>();
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

          for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if (!blankSpaces[i, j] && !concreteTiles[i, j]) {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIteration = 0;

                    while (MatchAt(i, j, newBoard[pieceToUse]) && maxIteration < 100) {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIteration++;
                    }

                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIteration = 0;
                    piece.col = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
          }

          if (isDeadlocked()) {
            ShuffleBoard();
          }
    }
}
