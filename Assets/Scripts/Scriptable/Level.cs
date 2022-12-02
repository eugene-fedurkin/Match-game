using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject {
    public int width;
    public int height;

    public TileType[] boardLayout;
    public GameObject[] dots;
    public int[] scoreGoals;

    public EndGameRequirements endGameRequirements;
    public BlankGoal[] LevelGoal;
}
