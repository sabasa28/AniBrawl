using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//hacer singleton
public class UIGameplay : MonoBehaviour
{
    public TextMeshProUGUI[] HPText;
    GameplayController gameplayController;
    PlayerController[] player;
    int[] playersHP = { 0, 0, 0, 0 }; //implementar esto despues asi no setea la vida todos los frames
    public GameObject endResultPanel;
    public TextMeshProUGUI winnerPlayerText;
    void Start()
    {
        gameplayController = FindObjectOfType<GameplayController>();
        player = FindObjectsOfType<PlayerController>();
        
        for (int i = 0; i < player.Length; i++)
        {
            HPText[player[i].playerNumber - 1].gameObject.SetActive(true);
            HPText[player[i].playerNumber - 1].text="P"+ (player[i].playerNumber - 1)+ " HP: " + player[i].hp;
        }
    }

    void Update()
    {
        for (int i = 0; i < player.Length; i++)
        {
            HPText[player[i].playerNumber - 1].text = "P"+ player[i].playerNumber + " HP: " + player[i].hp;
        }
    }
    public void SetWinner()
    {
        endResultPanel.SetActive(true);
        winnerPlayerText.text = "Player " + gameplayController.winnerPlayerNumber;
    }
}
