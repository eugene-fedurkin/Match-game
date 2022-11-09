using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject[] _dots;
    [SerializeField] MindMatches _findMatches;

    BackgroundTile[,] _allTiles;
    public GameObject[,] allDots;

    public GameState currentState = GameState.move;

    public int offset;
    public int height;
    public int width;


    // Start is called before the first frame update
    void Start()
    {
        _allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        _findMatches = FindObjectOfType<MindMatches>();
        SetUp();
    }

    // Update is called once per frame
    void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
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

                    Debug.Log(maxIteration);
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

    bool MatchAt(int col, int row, GameObject piece)
    {
        if (col > 1 && row > 1)
        {
            if (allDots[col - 1, row].tag == piece.tag && allDots[col - 2, row].tag == piece.tag) {
                return true;
            }

            if (allDots[col, row - 1].tag == piece.tag && allDots[col, row - 2].tag == piece.tag)
            {
                return true;
            }
        } else if (col <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[col, row - 1].tag == piece.tag && allDots[col, row - 2].tag == piece.tag) {
                    return true;
                }
            } else if (col > 1)
            {
                if (allDots[col - 1, row].tag == piece.tag && allDots[col - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DestroyMatchesAt(int col, int row)
    {
        if (allDots[col, row].GetComponent<Dot>().isMatched)
        {
            _findMatches.currentMatches.Remove(allDots[col, row]);
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

        StartCoroutine(DecrieseRowCo());
    }

    IEnumerator DecrieseRowCo()
    {
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                } else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
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
                if (allDots[i, j] == null)
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
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;

    }
}
