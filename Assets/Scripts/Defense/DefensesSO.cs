using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Scriptable Objects/Defense")]
public class DefensesSO : ScriptableObject
{
    [Header("General Data")]
    public string defenseName;
    public string description;
    public Vector3Int size = new Vector3Int(1, 1, 1);
    public GameObject prefab;
    [Header("Defense Data")]
    public float damage;
    public float health;
    public float mainCooldown;
    public float secondaryCooldown;
    public float cooldown3;
    public enum DebuffType
    {
        none,
        stun,
        slow,
        poison,
        weakness
    }
    public DebuffType appliedDebuff;
    public float appliedDebuffTime;
}
