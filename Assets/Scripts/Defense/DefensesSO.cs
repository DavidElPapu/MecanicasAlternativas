using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Scriptable Objects/Defense")]
public class DefensesSO : ScriptableObject
{
    [Header("General Data")]
    public string defenseName;
    public string description;
    public List<int> levelPrices = new List<int>();
    public Vector3Int size = new Vector3Int(1, 1, 1);
    public List<ObjectData.CellState> validCells = new List<ObjectData.CellState>();
    [Header("Defense Data")]
    public float damage;
    public float health;
    public float mainCooldown;
    public enum DebuffType
    {
        Stun,
        Slow,
        Poison,
        Weakness
    }
    public List<DebuffType> appliedDebuffs = new List<DebuffType>();
    public List<float> debuffsIntensities = new List<float>();
    public List<float> debuffsTimes = new List<float>();
    public enum EnemyInteractionType
    {
        IgnoreEnemyAndDefense,
        IgnoreEnemyTargetDefense,
        IgnoreDefenseTargetEnemy,
        TargetDefenseAndEnemy,
    }
    public EnemyInteractionType enemyInteraction;
}
