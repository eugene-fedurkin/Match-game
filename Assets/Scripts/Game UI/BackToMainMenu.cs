using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour {
    public string sceneToLoad;

    public void OK() {
        SceneManager.LoadScene(sceneToLoad);
    }
}
