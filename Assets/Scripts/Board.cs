using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject[] _dots;

    BackgroundTile[,] _allTiles;
    public GameObject[,] allDots;

    public int _height;
    public int _width;


    // Start is called before the first frame update
    void Start()
    {
        _allTiles = new BackgroundTile[_width, _height];
        allDots = new GameObject[_width, _height];
        SetUp();
    }

    // Update is called once per frame
    void SetUp()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject tile = Instantiate(_tilePrefab, tempPosition, Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = "(" + i + ", " + j + ")";
                int dotToUse = Random.Range(0, _dots.Length);
                GameObject dot = Instantiate(_dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + ", " + j + ")";
                allDots[i, j] = dot;
            }

        }
    }
}
