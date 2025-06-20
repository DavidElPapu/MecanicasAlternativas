using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public int defenseSlots, gunSlots;
    public Transform previewSpot;
    public DefensesSO[] equippedDefenses = new DefensesSO[10];
    public GridData mapGridData;
    private bool isBuilding;
    private int currentSelectionIndex, lastGunIndex, lastDefenseIndex, currentSelectionLimit;
    private Quaternion defaultRotation = Quaternion.identity;
    private GameObject currentDefensePreview;
    private GameObject[] defensePreviews = new GameObject[10];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBuilding = true;
        currentSelectionLimit = defenseSlots;
        currentSelectionIndex = 0;
        lastGunIndex = 0;
        lastDefenseIndex = 0;
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
            
        }
    }

    private void SwitchDefensePreview()
    {

    }

    private void ToggleDefensePreviews(bool createPreviews)
    {
        if (createPreviews)
        {
            for (int i = 0; i < defenseSlots; i++)
            {
                Instantiate(equippedDefenses[i].prefab, new Vector3(0, 0, 0), defaultRotation);
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
        }
        else
        {
            isBuilding = true;
            lastGunIndex = currentSelectionIndex;
            currentSelectionIndex = lastDefenseIndex;
            currentSelectionLimit = defenseSlots;
        }
    }

    private void SwitchSelection(float mouseScroll)
    {
        if (mouseScroll > 0f)
            currentSelectionIndex++;
        else if (mouseScroll < 0f)
            currentSelectionIndex--;
        if (currentSelectionIndex >= currentSelectionLimit)
            currentSelectionIndex = 0;
        else if (currentSelectionIndex < 0)
            currentSelectionIndex = currentSelectionLimit - 1;
    }
}
