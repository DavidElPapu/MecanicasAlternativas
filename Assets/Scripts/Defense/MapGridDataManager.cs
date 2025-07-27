using System.Collections.Generic;
using UnityEngine;

public class MapGridDataManager : MonoBehaviour
{
    public GridData mapGridData = new GridData();
    private List<GameObject> buildedDefenses = new List<GameObject>();
    private List<Vector3Int> defensesGridLocations = new List<Vector3Int>();

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

    public void DefenseBuilded(GameObject newDefense, Vector3Int defensePos)
    {
        buildedDefenses.Add(newDefense);
        defensesGridLocations.Add(defensePos);
    }

    public void DefenseRemoved(GameObject defense)
    {
        if (buildedDefenses.Contains(defense))
        {
            int removeIndex = buildedDefenses.IndexOf(defense);
            buildedDefenses.RemoveAt(removeIndex);
            defensesGridLocations.RemoveAt(removeIndex);
        }
    }

    public int GetNumberOfDefense(string defenseName)
    {
        int numberOfDefense = 0;
        foreach (GameObject defense in buildedDefenses)
        {
            if (defense.GetComponent<DefenseClass>().defenseName == defenseName)
                numberOfDefense++;
        }
        return numberOfDefense;
    }
}
