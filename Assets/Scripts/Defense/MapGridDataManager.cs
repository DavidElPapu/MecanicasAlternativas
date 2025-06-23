using System.Collections.Generic;
using UnityEngine;

public class MapGridDataManager : MonoBehaviour
{
    public GameObject groundValidDefenseIndicatorsParent;
    public Grid mapGrid;
    public GridData mapGridData = new GridData();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGridData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetGridData()
    {
        foreach (Transform child in groundValidDefenseIndicatorsParent.transform)
        {
            Transform childOfChild = child.GetChild(0);
            Vector3Int areaSize = new Vector3Int(Mathf.FloorToInt(childOfChild.localScale.x), Mathf.FloorToInt(childOfChild.localScale.y), Mathf.FloorToInt(childOfChild.localScale.z));
            mapGridData.AddObjectAt(mapGrid.WorldToCell(child.transform.position), areaSize, ObjectData.CellState.GroundAvailable, child.gameObject);
        }
    }
}
