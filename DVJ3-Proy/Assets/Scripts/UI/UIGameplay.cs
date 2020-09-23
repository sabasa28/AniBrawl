using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//hacer singleton
public class UIGameplay : MonoBehaviour
{
    public Slider[] HPSlider;
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
            HPSlider[player[i].playerNumber - 1].maxValue = player[i].hp;
            HPSlider[player[i].playerNumber - 1].value = player[i].hp;
        }
    }

    void Update()
    {
        for (int i = 0; i < player.Length; i++)
        {
            HPSlider[player[i].playerNumber - 1].value = player[i].hp;
        }
    }

    public void SetWinner()
    {
        endResultPanel.SetActive(true);
        winnerPlayerText.text = "Player " + gameplayController.winnerPlayerNumber;
    }
}
