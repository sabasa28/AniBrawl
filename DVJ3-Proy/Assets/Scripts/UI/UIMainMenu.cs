using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject main = null;
    [SerializeField] GameObject controls = null;
    [SerializeField] GameObject stageSelec = null;
    [SerializeField] GameObject charSelec = null;
    [SerializeField] GameObject loadingScreen = null;
    [SerializeField] float minTimeToLoad = 0;
    [SerializeField] Image generalBackground = null;
    [SerializeField] Image[] characterImage = null;
    [SerializeField] TextMeshProUGUI[] characterName = null;
    [SerializeField] TextMeshProUGUI versionText = null;
    int levelSelected = -1;

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
    int[] chosenChar = {0 , 0};

    private void Start()
    {
        versionText.text = "v" + Application.version;
        UpdateCharacterDisplayed(0);
        UpdateCharacterDisplayed(1);
        StartCoroutine(Preload(1));
    }
    public void StartGameplayScene()
    {
        StartCoroutine(UnloadMenu());
        GameManager.Get().StartGamePlay(chosenChar);
    }
    public void ShowControls()
    {
        //AkSoundEngine.PostEvent("Click_ui", gameObject);
        main.SetActive(false);
        controls.SetActive(true);
    }
    public void HideControls()
    {
        controls.SetActive(false);
        main.SetActive(true);
    }
    public void ShowStageSelec()
    {
        main.SetActive(false);
        stageSelec.SetActive(true);
    }
    public void HideStageSelec()
    {
        stageSelec.SetActive(false);
        main.SetActive(true);
    }
    public void ChangeStage(int stageNum)
    {
        if (levelSelected == stageNum) return;
        levelSelected = stageNum;
        GameManager.Get().SetLevel(levelSelected);
    }
    public void ShowCharSelec()
    {
        main.SetActive(false);
        charSelec.SetActive(true);
    }
    public void HideCharSelec()
    {
        charSelec.SetActive(false);
        main.SetActive(true);
    }
    public void CloseGame()
    {
        Application.Quit();
    }

    public void NextCharacter(int playerNum)
    {
        ChangeCharacter(playerNum, 1);
    }
    public void PrevCharacter(int playerNum)
    {
        ChangeCharacter(playerNum, -1);
    }
    void ChangeCharacter(int playerNum, int toAdd)
    {
        chosenChar[playerNum]+= toAdd;
        if (chosenChar[playerNum] >= charactersToDisplay.Length)
        {
            chosenChar[playerNum] = 0;
        }
        UpdateCharacterDisplayed(playerNum);
    }

    void UpdateCharacterDisplayed(int playerNum)
    {
        if (chosenChar.Length > playerNum)
        {
            characterImage[playerNum].sprite = charactersToDisplay[chosenChar[playerNum]].displayableImg;
            characterName[playerNum].text = charactersToDisplay[chosenChar[playerNum]].name;
        }
    }

    IEnumerator Preload(int scene)
    {
        float loadingProgress;
        float timeLoading = 0;
        yield return null;
        if (!SceneManager.GetSceneByBuildIndex(1).isLoaded)
        { 
            AsyncOperation ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            ao.allowSceneActivation = false;
            
            while (!ao.isDone)
            {
                timeLoading += Time.deltaTime;
                loadingProgress = ao.progress + 0.1f;
                loadingProgress = loadingProgress * timeLoading / minTimeToLoad;

                // Loading completed
                if (loadingProgress >= 1)
                {
                    ao.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        loadingScreen.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }

    IEnumerator UnloadMenu()
    {
        charSelec.SetActive(false);
        float t = 0;
        float fadeInTime = 3.0f;
        Color backgroundVisible = generalBackground.color;
        Color backgroundNotVisible = new Color(generalBackground.color.r, generalBackground.color.g, generalBackground.color.b, 0);
        while (t < 1)
        {
            t += Time.deltaTime / fadeInTime;
            generalBackground.color = Color.Lerp(backgroundVisible, backgroundNotVisible, t);
            yield return null;
        }
        AsyncOperation ao = SceneManager.UnloadSceneAsync(0);
        ao.allowSceneActivation = false;
        while (!ao.isDone)
        {
            yield return null;
        }
        ao.allowSceneActivation = true;
    }
}
