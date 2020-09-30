using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//hacer SINGLETON
public class GameplayController : MonoBehaviour
{
    UIGameplay uiGameplay;
    [SerializeField]
    List<PlayerController> players = new List<PlayerController>();
    public int winnerPlayerNumber = 0;
    [SerializeField] int forceWinByDeath;
    int currentRound = 1;
    int maxRounds = 3;
    int player1wins = 0;
    int player2wins = 0;
    void Start()
    {
        Time.timeScale = 1.0f;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }
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
        StartCoroutine(NewRound(player));
    }

    IEnumerator NewRound(PlayerController player)
    {
        yield return new WaitForSeconds(0.5f);
        int winnerPlayer;
        if (player.playerNumber == 1)
        {
            winnerPlayer = 2;
            player2wins++;
        }
        else
        {
            winnerPlayer = 1;
            player1wins++;
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ResetPlayer();
        }
        player.force += forceWinByDeath;
        uiGameplay.SetRoundWinner(winnerPlayer, currentRound);
        currentRound++;
        if (currentRound > maxRounds || player1wins > 1 || player2wins > 1)
        {
            OnGameOver();
        }
    }
    
    void OnGameOver()
    {
        Time.timeScale = 0.0f;
        if (player2wins > player1wins)
            winnerPlayerNumber = 2;
        else
            winnerPlayerNumber = 1;
        uiGameplay.SetWinner();
        StartCoroutine(EndscreenInput());
    }

    IEnumerator EndscreenInput()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(0);
            }
            yield return null;
        }
    }
}
