using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//hacer SINGLETON
public class GameplayController : MonoBehaviour
{
    UIGameplay uiGameplay;
    public int winnerPlayerNumber = 0;
    [SerializeField] List<PlayerController> players = new List<PlayerController>();
    public int forceWinByDeath = 0;
    [SerializeField] Vector3 p1StartPos = Vector3.zero;
    [SerializeField] Vector3 p2StartPos = Vector3.zero;
    [SerializeField] GrabbingZone[] grabbingZones = null;
    [SerializeField] CameraController cameraController = null;
    [SerializeField] PostProcessManager ppManager = null;
    public bool introDisplayed = false;
    [SerializeField] TesterTool tester;

    [Serializable]
    public struct PlayerVars
    {
        public float force;
        public float speed;
        public float rotSpeed;
        public int HP;
    }
    public PlayerVars commonPlayerVars;


    int currentRound = 1;
    int maxRounds = 3;
    int player1wins = 0;
    int player2wins = 0;
    bool paused = false;

    [SerializeField] PlayerController[] models = null;

    void Start()
    {
        Time.timeScale = 1.0f;
        uiGameplay = FindObjectOfType<UIGameplay>();
        GameManager.Get().SetGameplayController(this);

        if (SceneManager.sceneCount < 2)
            OnGameplaySceneStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                paused = false;
                Time.timeScale = 1;
                uiGameplay.SetPauseActiveState(paused);
            }
            else
            {
                paused = true;
                Time.timeScale = 0;
                uiGameplay.SetPauseActiveState(paused);
            }
        }
#if UNITY_EDITOR
        float speed1 = 1.0f;
        float speed2 = 0.3f;
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Time.timeScale == speed1)
            {
                Time.timeScale = speed2;
            }
            else
                Time.timeScale = speed1;
        }
#endif
    }

    void OnPlayerDead(PlayerController player)
    {
        StartCoroutine(NewRound(player));
    }

    IEnumerator NewRound(PlayerController player)
    {
        yield return new WaitForSeconds(0.5f);
        int winnerPlayer;
        if (player.playerNumber == 1)
        {
            winnerPlayer = 2;
            player2wins++;
        }
        else
        {
            winnerPlayer = 1;
            player1wins++;
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ResetPlayer();
        }
        player.force += forceWinByDeath;
        uiGameplay.SetRoundWinner(winnerPlayer, currentRound);
        currentRound++;
        if (currentRound > maxRounds || player1wins > 1 || player2wins > 1)
        {
            OnGameOver();
        }
    }
    
    void OnGameOver()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ableToMove = false;
        }
        if (player2wins > player1wins)
            winnerPlayerNumber = 2;
        else
            winnerPlayerNumber = 1;
        uiGameplay.SetWinner(winnerPlayerNumber);
        StartCoroutine(EndscreenInput());
    }

    IEnumerator EndscreenInput()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(0);
            }
            yield return null;
        }
    }
    public void OnGameplaySceneStart()//pasar por parametro que modelo tiene que usar cada uno
    {
        SetGameplay();
        StartCoroutine(DisplayIntroAndPlay());
    }
    void SetGameplay()
    {
        ppManager.StartRemovingCAberration();
        PlayerController P1 = Instantiate(models[0], p1StartPos, Quaternion.identity); // se puede hacer un for
        P1.playerNumber = 1;
        grabbingZones[0].player = P1;
        players.Add(P1);
        PlayerController P2 = Instantiate(models[1], p2StartPos, Quaternion.identity);
        P2.playerNumber = 2;
        grabbingZones[1].player = P2;
        players.Add(P2);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].force = commonPlayerVars.force;
            players[i].speed = commonPlayerVars.speed;
            players[i].rotSpeed = commonPlayerVars.rotSpeed;
            players[i].hp = commonPlayerVars.HP;
            players[i].OnDeath = OnPlayerDead;
            players[i].ableToMove = false;
            players[i].UpdateUI = uiGameplay.UpdateHealthBars;
        }
        uiGameplay.SetPlayers(players.ToArray());
        cameraController.OnGameplayStart(players);
        //tester.gameObject.SetActive(true);
    }

    IEnumerator DisplayIntroAndPlay()
    {
        uiGameplay.DisplayIntro();
        yield return new WaitUntil(()=>introDisplayed);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ableToMove = true;
        }
    }
}
