using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActions : MonoBehaviour
{
    public int defenseSlots, gunSlots, maxDefenseSlots, maxGunSlots;
    public float maxBuildingRange, buildCooldown;
    public Transform previewSpot;
    public Transform gunHolder;
    public LayerMask mapLayer;
    public Grid mapGrid;
    public EconomySystem economySystem;
    public MapGridDataManager mapGridDataManager;
    private List<GameObject> equippedDefensesPrefabs = new List<GameObject>();
    private List<GameObject> equippedGunsPrefabs = new List<GameObject>();
    private bool isBuilding, isAlive;
    private int currentSelectionIndex, lastSelectionIndex, lastGunIndex, lastDefenseIndex, currentSelectionLimit;
    private float buildCount;
    private Vector3Int lastPreviewGridPos;
    private Quaternion defaultRotation = Quaternion.identity;
    [SerializeField] private GameObject[] defensePreviews = new GameObject[10];
    private GunClass[] gunScripts = new GunClass[3];
    private GridData mapGridData;
    [Header("UI")]
    public PlayerMainUI playerUI;
    public GameObject previewInfoCanvas;
    public Sprite[] previewInfoImages = new Sprite[6];
    public Image previewInfoImage;
    public TextMeshProUGUI previewInfoText1, previewInfoText2;

    private enum DefenseAction
    {
        None,
        Build,
        Delete,
        Upgrade,
        Rotate
    }
    private DefenseAction defenseOnLeftClickAction, defenseOnRightClickAction;

    private void Awake()
    {
        isAlive = false;
        //el plan es que primero un  script externo le de los datos de equipped gun y defense prefabs junto con sus gun y defense slots a este script
        //para que luego este script se los pase a la UI cuando empieze el juego, por ahora lo hara en el awake
    }

    public void OnGameStart(List<GameObject> selectedDefenses, List<GameObject> selectedGuns)
    {
        equippedDefensesPrefabs = selectedDefenses;
        equippedGunsPrefabs = selectedGuns;
        isBuilding = true;
        isAlive = true;
        currentSelectionIndex = 0;
        lastSelectionIndex = -1;
        lastGunIndex = 0;
        lastDefenseIndex = 0;
        currentSelectionLimit = defenseSlots;
        buildCount = buildCooldown;
        lastPreviewGridPos = new Vector3Int(0, 0, 0);
        mapGridData = mapGridDataManager.mapGridData;
        defenseOnLeftClickAction = DefenseAction.None;
        defenseOnRightClickAction = DefenseAction.None;
        PlayerStatus.PlayerDeath += OnDeath;
        PlayerStatus.PlayerRevive += OnRevive;
        LevelManager.OnBreakStart += OnBreakStart;
        LevelManager.OnWaveStart += OnWaveStart;
        //esto nomas lo pongo porque inicia contrullendo, pero quiza no hara falta despues que este en start
        ToggleDefensePreviews(true);
        UpdateHotbarUI();
        UpdateSelectedIconUI();
    }

    private void Update()
    {
        if (!isAlive) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isBuilding)
            {
                if (buildCount <= 0)
                {
                    buildCount = buildCooldown;
                    BuildOrUpgrade();
                }
            }
            else
                gunScripts[currentSelectionIndex].OnAttackStart();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!isBuilding)
                gunScripts[currentSelectionIndex].OnAttackEnd();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isBuilding)
            {
                if (buildCount <= 0)
                {
                    buildCount = buildCooldown;
                    RotateOrDelete();
                }
            }
        }
        GetNumbersInput();
        
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0f)
            SwitchSelection(mouseScroll, -1);

        if (isBuilding)
        {
            MovePreview();
            AdjustPreviewSpot();
            previewInfoCanvas.transform.LookAt(Camera.main.transform.position);
            if (buildCount > 0)
            {
                buildCount -= Time.deltaTime;
            }
        }
    }

    private void GetNumbersInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && 0 < currentSelectionLimit)
        {
            SwitchSelection(0, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && 1 < currentSelectionLimit)
        {
            SwitchSelection(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && 2 < currentSelectionLimit)
        {
            SwitchSelection(0, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && 3 < currentSelectionLimit)
        {
            SwitchSelection(0, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && 4 < currentSelectionLimit)
        {
            SwitchSelection(0, 4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && 5 < currentSelectionLimit)
        {
            SwitchSelection(0, 5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && 6 < currentSelectionLimit)
        {
            SwitchSelection(0, 6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && 7 < currentSelectionLimit)
        {
            SwitchSelection(0, 7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && 8 < currentSelectionLimit)
        {
            SwitchSelection(0, 8);
        }
    }

    private void OnBreakStart()
    {
        SwitchMode();
    }

    private void OnWaveStart()
    {
        SwitchMode();
    }

    private void OnDeath()
    {
        isAlive = false;
        if (isBuilding)
        {
            ToggleDefensePreviews(false);
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse0))
                gunScripts[currentSelectionIndex].OnAttackEnd();
        }
    }

    private void OnRevive()
    {
        isAlive = true;
        if (isBuilding)
        {
            defenseOnLeftClickAction = DefenseAction.None;
            defenseOnRightClickAction = DefenseAction.None;
            ToggleDefensePreviews(true);
        }
    }

    #region DefenseFunctions

    private void BuildOrUpgrade()
    {
        if (defenseOnLeftClickAction == DefenseAction.None)
            return;
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        if (defenseOnLeftClickAction == DefenseAction.Build)
        {
            GameObject newDefense = Instantiate(equippedDefensesPrefabs[currentSelectionIndex], mapGrid.CellToWorld(currentPreviewGridPos), defaultRotation);
            DefenseClass newDefenseScript = newDefense.GetComponent<DefenseClass>();
            mapGridData.AddObjectAt(currentPreviewGridPos, newDefenseScript.size, ObjectData.CellState.Defese, newDefense);
            mapGridDataManager.DefenseBuilded(newDefense, currentPreviewGridPos);
            economySystem.ChangeCurrentMoney(-newDefenseScript.defenseLevels[newDefenseScript.GetCurrentLevel()].price);
            newDefenseScript.OnPlacing();
        }
        else if (defenseOnLeftClickAction == DefenseAction.Upgrade)
        {
            DefenseClass otherDefenseScript = mapGridData.GetDefenseScriptAt(currentPreviewGridPos);
            if (otherDefenseScript == null) return;
            economySystem.ChangeCurrentMoney(-otherDefenseScript.defenseLevels[otherDefenseScript.GetCurrentLevel() + 1].price);
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
            for (int i = 0; i <= otherDefenseScript.GetCurrentLevel(); i++)
            {
                returnCash += otherDefenseScript.defenseLevels[i].price;
            }
            mapGridData.RemoveObjectAt(currentPreviewGridPos, otherDefenseScript.size);
            mapGridDataManager.DefenseRemoved(otherDefenseScript.gameObject);
            economySystem.ChangeCurrentMoney(returnCash);
            otherDefenseScript.OnDeleting();
        }
        ChangePreviewColor();
    }

    private void ChangePreviewColor()
    {
        Vector3Int currentPreviewGridPos = mapGrid.WorldToCell(previewSpot.position);
        DefenseClass myDefenseScript = defensePreviews[currentSelectionIndex].GetComponent<DefenseClass>();
        ObjectData.CellState cellState = mapGridData.GetCellStateAt(currentPreviewGridPos, myDefenseScript.size);
        defensePreviews[currentSelectionIndex].SetActive(true);
        defenseOnLeftClickAction = DefenseAction.None;
        defenseOnRightClickAction = DefenseAction.None;
        if(cellState == ObjectData.CellState.Unavailable)
        {
            myDefenseScript.ChangeModelMaterials(Color.red);
            previewInfoText1.text = "";
            previewInfoText2.text = "No Disponible";
            previewInfoText2.color = Color.red;
            previewInfoImage.sprite = previewInfoImages[0];
        }
        else if(cellState == ObjectData.CellState.Defese)
        {
            defensePreviews[currentSelectionIndex].SetActive(false);
            DefenseClass otherDefenseScript = mapGridData.GetDefenseScriptAt(currentPreviewGridPos);
            if (otherDefenseScript == null)
                print("esto no deberia pasar");
            if (otherDefenseScript.defenseName == myDefenseScript.defenseName)
            {
                if (otherDefenseScript.CanBeUpgraded())
                {
                    if (economySystem.GetCurrentMoney() >= otherDefenseScript.GetUpgradePrice())
                    {
                        defenseOnLeftClickAction = DefenseAction.Upgrade;
                        previewInfoText1.text = "Click Izquierdo: Mejorar a nivel " + (otherDefenseScript.GetCurrentLevel() + 2).ToString() + " ($" + otherDefenseScript.GetUpgradePrice().ToString() + ")";
                        previewInfoText1.color = Color.green;
                        previewInfoImage.sprite = previewInfoImages[3];
                    }
                    else
                    {
                        previewInfoText1.text = "Dinero Insuficiente para mejora ($" + otherDefenseScript.GetUpgradePrice().ToString() + ")";
                        previewInfoText1.color = Color.yellow;
                        previewInfoImage.sprite = previewInfoImages[5];
                    }
                }
                else
                {
                    previewInfoText1.text = "Nivel Maximo";
                    previewInfoText1.color = Color.yellow;
                    previewInfoImage.sprite = previewInfoImages[4];
                }
            }
            else
            {
                previewInfoText1.text = "Locacion Ocupada";
                previewInfoText1.color = Color.yellow;
                previewInfoImage.sprite = previewInfoImages[2];
            }
            defenseOnRightClickAction = DefenseAction.Delete;
            previewInfoText2.text = "Click Derecho: Borrar Defensa";
            previewInfoText2.color = Color.black;
        }
        else
        {
            if(mapGridDataManager.GetNumberOfDefense(myDefenseScript.defenseName) < myDefenseScript.maxPlacements)
            {
                if (myDefenseScript.validCells.Contains(cellState))
                {
                    if (economySystem.GetCurrentMoney() >= myDefenseScript.defenseLevels[myDefenseScript.GetCurrentLevel()].price)
                    {
                        myDefenseScript.ChangeModelMaterials(Color.green);
                        defenseOnLeftClickAction = DefenseAction.Build;
                        previewInfoText1.text = "Click Izquierdo: Construir Defensa ($" + myDefenseScript.defenseLevels[myDefenseScript.GetCurrentLevel()].price.ToString() + ")";
                        previewInfoText1.color = Color.green;
                        previewInfoImage.sprite = previewInfoImages[1];
                    }
                    else
                    {
                        myDefenseScript.ChangeModelMaterials(Color.yellow);
                        previewInfoText1.text = "Dinero Insuficiente para construir ($" + myDefenseScript.defenseLevels[myDefenseScript.GetCurrentLevel()].price.ToString() + ")";
                        previewInfoText1.color = Color.yellow;
                        previewInfoImage.sprite = previewInfoImages[5];
                    }
                    defenseOnRightClickAction = DefenseAction.Rotate;
                    previewInfoText2.text = "Click Derecho: Rotar Defensa";
                    previewInfoText2.color = Color.black;
                }
                else
                {
                    myDefenseScript.ChangeModelMaterials(Color.yellow);
                    previewInfoText1.text = "";
                    previewInfoText2.text = "Locacion Invalida";
                    previewInfoText2.color = Color.yellow;
                    previewInfoImage.sprite = previewInfoImages[2];
                }
            }
            else
            {
                myDefenseScript.ChangeModelMaterials(Color.yellow);
                previewInfoText1.text = "";
                previewInfoText2.text = "Limite de " + myDefenseScript.defenseName + " alcazado";
                previewInfoText2.color = Color.yellow;
                previewInfoImage.sprite = previewInfoImages[2];
            }
        }
    }

    private void AdjustPreviewSpot()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildingRange, mapLayer, QueryTriggerInteraction.Ignore))
            previewSpot.localPosition = new Vector3(0, 0, hit.distance - 0.5f);
        else
            previewSpot.localPosition = new Vector3(0, 0, maxBuildingRange);
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
                //ChangePreviewColor();
            }
            else
            {
                Destroy(defensePreviews[i]);
                defensePreviews[i] = null;
            }
        }
    }

    #endregion

    #region GunFunctions

    private void ToggleGunObjects(bool createGunObjects)
    {
        for (int i = 0; i < gunSlots; i++)
        {
            if (createGunObjects)
            {
                GameObject newGunObject = Instantiate(equippedGunsPrefabs[i], gunHolder.position, gunHolder.rotation, gunHolder);
                gunScripts[i] = newGunObject.GetComponent<GunClass>();
                gunScripts[i].OnDeselect();
                if (i == currentSelectionIndex)
                    gunScripts[i].OnSelect(false);
            }
            else
            {
                Destroy(gunScripts[i].gameObject);
                gunScripts[i] = null;
            }
        }
    }

    private void SwitchGun()
    {
        if (lastSelectionIndex == currentSelectionIndex)
            return;
        gunScripts[lastSelectionIndex].OnDeselect();
        gunScripts[currentSelectionIndex].OnSelect(Input.GetKey(KeyCode.Mouse0));
    }

    #endregion

    private void SwitchMode()
    {
        if (isBuilding)
        {
            isBuilding = false;
            lastDefenseIndex = currentSelectionIndex;
            currentSelectionIndex = lastGunIndex;
            currentSelectionLimit = gunSlots;
            ToggleDefensePreviews(false);
            ToggleGunObjects(true);
            previewInfoCanvas.SetActive(false);
        }
        else
        {
            lastGunIndex = currentSelectionIndex;
            currentSelectionIndex = lastDefenseIndex;
            currentSelectionLimit = defenseSlots;
            defenseOnLeftClickAction = DefenseAction.None;
            defenseOnRightClickAction = DefenseAction.None;
            ToggleDefensePreviews(true);
            ToggleGunObjects(false);
            previewInfoCanvas.SetActive(true);
            isBuilding = true;
        }
        UpdateHotbarUI();
        UpdateSelectedIconUI();
    }

    private void SwitchSelection(float mouseScroll, int specificSelection)
    {
        lastSelectionIndex = currentSelectionIndex;
        if (specificSelection == -1)
        {
            //Cambia seleccion en base al mouse
            if (mouseScroll > 0f)
                currentSelectionIndex--;
            else if (mouseScroll < 0f)
                currentSelectionIndex++;
            //Se asegura de que de la vuelta la seleccion
            if (currentSelectionIndex >= currentSelectionLimit)
                currentSelectionIndex = 0;
            else if (currentSelectionIndex < 0)
                currentSelectionIndex = currentSelectionLimit - 1;
        }
        else
        {
            currentSelectionIndex = specificSelection;
        }
        //Si construlle, llama el cambio de defensa
        if (isBuilding)
            SwitchDefensePreview();
        else
            SwitchGun();
        UpdateSelectedIconUI();
    }

    private void UpdateHotbarUI()
    {
        Sprite[] newHotbarIcons = new Sprite[playerUI.hotbarItemsIcons.Length];
        for (int i = 0; i < newHotbarIcons.Length; i++)
        {
            if (i < defenseSlots && isBuilding)
                newHotbarIcons[i] = equippedDefensesPrefabs[i].GetComponent<DefenseClass>().icon;
            else if (i < gunSlots && !isBuilding)
                newHotbarIcons[i] = equippedGunsPrefabs[i].GetComponent<GunClass>().gunSO.icon;
            else if ((i < maxDefenseSlots && isBuilding) || (i < maxGunSlots && !isBuilding))
                newHotbarIcons[i] = playerUI.lockedItemSprite;
            else
                newHotbarIcons[i] = null;
        }
        playerUI.SetHotbarIcons(newHotbarIcons);
    }

    private void UpdateSelectedIconUI()
    {
        if (isBuilding)
        {
            DefenseClass selectedDCS = equippedDefensesPrefabs[currentSelectionIndex].GetComponent<DefenseClass>();
            playerUI.ChangeSelectedItemUI(selectedDCS.icon, selectedDCS.defenseName, "$" + selectedDCS.defenseLevels[selectedDCS.GetCurrentLevel()].price.ToString());
        }
        else
        {
            GunClass selectedGCS = equippedGunsPrefabs[currentSelectionIndex].GetComponent<GunClass>();
            playerUI.ChangeSelectedItemUI(selectedGCS.gunSO.icon, selectedGCS.gunSO.gunName, "por ahora nada, pero aqui podria ir la municion");
        }
    }
}
