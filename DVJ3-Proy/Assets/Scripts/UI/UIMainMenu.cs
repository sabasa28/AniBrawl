﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject main = null;
    [SerializeField] GameObject controls = null;
    [SerializeField] GameObject stageSelec = null;
    [SerializeField] GameObject charSelec = null;
    [SerializeField] GameObject creditsScreen = null;
    [SerializeField] GameObject creditsTxt = null;
    Vector3 minPosCredits = Vector3.zero;
    [SerializeField] Vector3 maxPosCredits = Vector3.zero;
    [SerializeField] float minTimeToLoad = 0;
    [SerializeField] Image generalBackground = null;
    [SerializeField] Image[] characterImage = null;
    [SerializeField] TextMeshProUGUI[] characterName = null;
    [SerializeField] TextMeshProUGUI versionText = null;
    int levelSelected = 1;
    Coroutine creditsCoroutine = null;

    public enum Character
    {
        frog,
        duck
    }

    [Serializable]
    public class CharacterDisplay
    {
        public Sprite displayableImg;
        public string name;
        public Character characterSelected;
    }
    public CharacterDisplay[] charactersToDisplay;
    int[] chosenChar = { 0, 0 };

    private void Start()
    {
        minPosCredits = creditsTxt.transform.localPosition;
        versionText.text = "v" + Application.version;
        UpdateCharacterDisplayed(0);
        UpdateCharacterDisplayed(1);
        LoadManager.Get().StartCoroutine(LoadManager.Get().Preload(2,transform));
    }
    public void StartGameplayScene()
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        charSelec.SetActive(false);
        LoadManager.Get().StartCoroutine(LoadManager.Get().UnloadMenu(generalBackground));
        GameManager.Get().StartGamePlay(chosenChar);
    }
    public void ShowControls()
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        AkSoundEngine.PostEvent("Inicia_controls", gameObject);
        main.SetActive(false);
        controls.SetActive(true);
    }
    public void HideControls()
    {
        AkSoundEngine.PostEvent("Exit_game", gameObject);
        controls.SetActive(false);
        main.SetActive(true);
    }
    public void ShowStageSelec()
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        main.SetActive(false);
        stageSelec.SetActive(true);
    }
    public void HideStageSelec()
    {
        AkSoundEngine.PostEvent("Exit_game", gameObject);
        stageSelec.SetActive(false);
        main.SetActive(true);
    }
    public void ChangeStage(int stageNum)
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        if (levelSelected == stageNum) return;
        levelSelected = stageNum;
        GameManager.Get().SetLevel(levelSelected);
    }
    public void ShowCharSelec()
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        main.SetActive(false);
        charSelec.SetActive(true);
    }
    public void HideCharSelec()
    {
        AkSoundEngine.PostEvent("Exit_game", gameObject);
        charSelec.SetActive(false);
        main.SetActive(true);
    }
    public void ShowCredits()
    {
        AkSoundEngine.PostEvent("Click_ui", gameObject);
        AkSoundEngine.PostEvent("Inicia_creditos", gameObject);
        main.SetActive(false);
        creditsScreen.SetActive(true);
        creditsCoroutine = StartCoroutine(ScrollCredits());
    }
    public void HideCredits()
    {
        AkSoundEngine.PostEvent("Exit_game", gameObject);
        StopCoroutine(creditsCoroutine);
        creditsScreen.SetActive(false);
        main.SetActive(true);
    }
    public void CloseGame()
    {
        AkSoundEngine.PostEvent("Exit_game", gameObject);
        Application.Quit();
    }

    public void NextCharacter(int playerNum)
    {
        AkSoundEngine.PostEvent("Player_select", gameObject);
        ChangeCharacter(playerNum, 1);
    }
    public void PrevCharacter(int playerNum)
    {
        AkSoundEngine.PostEvent("Player_select", gameObject);
        ChangeCharacter(playerNum, -1);
    }
    void ChangeCharacter(int playerNum, int toAdd)
    {
        chosenChar[playerNum] += toAdd;
        if (chosenChar[playerNum] >= charactersToDisplay.Length)
        {
            chosenChar[playerNum] = 0;
        }
        else if (chosenChar[playerNum] < 0)
        {
            chosenChar[playerNum] = charactersToDisplay.Length - 1;
        }
        UpdateCharacterDisplayed(playerNum);
    }

    void UpdateCharacterDisplayed(int playerNum)
    {
        characterImage[playerNum].sprite = charactersToDisplay[chosenChar[playerNum]].displayableImg;
        characterName[playerNum].text = charactersToDisplay[chosenChar[playerNum]].name;
    }

    public void OnHoverButton()
    { 
        AkSoundEngine.PostEvent("Hover_ui", gameObject);
    }

    IEnumerator ScrollCredits()
    {
        float t = 0.0f;
        float timeScrolling = 30f;
        while (true)
        {
            creditsTxt.transform.localPosition = minPosCredits;
            t = 0.0f;
            while (t < 1)
            {
                t += Time.deltaTime / timeScrolling;
                creditsTxt.transform.localPosition = Vector3.Lerp(minPosCredits, maxPosCredits, t);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
