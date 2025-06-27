using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelsData levelData;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeMapGrid()
    {
        mapGridDataManager.SetGridData(mapGrid, groundValidDefenseIndicatorsParent, ceilingValidDefenseIndicatorParent);
    }
}
