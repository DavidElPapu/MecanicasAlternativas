using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public int defenseSlots, gunSlots;
    public float maxBuildingRange;
    public Transform previewSpot;
    public LayerMask mapLayer;
    public Grid mapGrid;
    public EconomySystem economySystem;
    public MapGridDataManager mapGridDataManager;
    public GameObject[] equippedDefensesPrefabs = new GameObject[10];
    private bool isBuilding;
    private int currentSelectionIndex, lastSelectionIndex, lastGunIndex, lastDefenseIndex, currentSelectionLimit;
    private Vector3Int lastPreviewGridPos;
    private Quaternion defaultRotation = Quaternion.identity;
    private GameObject[] defensePreviews = new GameObject[10];
    private GridData mapGridData;
    private enum DefenseAction
    {
        None,
        Build,
        Delete,
        Upgrade,
        Rotate
    }
    private DefenseAction defenseOnLeftClickAction, defenseOnRightClickAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapGridData = mapGridDataManager.mapGridData;
        isBuilding = true;
        currentSelectionLimit = defenseSlots;
        currentSelectionIndex = 0;
        lastGunIndex = 0;
        lastDefenseIndex = 0;
        lastPreviewGridPos = new Vector3Int(0, 0, 0);
        //esto nomas lo pongo porque inicia contrullendo, pero quiza no hara falta despues que este en start
        ToggleDefensePreviews(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (isBuilding)
                BuildOrUpgrade();
            //else
                    //dispara
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (isBuilding)
                RotateOrDelete();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            SwitchMode();
        }
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0f)
            SwitchSelection(mouseScroll);

        if (isBuilding)
        {
            MovePreview();
            AdjustPreviewSpot();
        }
    }

    private void BuildOrUpgrade()
    {
        if (defenseOnLeftClickAction == DefenseAction.None)
            return;
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        if (defenseOnLeftClickAction == DefenseAction.Build)
        {
            GameObject newDefense = Instantiate(equippedDefensesPrefabs[currentSelectionIndex], mapGrid.CellToWorld(currentPreviewGridPos), defaultRotation);
            DefenseClass newDefenseScript = newDefense.GetComponent<DefenseClass>();
            mapGridData.AddObjectAt(currentPreviewGridPos, newDefenseScript.defenseSO.size, ObjectData.CellState.Defese, newDefense);
            economySystem.ChangeCurrentMoney(-newDefenseScript.defenseSO.levelPrices[newDefenseScript.GetCurrentLevel()]);
            newDefenseScript.OnPlacing();
        }
        else if (defenseOnLeftClickAction == DefenseAction.Upgrade)
        {
            DefenseClass otherDefenseScript = mapGridData.GetDefenseScriptAt(currentPreviewGridPos);
            economySystem.ChangeCurrentMoney(-otherDefenseScript.defenseSO.levelPrices[otherDefenseScript.GetCurrentLevel()]);
            otherDefenseScript.OnUpgrading();

        }
        ChangePreviewColor();
    }

    private void RotateOrDelete()
    {
        if (defenseOnRightClickAction == DefenseAction.None)
            return;
        if (defenseOnRightClickAction == DefenseAction.Rotate)
        {
            //rotacion insana
        }
        else if (defenseOnRightClickAction == DefenseAction.Delete)
        {
            Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
            DefenseClass otherDefenseScript = mapGridData.GetDefenseScriptAt(currentPreviewGridPos);
            int returnCash = 0;
            for (int i = 0; i < otherDefenseScript.GetCurrentLevel(); i++)
            {
                returnCash += otherDefenseScript.defenseSO.levelPrices[i];
            }
            mapGridData.RemoveObjectAt(currentPreviewGridPos, otherDefenseScript.defenseSO.size);
            economySystem.ChangeCurrentMoney(returnCash);
            otherDefenseScript.OnDeleting();
        }
        ChangePreviewColor();
    }

    private void ChangePreviewColor()
    {
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        DefenseClass myDefenseScript = defensePreviews[currentSelectionIndex].GetComponent<DefenseClass>();
        ObjectData.CellState cellState = mapGridData.GetCellStateAt(currentPreviewGridPos, myDefenseScript.defenseSO.size);
        defensePreviews[currentSelectionIndex].SetActive(true);
        defenseOnLeftClickAction = DefenseAction.None;
        defenseOnRightClickAction = DefenseAction.None;
        if(cellState == ObjectData.CellState.Unavailable)
            myDefenseScript.ChangeModelMaterials(Color.red);
        else if(cellState == ObjectData.CellState.Defese)
        {
            defensePreviews[currentSelectionIndex].SetActive(false);
            DefenseClass otherDefenseScript = mapGridData.GetDefenseScriptAt(currentPreviewGridPos);
            if (otherDefenseScript == null)
                print("esto no deberia pasar");
            if (otherDefenseScript.defenseSO.defenseName == myDefenseScript.defenseSO.defenseName)
            {
                if (otherDefenseScript.GetCurrentLevel() < otherDefenseScript.defenseSO.levelPrices.Count)
                {
                    if (economySystem.GetCurrentMoney() >= otherDefenseScript.defenseSO.levelPrices[otherDefenseScript.GetCurrentLevel()])
                        defenseOnLeftClickAction = DefenseAction.Upgrade;
                }
                else
                {
                    //esta al nivel max
                }
            }
            defenseOnRightClickAction = DefenseAction.Delete;
        }
        else
        {
            if (myDefenseScript.defenseSO.validCells.Contains(cellState))
            {
                if (economySystem.GetCurrentMoney() >= myDefenseScript.defenseSO.levelPrices[myDefenseScript.GetCurrentLevel()])
                {
                    myDefenseScript.ChangeModelMaterials(Color.green);
                    defenseOnLeftClickAction = DefenseAction.Build;
                }
                else
                    myDefenseScript.ChangeModelMaterials(Color.yellow);
                defenseOnRightClickAction = DefenseAction.Rotate;
            }
            else
                myDefenseScript.ChangeModelMaterials(Color.red);
        }
    }

    private void AdjustPreviewSpot()
    {
        Vector3 rayDirection = previewSpot.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection.normalized, out RaycastHit hit, maxBuildingRange, mapLayer, QueryTriggerInteraction.Ignore))
        {
            previewSpot.position = hit.point;
        }
        else
        {
            previewSpot.localPosition = new Vector3(0, 0, maxBuildingRange);
        }
    }

    private void MovePreview()
    {
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        if (currentPreviewGridPos == lastPreviewGridPos)
            return;
        lastPreviewGridPos = currentPreviewGridPos;
        defensePreviews[currentSelectionIndex].transform.position = currentPreviewGridPos;
        ChangePreviewColor();
    }

    private void SwitchDefensePreview()
    {
        if (lastSelectionIndex == currentSelectionIndex)
            return;
        defensePreviews[lastSelectionIndex].SetActive(false);
        defensePreviews[currentSelectionIndex].SetActive(true);
        lastPreviewGridPos = mapGrid.WorldToCell(defensePreviews[currentSelectionIndex].transform.position);
        ChangePreviewColor();
    }

    private void ToggleDefensePreviews(bool createPreviews)
    {
        for (int i = 0; i < defenseSlots; i++)
        {
            if (createPreviews)
            {
                GameObject newPreview = Instantiate(equippedDefensesPrefabs[i], new Vector3(0, 0, 0), defaultRotation);
                defensePreviews[i] = newPreview;
                newPreview.SetActive(false);
                if (i == currentSelectionIndex)
                    newPreview.SetActive(true);
                ChangePreviewColor();
            }
            else
            {
                Destroy(defensePreviews[i]);
                defensePreviews[i] = null;
            }
        }
    }

    private void SwitchMode()
    {
        if (isBuilding)
        {
            isBuilding = false;
            lastDefenseIndex = currentSelectionIndex;
            currentSelectionIndex = lastGunIndex;
            currentSelectionLimit = gunSlots;
            ToggleDefensePreviews(false);
        }
        else
        {
            isBuilding = true;
            lastGunIndex = currentSelectionIndex;
            currentSelectionIndex = lastDefenseIndex;
            currentSelectionLimit = defenseSlots;
            ToggleDefensePreviews(true);
        }
    }

    private void SwitchSelection(float mouseScroll)
    {
        lastSelectionIndex = currentSelectionIndex;
        //Cambia seleccion en base al mouse
        if (mouseScroll > 0f)
            currentSelectionIndex++;
        else if (mouseScroll < 0f)
            currentSelectionIndex--;
        //Se asegura de que de la vuelta la seleccion
        if (currentSelectionIndex >= currentSelectionLimit)
            currentSelectionIndex = 0;
        else if (currentSelectionIndex < 0)
            currentSelectionIndex = currentSelectionLimit - 1;
        //Si construlle, llama el cambio de defensa
        if (isBuilding)
            SwitchDefensePreview();
    }
}
