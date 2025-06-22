using UnityEngine;

public class DefenseBuildingBehaviour : MonoBehaviour
{
    public GameObject defenseModel;
    public enum DefenseType
    {
        trap,
        turret,
        movement
    }
    public DefenseType defenseType;
    public Material validMAT, invalidMAT, expensiveMAT;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
