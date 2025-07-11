using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Scriptable Objects/Gun")]
public class GunSO : ScriptableObject
{
    [Header("General Data")]
    public string gunName;
    public string description;
    public Sprite icon;
    public LayerMask collisionLayer;
    [Header("Gun Data")]
    public float damage;
    public float attackCooldown;
    public float range;
}
