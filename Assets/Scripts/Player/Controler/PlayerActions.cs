using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public int defenseSlots, gunSlots;
    public Transform previewSpot;
    public Grid mapGrid;
    public DefensesSO[] equippedDefenses = new DefensesSO[10];
    public GridData mapGridData;
    private bool isBuilding;
    private int currentSelectionIndex, lastSelectionIndex, lastGunIndex, lastDefenseIndex, currentSelectionLimit;
    private Vector3Int lastPreviewGridPos;
    private Quaternion defaultRotation = Quaternion.identity;
    private GameObject[] defensePreviews = new GameObject[10];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            //if(isBuilding)
            //    //construlle
            //    else
            //        //dispara
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            //quiza habilidad secundaria de arma o rotacion de defensa
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
        }
    }

    private void MovePreview()
    {
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        if (currentPreviewGridPos == lastPreviewGridPos)
            return;
        lastPreviewGridPos = currentPreviewGridPos;
        defensePreviews[currentSelectionIndex].transform.position = currentPreviewGridPos;
    }

    private void SwitchDefensePreview()
    {
        if (lastSelectionIndex == currentSelectionIndex)
            return;
        defensePreviews[lastSelectionIndex].SetActive(false);
        defensePreviews[currentSelectionIndex].SetActive(true);
    }

    private void ToggleDefensePreviews(bool createPreviews)
    {
        for (int i = 0; i < defenseSlots; i++)
        {
            if (createPreviews)
            {
                GameObject newPreview = Instantiate(equippedDefenses[i].prefab, new Vector3(0, 0, 0), defaultRotation);
                defensePreviews[i] = newPreview;
                newPreview.SetActive(false);
                if (i == currentSelectionIndex)
                    newPreview.SetActive(true);
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
