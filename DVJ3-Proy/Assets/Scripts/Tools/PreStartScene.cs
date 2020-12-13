using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreStartScene : MonoBehaviour
{
    void Start()
    {
        AkSoundEngine.PostEvent("Inicio_game", gameObject);
        SceneManager.LoadScene(1);
    }
}
