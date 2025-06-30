using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemiesSO : ScriptableObject
{
    [Header("General Data")]
    public string enemyName;
    public string description;
    public enum EnemyType
    {
        Normal,
        Flying,
        Invisible,
        CcInmune,
        Boss
    }
    public EnemyType enemyType;
    [Header("Enemy Data")]
    public float damage;
    public float attackCooldown;
    public float health;
    public int shieldPoints;
    public float speed;
    public enum FocusType
    {
        Ignore,
        AttackOnlyInRange,
        AlwaysFollow
    }
    public FocusType playerFocus;
}
