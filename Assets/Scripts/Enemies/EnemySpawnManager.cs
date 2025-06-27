using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    private Transform[] enemySpawnPoints = new Transform[6];
    private List<Transform> enemyPath1WayPoints = new List<Transform>();
    private List<Transform> enemyPath2WayPoints = new List<Transform>();
    private List<Transform> enemyPath3WayPoints = new List<Transform>();
    private List<Transform> enemyPath4WayPoints = new List<Transform>();
    private List<Transform> enemyPath5WayPoints = new List<Transform>();
    private List<Transform> enemyPath6WayPoints = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
