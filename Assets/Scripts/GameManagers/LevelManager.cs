using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("LevelManagement")]
    public LevelsData levelData;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWave = 0;
        isOnBreak = true;
        InitializeMapGrid();
        InitializeEnemySpawner();
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
