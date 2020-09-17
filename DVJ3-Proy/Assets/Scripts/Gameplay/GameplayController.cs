using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hacer SINGLETON
public class GameplayController : MonoBehaviour
{
    UIGameplay uiGameplay;
    [SerializeField]
    List<Player> players = new List<Player>();
    public int winnerPlayerNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        uiGameplay = FindObjectOfType<UIGameplay>();
        Player[] allPlayers = FindObjectsOfType<Player>(); ;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            players.Add(allPlayers[i]);
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].OnDeath = OnPlayerDeath;
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
    void OnPlayerDeath(Player player)
    {
        players.Remove(player);
        if (players.Count <= 1)
        {
            winnerPlayerNumber = players[0].playerNumber;
            uiGameplay.SetWinner();
        } 
    }
}
