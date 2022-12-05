using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour {
    public int hitPoints;
    SpriteRenderer sprite;
    GoalManger goalManger;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        goalManger = GetComponent<GoalManger>();
    }

    // Update is called once per frame
    void Update() {
        if (hitPoints <= 0) {
            if (goalManger != null) {
                goalManger.CompareGoal(gameObject.tag);
                goalManger.UpdateGoals();
            }
            Destroy(this.gameObject);
        }
        
    }

    public void TakeDamage(int damage) {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter() {
        Color color = sprite.color;
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, newAlpha);
    }
}
