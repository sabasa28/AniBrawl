using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public enum GameState
    { 
        inMenus,
        playing,
    }
    GameplayController gc;

    public GameState gameState; // ver si esto es necesario
    void Start()
    {
        gameState = GameState.inMenus;
    }

    public void SetGameplayController(GameplayController newGc)
    {
        gc = newGc;
    }

    public void SetLevel(int lvlNum)
    {
        if (gc)
        {
            gc.SetLevel(lvlNum);
        }
    }

    public void StartGamePlay(int[] playerChar)
    {
        gameState = GameState.playing;
        gc.OnGameplaySceneStart(playerChar);
    }
}
