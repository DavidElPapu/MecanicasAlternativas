using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, ObjectData> objectsInGrid = new();

    public void AddObjectAt(Vector3Int gridPos, Vector3Int objectSize, ObjectData.CellState cellState, GameObject objectGO)
    {
        List<Vector3Int> objectPositions = CalculatePositions(gridPos, objectSize);
        foreach (Vector3Int pos in objectPositions)
        {
            ObjectData data;
            if (!objectsInGrid.ContainsKey(pos))
                data = new ObjectData(cellState, ObjectData.CellState.Unavailable, objectGO, null);
            else
                data = new ObjectData(cellState, objectsInGrid[pos].cellState, objectGO, objectsInGrid[pos].objectGO);
            objectsInGrid[pos] = data;
        }
    }

    public void RemoveObjectAt(Vector3Int gridPos, Vector3Int objectSize)
    {
        List<Vector3Int> objectPositions = CalculatePositions(gridPos, objectSize);
        foreach (Vector3Int pos in objectPositions)
        {
            //Asumimos que si o si hay algo en todas las casillas, por lo que no hace falta checar
            objectsInGrid[pos].cellState = objectsInGrid[pos].lastCellState;
            objectsInGrid[pos].objectGO = objectsInGrid[pos].lastObjectGO;
        }
    }

    public DefenseClass GetDefenseScriptAt(Vector3Int gridPos)
    {
        if (!objectsInGrid.ContainsKey(gridPos))
            return null;
        if (objectsInGrid[gridPos].objectGO.TryGetComponent<DefenseClass>(out DefenseClass defenseScript))
            return defenseScript;
        return null;
    }

    public ObjectData.CellState GetCellStateAt(Vector3Int gridPos, Vector3Int objectSize)
    {
        if (!objectsInGrid.ContainsKey(gridPos))
            return ObjectData.CellState.Unavailable;
        else if (objectsInGrid[gridPos].cellState != ObjectData.CellState.Defese)
        {
            List<Vector3Int> objectPositions = CalculatePositions(gridPos, objectSize);
            foreach (Vector3Int pos in objectPositions)
            {
                //Asumiendo que groundAvailable y ceilingAvailable jamas estaran juntos, aqui comprueba que todas las casillas sean la misma, sin ningun unavailable ni defense
                if (objectsInGrid.ContainsKey(pos))
                {
                    if (objectsInGrid[pos].cellState == ObjectData.CellState.Unavailable || objectsInGrid[pos].cellState == ObjectData.CellState.Defese)
                        return ObjectData.CellState.Unavailable;
                }
                else
                    return ObjectData.CellState.Unavailable;
            }
        }
        ObjectData.CellState cellStateToReturn = objectsInGrid[gridPos].cellState;
        
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
    public enum CellState
    {
        Unavailable,
        Defese,
        GroundAvailable,
        CeilingAvailable
    }
    public CellState cellState;
    public CellState lastCellState;
    public GameObject objectGO;
    public GameObject lastObjectGO;

    public ObjectData(CellState cellState, CellState lastCellState, GameObject objectGO, GameObject lastObjectGO)
    {
        this.cellState = cellState;
        this.objectGO = objectGO;
        this.lastCellState = lastCellState;
        this.lastObjectGO = lastObjectGO;
    }
}
