using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    [Header("LevelManagement")]
    public LevelsData levelData;
    public static event Action OnWaveStart;
    public static event Action OnBreakStart;
    public static event Action OnGameLost;
    private int currentWave;
    private bool isOnBreak, lostGame;
    [Header("MapGridData")]
    public MapGridDataManager mapGridDataManager;
    public GameObject groundValidDefenseIndicatorsParent, ceilingValidDefenseIndicatorParent;
    public Grid mapGrid;
    [Header("EnemyManagerData")]
    public EnemySpawnManager enemySpawnManager;
    public Transform[] enemySpawnPoints = new Transform[6];
    public Transform[] mapWayPointParents = new Transform[6];
    private List<List<GameObject>> wavesOfEnemies = new List<List<GameObject>>();
    public float enemySpawnRate;
    private int enemyIndex;
    [Header("PlayerManagement")]
    public PlayerActions playerActions;
    public PlayerMovement playerMovement;
    public PlayerStatus playerStatus;
    public EconomySystem playerEconomy;
    public float playerReviveTime;
    public Transform playerSpawn, playerTransform;
    [Header("UI")]
    public PlayerMainUI playerUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWave = 0;
        isOnBreak = true;
        lostGame = false;
        enemyIndex = 0;
        InitializeMapGrid();
        InitializeEnemySpawner();
        SetWavesOfEnemies();
        PlayerStatus.PlayerDeath += OnPlayerDeath;
        enemySpawnManager.WaveCleared += OnWaveContinue;
        //enemySpawnManager.SpawnEnemy(levelData.wave1Enemies[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q) && isOnBreak)
        {
            OnWaveContinue();
        }
    }

    public void OnGameStart(List<GameObject> selectedDefenses, List<GameObject> selectedGuns)
    {
        playerActions.OnGameStart(selectedDefenses, selectedGuns);
        playerMovement.OnGameStart();
    }

    public void OnWaveContinue()
    {
        if (isOnBreak)
        {
            currentWave++;
            isOnBreak = false;
            playerUI.ChangeWaveStatus("Oleada " + currentWave.ToString());
            OnWaveStart?.Invoke();
            enemyIndex = 0;
            InvokeRepeating("SpawnEnemies", enemySpawnRate, enemySpawnRate);
        }
        else
        {
            isOnBreak = true;
            playerUI.ChangeWaveStatus("Descanso");
            playerEconomy.ChangeCurrentMoney(levelData.cashWaveReward[currentWave]);
            OnBreakStart?.Invoke();
        }
    }

    private void SpawnEnemies()
    {
        enemySpawnManager.SpawnEnemy(wavesOfEnemies[currentWave - 1][enemyIndex]);
        enemyIndex++;
        if (enemyIndex >= wavesOfEnemies[currentWave - 1].Count)
            CancelInvoke("SpawnEnemies");
    }

    private void InitializeEnemySpawner()
    {
        enemySpawnManager.SetStartingValues(enemySpawnPoints, mapWayPointParents);
    }

    private void InitializeMapGrid()
    {
        mapGridDataManager.SetGridData(mapGrid, groundValidDefenseIndicatorsParent, ceilingValidDefenseIndicatorParent);
    }

    private void SetWavesOfEnemies()
    {
        if (levelData.levelWaves <= 10)
        {
            wavesOfEnemies.Add(levelData.wave1Enemies);
            wavesOfEnemies.Add(levelData.wave2Enemies);
            wavesOfEnemies.Add(levelData.wave3Enemies);
            wavesOfEnemies.Add(levelData.wave4Enemies);
            wavesOfEnemies.Add(levelData.wave5Enemies);
            wavesOfEnemies.Add(levelData.wave6Enemies);
            wavesOfEnemies.Add(levelData.wave7Enemies);
            wavesOfEnemies.Add(levelData.wave8Enemies);
            wavesOfEnemies.Add(levelData.wave9Enemies);
            wavesOfEnemies.Add(levelData.wave10Enemies);
        }
        if (levelData.levelWaves <= 20)
        {
            wavesOfEnemies.Add(levelData.wave11Enemies);
            wavesOfEnemies.Add(levelData.wave12Enemies);
            wavesOfEnemies.Add(levelData.wave13Enemies);
            wavesOfEnemies.Add(levelData.wave14Enemies);
            wavesOfEnemies.Add(levelData.wave15Enemies);
            wavesOfEnemies.Add(levelData.wave16Enemies);
            wavesOfEnemies.Add(levelData.wave17Enemies);
            wavesOfEnemies.Add(levelData.wave18Enemies);
            wavesOfEnemies.Add(levelData.wave19Enemies);
            wavesOfEnemies.Add(levelData.wave20Enemies);
        }
        if (levelData.levelWaves <= 30)
        {
            wavesOfEnemies.Add(levelData.wave21Enemies);
            wavesOfEnemies.Add(levelData.wave22Enemies);
            wavesOfEnemies.Add(levelData.wave23Enemies);
            wavesOfEnemies.Add(levelData.wave24Enemies);
            wavesOfEnemies.Add(levelData.wave25Enemies);
            wavesOfEnemies.Add(levelData.wave26Enemies);
            wavesOfEnemies.Add(levelData.wave27Enemies);
            wavesOfEnemies.Add(levelData.wave28Enemies);
            wavesOfEnemies.Add(levelData.wave29Enemies);
            wavesOfEnemies.Add(levelData.wave30Enemies);
        }
    }

    public void BaseDied()
    {
        OnGameLost?.Invoke();
    }

    private void OnPlayerDeath()
    {
        playerUI.ToggleDeathUI(true);
        Invoke("RevivePlayer", playerReviveTime);
    }

    private void RevivePlayer()
    {
        playerUI.ToggleDeathUI(false);
        playerTransform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);
        playerStatus.OnRevive();
    }
}
