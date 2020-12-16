using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//hacer singleton
public class UIGameplay : MonoBehaviour
{
    public Image[] HPBar;
    GameplayController gameplayController;
    PlayerController[] players;
    int[] playersHP = { 0, 0, 0, 0 }; //implementar esto despues asi no setea la vida todos los frames
    public GameObject endResultPanel;
    public TextMeshProUGUI winnerPlayerText;
    [SerializeField] GameObject pausePanel = null;
    [SerializeField] GameObject[] P1Points = null;
    [SerializeField] GameObject[] P2Points = null;
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
            HPBar[players[i].playerNumber - 1].fillAmount = (float)players[i].hp / players[i].startingHp;
        }
    }

    public void SetPlayers(PlayerController[] playersToSet)
    {
        this.players = playersToSet;
        for (int i = 0; i < players.Length; i++)
        {
            if (HPBar[players[i].playerNumber - 1])
            {
                HPBar[players[i].playerNumber - 1].fillAmount = players[i].hp / players[i].startingHp;
            }
            uiPlayerWS.Add(playersToSet[i].GetComponentInChildren<UIPlayerWS>());
        }
    }


    public void SetWinner(int winnerPlayerNum)
    {
        endResultPanel.SetActive(true);
        winnerPlayerText.text = "Player " + winnerPlayerNum;
    }

    public void SetRoundWinner(int winnerNumber)
    {
        if (winnerNumber == 1)
            AddRoundWinIndicator(P1Points);
        else
            AddRoundWinIndicator(P2Points);
    }

    void AddRoundWinIndicator(GameObject[] rounds)
    {
        for (int i = 0; i < rounds.Length; i++)
        {
            if (!rounds[i].activeInHierarchy)
            {
                rounds[i].SetActive(true);
                break;
            }
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
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1.0f;
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        LoadManager.Get().LoadMenu();
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
            float timeInText = 0.5f;
            float t = 0.0f;
            Color visibleCol = new Color (introText[i].color.r, introText[i].color.g, introText[i].color.b, 1);
            Color notVisibleCol = new Color (introText[i].color.r, introText[i].color.g, introText[i].color.b, 0);
            while (t < 1)
            {
                t += Time.deltaTime / timeInText;
                introText[i].color = Color.Lerp(notVisibleCol,visibleCol,t);
                yield return null;
            }
            t = 0.0f;
            while (t < 1)
            {
                t += Time.deltaTime / timeInText;
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
