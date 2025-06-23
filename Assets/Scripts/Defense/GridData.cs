using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, ObjectData> objectsInGrid = new();

    public void AddObjectAt(Vector3Int gridPos, Vector3Int objectSize, ObjectData.CellState cellState, GameObject objectGO)
    {
        List<Vector3Int> objectPositions = CalculatePositions(gridPos, objectSize);
        ObjectData data = new ObjectData(objectPositions, cellState, objectGO);
        foreach (Vector3Int pos in objectPositions)
        {
            objectsInGrid[pos] = data;
        }
    }

    public ObjectData.CellState GetCellStateAt(Vector3Int gridPos)
    {
        if (!objectsInGrid.ContainsKey(gridPos))
            return ObjectData.CellState.Unavailable;
        return objectsInGrid[gridPos].cellState;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, Vector3Int objectSize)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                for (int z = 0; z < objectSize.z; z++)
                {
                    positions.Add(gridPos + new Vector3Int(x, y, z));
                }
            }
        }
        return positions;
    }
}

public class ObjectData
{
    public List<Vector3Int> occupiedPositions;
    public enum CellState
    {
        Unavailable,
        Defese,
        GroundAvailable,
        CeilingAvailable,
        WallAvailable
    }
    public CellState cellState;
    public GameObject objectGO;

    public ObjectData(List<Vector3Int> occupiedPositions, CellState cellState, GameObject objectGO)
    {
        this.occupiedPositions = occupiedPositions;
        this.cellState = cellState;
        this.objectGO = objectGO;
    }
}
