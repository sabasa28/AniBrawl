using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TesterTool : MonoBehaviour
{
    [SerializeField] PlayerController[] players = null;
    [SerializeField] TextMeshProUGUI forceText = null;
    [SerializeField] TextMeshProUGUI forceAddInDeathText = null;
    [SerializeField] TextMeshProUGUI MaxHPText = null;
    [SerializeField] TextMeshProUGUI currentHPP1Text = null;
    [SerializeField] TextMeshProUGUI currentHPP2Text = null;
    [SerializeField] GameplayController gpController = null;

    int p1 = -1;
    int p2 = -1;
    void Start()
    {
        StartCoroutine(GetPlayers());
    }

    private void Update()
    {
        if (p1 != -1)
            currentHPP1Text.text = players[p1].hp.ToString();
        if (p2 != -1)
            currentHPP2Text.text = players[p2].hp.ToString();
    }

    IEnumerator GetPlayers()
    {
        yield return new WaitForSeconds(1);
        players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerNumber == 1) p1 = i;
        }
        if (p1 == 0) p2 = 1;
        else p2 = 0;
        currentHPP1Text.text = players[p1].hp.ToString();
        currentHPP2Text.text = players[p2].hp.ToString();
        MaxHPText.text = "MaxHP " + players[p1].startingHp.ToString();
        forceText.text = "Force " + players[p1].force.ToString();
        forceAddInDeathText.text = "Force per Death " + gpController.forceWinByDeath.ToString();
    }

    public void AddForce()
    {
        ChangeForce(1);
    }
    public void RemoveForce()
    {
        ChangeForce(-1);
    }
    void ChangeForce(int change)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].force += change;
        }
        forceText.text = "Force " + players[p1].force.ToString();
    }

    public void AddForcePerDeath()
    {
        ChangeForcePerDeath(1);
    }
    public void RemoveForcePerDeath()
    {
        ChangeForcePerDeath(+-1);
    }
    void ChangeForcePerDeath(int change)
    {
        gpController.forceWinByDeath += change;
        forceAddInDeathText.text = "Force perDeath" + gpController.forceWinByDeath.ToString();
    }

    public void AddMaxHP()
    {
        ChangeMaxHP(5);
    }
    public void RemoveMaxHP()
    {
        ChangeMaxHP(-5);
    }

    void ChangeMaxHP(int change)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].startingHp = players[p1].startingHp + change;
            players[i].hp = players[i].startingHp;
            players[i].UpdateUI();
        }
        MaxHPText.text = "MaxHP "+ players[p1].startingHp.ToString();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
