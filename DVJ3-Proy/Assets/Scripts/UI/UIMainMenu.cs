using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject main = null;
    [SerializeField] GameObject controls = null;
    [SerializeField] GameObject options = null;
    [SerializeField] GameObject charSelec = null;
    [SerializeField] GameObject loadingScreen = null;
    [SerializeField] float minTimeToLoad = 0;
    [SerializeField] Image generalBackground = null;
    [SerializeField] GameObject[] characterSelectedP1 = null;
    [SerializeField] GameObject[] characterSelectedP2 = null;
    [SerializeField] TextMeshProUGUI versionText = null;
    int chosenCharP1;
    int chosenCharP2;

    private void Start()
    {
        versionText.text = "v" + Application.version;
        chosenCharP1 = 0;
        chosenCharP2 = 0;
        characterSelectedP1[chosenCharP1].SetActive(true);
        characterSelectedP2[chosenCharP2].SetActive(true);
        StartCoroutine(Preload(1));
    }
    public void StartGameplayScene()
    {
        StartCoroutine(UnloadMenu());
        GameManager.Get().StartGamePlay();
    }
    public void ShowControls()
    {
        main.SetActive(false);
        controls.SetActive(true);
    }
    public void HideControls()
    {
        controls.SetActive(false);
        main.SetActive(true);
    }
    public void ShowOptions()
    {
        main.SetActive(false);
        options.SetActive(true);
    }
    public void HideOptions()
    {
        options.SetActive(false);
        main.SetActive(true);
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
    public void PrevCharacterP1()
    {
        characterSelectedP1[chosenCharP1].SetActive(false);
        chosenCharP1--;
        if (chosenCharP1 < 0) chosenCharP1 = characterSelectedP1.Length - 1;
        characterSelectedP1[chosenCharP1].SetActive(true);
    }
    public void NextCharacterP1()
    {
        characterSelectedP1[chosenCharP1].SetActive(false);
        chosenCharP1++;
        if (chosenCharP1 > characterSelectedP1.Length-1) chosenCharP1 = 0;
        characterSelectedP1[chosenCharP1].SetActive(true);
    }
    public void PrevCharacterP2()
    {
        characterSelectedP2[chosenCharP2].SetActive(false);
        chosenCharP2--;
        if (chosenCharP2 < 0) chosenCharP2 = characterSelectedP2.Length - 1;
        characterSelectedP2[chosenCharP2].SetActive(true);
    }
    public void NextCharacterP2()
    {
        characterSelectedP2[chosenCharP2].SetActive(false);
        chosenCharP2++;
        if (chosenCharP2 > characterSelectedP2.Length - 1) chosenCharP2 = 0;
        characterSelectedP2[chosenCharP2].SetActive(true);
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
                    loadingScreen.SetActive(false);
                    ao.allowSceneActivation = true;

                }
                yield return null;
            }
        }
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
