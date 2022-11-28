using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator paneAnimation;
    public Animator gameInfoAnimation;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ok() {
        if (paneAnimation && gameInfoAnimation) {
            paneAnimation.SetBool("Out", true);
            gameInfoAnimation.SetBool("Out", true);
        }
    }
}
