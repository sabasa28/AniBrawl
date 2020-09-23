using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject controls;
    [SerializeField] GameObject options;
    [SerializeField] GameObject charSelec;
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
}
