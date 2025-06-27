using System.Collections.Generic;
using UnityEngine;

public class MapGridDataManager : MonoBehaviour
{
    public GridData mapGridData = new GridData();

    public void SetGridData(Grid mapGrid, GameObject groundValidDefenseIndicatorsParent, GameObject ceilingValidDefenseIndicatorParent)
    {
        foreach (Transform child in groundValidDefenseIndicatorsParent.transform)
        {
            Transform childOfChild = child.GetChild(0);
            Vector3Int areaSize = new Vector3Int(Mathf.FloorToInt(childOfChild.localScale.x), Mathf.FloorToInt(childOfChild.localScale.y), Mathf.FloorToInt(childOfChild.localScale.z));
            mapGridData.AddObjectAt(mapGrid.WorldToCell(child.transform.position), areaSize, ObjectData.CellState.GroundAvailable, child.gameObject);
        }
        foreach (Transform child in ceilingValidDefenseIndicatorParent.transform)
        {
            Transform childOfChild = child.GetChild(0);
            Vector3Int areaSize = new Vector3Int(Mathf.FloorToInt(childOfChild.localScale.x), Mathf.FloorToInt(childOfChild.localScale.y), Mathf.FloorToInt(childOfChild.localScale.z));
            mapGridData.AddObjectAt(mapGrid.WorldToCell(child.transform.position), areaSize, ObjectData.CellState.CeilingAvailable, child.gameObject);
        }
    }
}
