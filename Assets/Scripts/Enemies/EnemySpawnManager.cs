using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class EnemySpawnManager : MonoBehaviour
{
    public PlayerMainUI playerUI;
    public event Action WaveCleared;
    public List<GameObject> enemiesAlive = new List<GameObject>();
    private Transform[] enemySpawnPoints = new Transform[6];
    private List<Transform> enemyPath1WayPoints = new List<Transform>();
    private List<Transform> enemyPath2WayPoints = new List<Transform>();
    private List<Transform> enemyPath3WayPoints = new List<Transform>();
    private List<Transform> enemyPath4WayPoints = new List<Transform>();
    private List<Transform> enemyPath5WayPoints = new List<Transform>();
    private List<Transform> enemyPath6WayPoints = new List<Transform>();

    public void SpawnEnemy(GameObject enemyPrefab)
    {
        int spawnPointIndex = UnityEngine.Random.Range(0, enemySpawnPoints.Length);
        List<Transform> newEnemyWayPoints = new List<Transform>();
        switch (spawnPointIndex)
        {
            case 0:
                newEnemyWayPoints = enemyPath1WayPoints;
                break;
            case 1:
                newEnemyWayPoints = enemyPath2WayPoints;
                break;
            case 2:
                newEnemyWayPoints = enemyPath3WayPoints;
                break;
            case 3:
                newEnemyWayPoints = enemyPath4WayPoints;
                break;
            case 4:
                newEnemyWayPoints = enemyPath5WayPoints;
                break;
            case 5:
                newEnemyWayPoints = enemyPath6WayPoints;
                break;
            default:
                break;
        }
        GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnPoints[spawnPointIndex].position, enemySpawnPoints[spawnPointIndex].rotation);
        newEnemy.GetComponent<EnemyClass>().OnSpawn(this, newEnemyWayPoints);
        newEnemy.GetComponent<EnemyClass>().EnemyDeath += EnemyDied;
        enemiesAlive.Add(newEnemy);
        playerUI.ChangeEnemiesAliveText(enemiesAlive.Count);
    }

    public void SetStartingValues(Transform[] enemySpawnPoints, Transform[] wayPointParents)
    {
        this.enemySpawnPoints = enemySpawnPoints;
        for (int i = 0; i < wayPointParents.Length; i++)
        {
            for (int f = 0; f < wayPointParents[i].childCount; f++)
            {
                switch (i)
                {
                    case 0:
                        enemyPath1WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    case 1:
                        enemyPath2WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    case 2:
                        enemyPath3WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    case 3:
                        enemyPath4WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    case 4:
                        enemyPath5WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    case 5:
                        enemyPath6WayPoints.Add(wayPointParents[i].GetChild(f));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void EnemyDied(GameObject enemy)
    {
        if (enemiesAlive.Contains(enemy))
        {
            enemiesAlive.Remove(enemy);
            StartCoroutine(DestroyEnemyAfterDelay(enemy, 0.5f));
            playerUI.ChangeEnemiesAliveText(enemiesAlive.Count);
            if (enemiesAlive.Count <= 0)
                WaveCleared?.Invoke();
        }
    }

    private IEnumerator DestroyEnemyAfterDelay(GameObject enemy, float delay)
    {
        enemy.SetActive(false);
        yield return new WaitForSeconds(delay);
        Destroy(enemy);
    }
}
