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
    [SerializeField] GameObject pausePanel;
    [SerializeField] Sprite roundWonP1;
    [SerializeField] Sprite roundWonP2;
    [SerializeField] Image round1;
    [SerializeField] Image round2;
    [SerializeField] Image round3;

    void Start()
    {
        gameplayController = FindObjectOfType<GameplayController>();
        player = FindObjectsOfType<PlayerController>();
        pausePanel.SetActive(false);
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

    public void SetRoundWinner(int winnerNumber, int round)
    {
        Sprite sprToUse;
        if (winnerNumber == 1)
            sprToUse = roundWonP1;
        else
            sprToUse = roundWonP2;
        switch (round)
        {
            case 1:
                round1.sprite = sprToUse;
                break;
            case 2:
                round2.sprite = sprToUse;
                break;
            case 3:
                round3.sprite = sprToUse;
                break;
        }
    }

    public void SetPauseActiveState(bool activeState)
    { 
        pausePanel.SetActive(activeState);
    }
}
