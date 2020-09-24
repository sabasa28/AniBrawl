using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hacer SINGLETON
public class GameplayController : MonoBehaviour
{
    UIGameplay uiGameplay;
    [SerializeField]
    List<PlayerController> players = new List<PlayerController>();
    public int winnerPlayerNumber = 0;
    int currentRound = 1;
    // Start is called before the first frame update
    void Start()
    {
        uiGameplay = FindObjectOfType<UIGameplay>();
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>(); ;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            players.Add(allPlayers[i]);
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].OnDeath = OnPlayerDead;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        float speed1 = 1.0f;
        float speed2 = 0.3f;
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Time.timeScale == speed1)
            {
                Time.timeScale = speed2;
            }
            else
                Time.timeScale = speed1;
        }
#endif
    }

    void OnPlayerDead(PlayerController player)
    {
        NewRound(player);
    }

    void NewRound(PlayerController player)//usar este player para darle mas fuerza
    {
        int winnerPlayer;
        if (player.playerNumber == 1)
            winnerPlayer = 2;
        else
            winnerPlayer = 1;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ResetPlayer();
        }
        uiGameplay.SetRoundWinner(winnerPlayer,currentRound);
        currentRound++;
    }
    void OnGameOver(int winner)
    {
         
    }
}
