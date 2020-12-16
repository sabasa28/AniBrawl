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

    public GameState gameState;

    public void SetLevel(int lvlNum)
    {
        GetGc().SetLevel(lvlNum);
    }

    public void StartGamePlay(int[] playerChar)
    {
        gameState = GameState.playing;
        GetGc().OnGameplaySceneStart(playerChar);
    }
    GameplayController GetGc()
    {
        if (!gc) gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameplayController>();
        return gc;
    }
}
