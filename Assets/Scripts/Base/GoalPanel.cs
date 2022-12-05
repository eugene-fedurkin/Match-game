using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalPanel : MonoBehaviour {
    public Image currImage;
    public Sprite currSprite;
    public Color currColor;
    public TextMeshProUGUI currText;
    public string currString;

    void Start()
    {
        Setup();
    }

    void Setup() {
        currImage.color = currColor;
        // currImage.sprite = currSprite; // use sprite after has drawn it
        currText.text = currString;
    }
}
