using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject controls;
    [SerializeField] GameObject options;
    [SerializeField] GameObject charSelec;
    [SerializeField] GameObject[] characterSelectedP1;
    [SerializeField] GameObject[] characterSelectedP2;
    int chosenCharP1;
    int chosenCharP2;

    private void Start()
    {
        chosenCharP1 = 0;
        chosenCharP2 = 0;
        characterSelectedP1[chosenCharP1].SetActive(true);
        characterSelectedP2[chosenCharP2].SetActive(true);
    }
    public void StartGameplayScene()
    {
        SceneManager.LoadScene(1);
    }
    public void ShowControls()
    {
        controls.SetActive(true);
    }
    public void HideControls()
    {
        controls.SetActive(false);
    }
    public void ShowOptions()
    {
        options.SetActive(true);
    }
    public void HideOptions()
    {
        options.SetActive(false);
    }
    public void ShowCharSelec()
    {
        charSelec.SetActive(true);
    }
    public void HideCharSelec()
    {
        charSelec.SetActive(false);
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
}
