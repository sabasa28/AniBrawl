using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


//hacer singleton
public class UIGameplay : MonoBehaviour
{
    public Slider[] HPSlider;
    GameplayController gameplayController;
    PlayerController[] players;
    int[] playersHP = { 0, 0, 0, 0 }; //implementar esto despues asi no setea la vida todos los frames
    public GameObject endResultPanel;
    public TextMeshProUGUI winnerPlayerText;
    [SerializeField] GameObject pausePanel = null;
    [SerializeField] Sprite roundWonP1 = null;
    [SerializeField] Sprite roundWonP2 = null;
    [SerializeField] Image round1 = null;
    [SerializeField] Image round2 = null;
    [SerializeField] Image round3 = null;
    [SerializeField] GameObject introPanel = null;
    [SerializeField] TextMeshProUGUI[] introText = null;
    [SerializeField] GameObject[] uiPlayer = null;
    List<UIPlayerWS> uiPlayerWS = new List<UIPlayerWS>(); 

    void Start()
    {
        SetGameplayUIActiveState(false);
        gameplayController = FindObjectOfType<GameplayController>();
        pausePanel.SetActive(false);
    }

    public void UpdateHealthBars()
    {
        for (int i = 0; i < players.Length; i++)
        {
            HPSlider[players[i].playerNumber - 1].value = players[i].hp;
        }
    }

    public void SetPlayers(PlayerController[] players)
    {
        this.players = players;
        for (int i = 0; i < this.players.Length; i++)
        {
            if (HPSlider[this.players[i].playerNumber - 1])
            {
                HPSlider[this.players[i].playerNumber - 1].maxValue = this.players[i].hp;
                HPSlider[this.players[i].playerNumber - 1].value = this.players[i].hp;
            }
            uiPlayerWS.Add(players[i].GetComponentInChildren<UIPlayerWS>());
        }
    }


    public void SetWinner(int winnerPlayerNum)
    {
        endResultPanel.SetActive(true);
        winnerPlayerText.text = "Player " + winnerPlayerNum;
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
    void SetGameplayUIActiveState(bool activeState)
    {
        for (int i = 0; i < uiPlayer.Length; i++)
        {
            uiPlayer[i].SetActive(activeState);
        }
        round1.gameObject.SetActive(activeState);
        round2.gameObject.SetActive(activeState);
        round3.gameObject.SetActive(activeState);
    }

    public void DisplayIntro()
    {
        StartCoroutine(IntroAnim());
    }

    IEnumerator IntroAnim()
    {
        introPanel.SetActive(true);

        for (int i = 0; i < uiPlayerWS.Count; i++)
        {
            uiPlayerWS[i].Animate();
        }

        for (int i = 0; i < introText.Length; i++)
        {
            float t = 0.0f;
            Color visibleCol = new Color (introText[i].color.r, introText[i].color.g, introText[i].color.b, 1);
            Color notVisibleCol = new Color (introText[i].color.r, introText[i].color.g, introText[i].color.b, 0);
            while (t < 1)
            {
                t += Time.deltaTime;
                introText[i].color = Color.Lerp(notVisibleCol,visibleCol,t);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            t = 0.0f;
            while (t < 1)
            {
                t += Time.deltaTime *2;
                introText[i].color = Color.Lerp(visibleCol, notVisibleCol, t);
                yield return null;
            }
        }
        yield return null;
        introPanel.SetActive(false);
        SetGameplayUIActiveState(true);
        gameplayController.introDisplayed = true;
    }
}
