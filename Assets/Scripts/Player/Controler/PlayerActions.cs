using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActions : MonoBehaviour
{
    public int defenseSlots, gunSlots;
    public float maxBuildingRange, buildCooldown;
    public Transform previewSpot;
    public Transform gunHolder;
    public LayerMask mapLayer;
    public Grid mapGrid;
    public EconomySystem economySystem;
    public MapGridDataManager mapGridDataManager;
    public GameObject[] equippedDefensesPrefabs = new GameObject[10];
    public GameObject[] equippedGunsPrefabs = new GameObject[3];
    private bool isBuilding, isAlive;
    private int currentSelectionIndex, lastSelectionIndex, lastGunIndex, lastDefenseIndex, currentSelectionLimit;
    private float buildCount;
    private Vector3Int lastPreviewGridPos;
    private Quaternion defaultRotation = Quaternion.identity;
    [SerializeField] private GameObject[] defensePreviews = new GameObject[10];
    private GunClass[] gunScripts = new GunClass[3];
    private GridData mapGridData;
    [Header("UI")]
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

    void Start()
    {
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
    }

    void Update()
    {
        if (!isAlive)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isBuilding)
                gunScripts[currentSelectionIndex].OnAttackStart();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
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
                gunScripts[currentSelectionIndex].OnAttackEnd();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
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
        
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0f)
            SwitchSelection(mouseScroll);

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
            defenseOnRightClickAction = DefenseAction.Delete;
            previewInfoText2.text = "Click Derecho: Borrar Defensa";
            previewInfoText2.color = Color.black;
        }
        else
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
        else
            SwitchGun();
    }
}
