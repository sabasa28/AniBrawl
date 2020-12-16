using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif
//hacer SINGLETON
public class GameplayController : MonoBehaviour
{
    UIGameplay uiGameplay;
    public int winnerPlayerNumber = 0;
    [SerializeField] List<PlayerController> players = new List<PlayerController>();
    public int forceWinByDeath = 0;
    
    [SerializeField] GrabbingZone[] grabbingZones = null;
    [SerializeField] CameraController cameraController = null;
    [SerializeField] PostProcessManager ppManager = null;
    public bool introDisplayed = false;
    [SerializeField] GameObject[] level = null;
    int activeLevel = 1;

    [Header("General Stage")]
    [SerializeField] Transform[] playerSpawner = null;
    [SerializeField] FallingObstController fallingOC = null;
    [SerializeField] FireController fireC = null;

    [Header("Stage1")]
    [SerializeField] GameObject staticObstaclePrefStg1 = null;
    [SerializeField] Transform[] staticObstSpawnStg1 = null;
    [SerializeField] GameObject fallingObstaclePrefStg1 = null;

    [Header("Stage2")]
    [SerializeField] GameObject staticObstaclePrefStg2 = null;
    [SerializeField] Transform[] staticObstSpawnStg2 = null;
    [SerializeField] GameObject fallingObstaclePrefStg2 = null;

    Coroutine winRoundCor = null;

    [Serializable]
    public struct PlayerVars
    {
        public float force;
        public float speed;
        public float rotSpeed;
        public int HP;
    }
    [Space]
    public PlayerVars commonPlayerVars;

    int maxRounds = 5;
    int player1wins = 0;
    int player2wins = 0;
    bool paused = false;

    [SerializeField] PlayerController playerPrefab = null;

    void Start()
    {
        Time.timeScale = 1.0f;
        uiGameplay = FindObjectOfType<UIGameplay>();
#if UNITY_EDITOR
        if (SceneManager.sceneCount < 2)
            OnGameplaySceneStart(new int[] {0, 0});
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].ableToMove = true;
                }
                Time.timeScale = 1;
                uiGameplay.SetPauseActiveState(paused);
            }
            else
            {
                paused = true;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].ableToMove = false;
                }
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
        if (winRoundCor == null) winRoundCor = StartCoroutine(NewRound(player));
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
        uiGameplay.SetRoundWinner(winnerPlayer);
        if (player1wins > maxRounds/2 || player2wins > maxRounds / 2)
        {
            OnGameOver();
        }
        winRoundCor = null;
    }
    
    void OnGameOver()
    {
        fallingOC.StopSpawning();
        fireC.StopSpawning();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ableToMove = false;
        }
        if (player2wins > player1wins)
            winnerPlayerNumber = 2;
        else
            winnerPlayerNumber = 1;
        uiGameplay.SetWinner(winnerPlayerNumber);
        AkSoundEngine.PostEvent("End_fight", gameObject);
    }

    public void OnGameplaySceneStart(int[] playerCharacter)
    {
        SetGameplay(playerCharacter);
        StartCoroutine(DisplayIntroAndPlay());
    }
    void SetGameplay(int[] playerCharacter)
    {
        switch (activeLevel)
        {
            case 0:
                AkSoundEngine.PostEvent("Inicia_bosque", gameObject);
                SpawnStaticObstacles(staticObstaclePrefStg1,staticObstSpawnStg1);
                fallingOC.StartSpawning(fallingObstaclePrefStg1);
                break;
            case 1:
            default:
                AkSoundEngine.PostEvent("Inicia_granja", gameObject);
                SpawnStaticObstacles(staticObstaclePrefStg2,staticObstSpawnStg2);
                fallingOC.StartSpawning(fallingObstaclePrefStg2);
                break;
        }
        fireC.StartSpawning();
        ppManager.StartRemovingCAberration();
        PlayerController P1 = Instantiate(playerPrefab, playerSpawner[0].position, playerSpawner[0].rotation);
        P1.playerNumber = 1;
        P1.modelIndex = playerCharacter[0];
        grabbingZones[0].player = P1;
        players.Add(P1);
        PlayerController P2 = Instantiate(playerPrefab, playerSpawner[1].position, playerSpawner[1].rotation);
        P2.playerNumber = 2;
        grabbingZones[1].player = P2;
        P2.modelIndex = playerCharacter[1];
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
    }

    void SpawnStaticObstacles(GameObject staticObjPref,Transform[] spawner)
    {
        int rand = UnityEngine.Random.Range(0, spawner.Length);
        Instantiate(staticObjPref, spawner[rand].position, Quaternion.identity);
        rand++;
        if (rand >= spawner.Length) rand = 0;
        Instantiate(staticObjPref, spawner[rand].position, Quaternion.identity);
    }

    IEnumerator DisplayIntroAndPlay()
    {
        uiGameplay.DisplayIntro();
        yield return new WaitUntil(()=>introDisplayed);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ableToMove = true;
            
        }
        AkSoundEngine.PostEvent("Play_fight", gameObject);
    }

    public void SetLevel(int levelNum)
    {
        if (levelNum >= level.Length) return;
        activeLevel = levelNum;
        for (int i = 0; i < level.Length; i++)
        {
            level[i].SetActive(i == activeLevel);
        }
    }
}
