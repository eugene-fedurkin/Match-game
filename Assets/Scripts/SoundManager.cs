using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource destroyNoise;
    // Start is called before the first frame update
    
    public void PlayDestroyNoise() {
        destroyNoise.Play();
    }
}
