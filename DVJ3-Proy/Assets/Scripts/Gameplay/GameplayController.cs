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
    
    [SerializeField] GrabbingZone[] grabbingZones = null;
    [SerializeField] CameraController cameraController = null;
    [SerializeField] PostProcessManager ppManager = null;
    public bool introDisplayed = false;
    [SerializeField] TesterTool tester = null;
    [SerializeField] GameObject testerActivatedText = null;
    static bool activateTester = false;
    [SerializeField] GameObject[] level = null;
    int activeLevel = 1;

    [Header("General Stage")]
    [SerializeField] Transform[] playerSpawner = null;
    [SerializeField] GameObject firePref = null;
    [SerializeField] FallingObstController foc = null;

    [Header("Stage1")]
    [SerializeField] GameObject staticObstaclePrefStg1 = null;
    [SerializeField] Transform[] staticObstSpawnStg1 = null;
    [SerializeField] Transform[] fireSpawnStg1 = null;
    [SerializeField] GameObject fallingObstaclePrefStg1 = null;

    [Header("Stage2")]
    [SerializeField] GameObject staticObstaclePrefStg2 = null;
    [SerializeField] Transform[] staticObstSpawnStg2 = null;
    [SerializeField] Transform[] fireSpawnStg2 = null;
    [SerializeField] GameObject fallingObstaclePrefStg2 = null;

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

    string cheat ="AEZAKMI";
    int cheatPos = 0;

    void Start()
    {
        Time.timeScale = 1.0f;
        uiGameplay = FindObjectOfType<UIGameplay>();
        GameManager.Get().SetGameplayController(this);

        if (SceneManager.sceneCount < 2)
            OnGameplaySceneStart(new int[] {0, 0});
    }

    private void Update()
    {
        if (GameManager.Get().gameState==GameManager.GameState.inMenus && !activateTester && (Input.inputString == cheat[cheatPos].ToString() || Input.inputString == (cheat[cheatPos].ToString().ToLower())))
        {
            Debug.Log(cheat[cheatPos]);
            cheatPos++;
            if (cheatPos >= cheat.Length)
            {
                cheatPos = 0;
                testerActivatedText.SetActive(true);
                activateTester = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
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
        uiGameplay.SetRoundWinner(winnerPlayer);
        if (player1wins > maxRounds/2 || player2wins > maxRounds / 2)
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
        AkSoundEngine.PostEvent("End_fight", gameObject);
        StartCoroutine(EndscreenInput());
    }

    IEnumerator EndscreenInput()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(1);
                GameManager.Get().gameState = GameManager.GameState.inMenus;
            }
            yield return null;
        }
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
                foc.StartSpawning(fallingObstaclePrefStg1);
                break;
            case 1:
            default:
                AkSoundEngine.PostEvent("Inicia_granja", gameObject);
                SpawnStaticObstacles(staticObstaclePrefStg2,staticObstSpawnStg2);
                foc.StartSpawning(fallingObstaclePrefStg2);
                break;
        }
        ppManager.StartRemovingCAberration();
        PlayerController P1 = Instantiate(playerPrefab, playerSpawner[0].position, playerSpawner[0].rotation); // se puede hacer un for
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
        if (activateTester) tester.gameObject.SetActive(true);
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
