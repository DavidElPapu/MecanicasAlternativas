using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    [Header("LevelManagement")]
    public LevelsData levelData;
    public static event Action OnWaveStart;
    public static event Action OnBreakStart;
    private int currentWave;
    private bool isOnBreak;
    [Header("MapGridData")]
    public MapGridDataManager mapGridDataManager;
    public GameObject groundValidDefenseIndicatorsParent, ceilingValidDefenseIndicatorParent;
    public Grid mapGrid;
    [Header("EnemyManagerData")]
    public EnemySpawnManager enemySpawnManager;
    public Transform[] enemySpawnPoints = new Transform[6];
    public Transform[] mapWayPointParents = new Transform[6];
    [Header("PlayerManagement")]
    public PlayerStatus playerStatus;
    public float playerReviveTime;
    public Transform playerSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWave = 0;
        isOnBreak = true;
        InitializeMapGrid();
        InitializeEnemySpawner();
        PlayerStatus.PlayerDeath += OnPlayerDeath;
        enemySpawnManager.SpawnEnemy(levelData.wave1Enemies[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnBreak)
        {

        }
        else
        {

        }
    }

    private void OnPlayerDeath()
    {
        Invoke("RevivePlayer", playerReviveTime);
    }

    private void RevivePlayer()
    {
        playerStatus.OnRevive();
    }


    private void SpawnEnemies()
    {

    }

    private void InitializeEnemySpawner()
    {
        enemySpawnManager.SetStartingValues(enemySpawnPoints, mapWayPointParents);
    }

    private void InitializeMapGrid()
    {
        mapGridDataManager.SetGridData(mapGrid, groundValidDefenseIndicatorsParent, ceilingValidDefenseIndicatorParent);
    }
}
