using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense", menuName = "Scriptable Objects/Defense")]
public class DefensesSO : ScriptableObject
{
    public string defenseName;
    public string description;
    public Vector3Int size = new Vector3Int(1, 1, 1);
    public GameObject prefab;
}
