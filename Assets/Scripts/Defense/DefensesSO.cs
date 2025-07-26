using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Scriptable Objects/Defense")]
public class DefensesSO : ScriptableObject
{
    public int price;
    public float damage;
    //Para toda defensa que necesite de un rango (ya sea para hacer raycast o expandir area de detección)
    public float range;
    public float maxHealth;
    //Cooldown que puede servir para cadencia de disparo, cooldown de trampa, de ataque, de uso, etc..
    public float mainCooldown;
    //Sirve para cualquier tipo de usos que tenga la defensa, como municion, o usos  especificos (0 significa que no lo necesita o usa)
    public int maxUses;
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
