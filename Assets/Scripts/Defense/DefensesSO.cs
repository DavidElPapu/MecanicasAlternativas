using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Scriptable Objects/Defense")]
public class DefensesSO : ScriptableObject
{
    public int price;
    public float damage;
    public float range;
    public float maxHealth;
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
    
}
