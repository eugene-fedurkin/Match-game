using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal {
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public Color currColor; // tempSolution
    public string matchDotTagValue;
}

public class GoalManger : MonoBehaviour {
    public BlankGoal[] levelGoal;
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public EndGameManager endGameManager;

    // Start is called before the first frame update
    void Start() {
        SetupIntroGoal();
        endGameManager = FindObjectOfType<EndGameManager>();
    }

    // Update is called once per frame
    public void UpdateGoals() {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoal.Length; i++) {
            currentGoals[i].currText.text = "" + levelGoal[i].numberCollected + "/" + levelGoal[i].numberNeeded;
            if (levelGoal[i].numberCollected >= levelGoal[i].numberNeeded) {
                goalsCompleted++;
                currentGoals[i].currText.text = "" + levelGoal[i].numberNeeded + "/" + levelGoal[i].numberNeeded;
            }
        }

        if (goalsCompleted >= levelGoal.Length) {
            if (endGameManager != null) {
                endGameManager.WinGame();
            }
        }
    }

    public void CompareGoal(string goalToCompare) {
        for (int i = 0; i < levelGoal.Length; i++) {
            if (goalToCompare == levelGoal[i].matchDotTagValue) {
                levelGoal[i].numberCollected++;
            }
        }
    }

    void SetupIntroGoal() {
        for (int i = 0; i < levelGoal.Length; i++) {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.currSprite = levelGoal[i].goalSprite;
            panel.currColor = levelGoal[i].currColor;
            panel.currString = "0/" + levelGoal[i].numberNeeded;

            GameObject goalIntro = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            goalIntro.transform.SetParent(goalGameParent.transform);

            panel = goalIntro.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.currSprite = levelGoal[i].goalSprite;
            panel.currColor = levelGoal[i].currColor;
            panel.currString = "0/" + levelGoal[i].numberNeeded;
        }
    }
}
