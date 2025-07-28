using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class ItemSelectionUI : MonoBehaviour
{
    public LevelManager levelManager;
    public PlayerActions playerActions;
    public GameObject itemSelectionUI;
    public Sprite lockedItemSprite;
    private bool showDefense;
    [Header("Items Prefabs")]
    public List<GameObject> defensePrefabs = new List<GameObject>();
    public List<bool> defenseIsAvailable = new List<bool>();
    public List<GameObject> gunPrefabs = new List<GameObject>();
    public List<bool> gunIsAvailable = new List<bool>();
    [Header("Selectable Items")]
    public GameObject[] availableItemsSlots = new GameObject[5];
    public TextMeshProUGUI[] availableItemsNames = new TextMeshProUGUI[5];
    public Image[] availableItemsIcons = new Image[5];
    public TextMeshProUGUI[] availableItemsDescriptions = new TextMeshProUGUI[5];
    private int firstAvailableItemIndex, maxAvailableItemSlots;
    [Header("Hotbar Items")]
    [SerializeField] private List<GameObject> selectedDefenses = new List<GameObject>();
    [SerializeField] private List<GameObject> selectedGuns = new List<GameObject>();
    private int maxDefenses, maxGuns;

    private void Awake()
    {
        maxAvailableItemSlots = 5;
        firstAvailableItemIndex = 0;
        maxDefenses = playerActions.defenseSlots;
        maxGuns = playerActions.gunSlots;
        OnNewPage();
    }

    public void StartButton()
    {
        if (selectedDefenses.Count == maxDefenses && selectedGuns.Count == maxGuns)
        {
            levelManager.OnGameStart(selectedDefenses, selectedGuns);
            itemSelectionUI.SetActive(false);
        }
        else
            Debug.Log("Faltan items por seleccionar");
    }

    private void OnNewPage()
    {
        for (int i = 0; i < maxAvailableItemSlots; i++)
        {
            if ((firstAvailableItemIndex + i >= defensePrefabs.Count && showDefense) || (firstAvailableItemIndex + i >= gunPrefabs.Count && !showDefense))
                availableItemsSlots[firstAvailableItemIndex + i].SetActive(false);
            else
            {
                if (!availableItemsSlots[firstAvailableItemIndex + i].activeSelf)
                    availableItemsSlots[firstAvailableItemIndex + i].SetActive(true);
                if (defenseIsAvailable[firstAvailableItemIndex + i] && showDefense)
                {
                    DefenseClass defenseData = defensePrefabs[firstAvailableItemIndex + i].GetComponent<DefenseClass>();
                    availableItemsNames[firstAvailableItemIndex + i].text = defenseData.defenseName;
                    availableItemsIcons[firstAvailableItemIndex + i].sprite = defenseData.icon;
                    availableItemsDescriptions[firstAvailableItemIndex + i].text = defenseData.description;
                }
                else if (gunIsAvailable[firstAvailableItemIndex + i] && !showDefense)
                {
                    GunSO gunData = gunPrefabs[firstAvailableItemIndex + i].GetComponent<GunClass>().gunSO;
                    availableItemsNames[firstAvailableItemIndex + i].text = gunData.gunName;
                    availableItemsIcons[firstAvailableItemIndex + i].sprite = gunData.icon;
                    availableItemsDescriptions[firstAvailableItemIndex + i].text = gunData.description;
                }
                else
                {
                    availableItemsNames[firstAvailableItemIndex + i].text = "No Desbloqueado";
                    availableItemsIcons[firstAvailableItemIndex + i].sprite = lockedItemSprite;
                    availableItemsDescriptions[firstAvailableItemIndex + i].text = "";
                }
            }
        }
    }

    public void NextPageButtonClicked(bool forward)
    {
        if (forward)
        {
            firstAvailableItemIndex += maxAvailableItemSlots;
            if ((firstAvailableItemIndex >= defensePrefabs.Count && showDefense) || (firstAvailableItemIndex >= gunPrefabs.Count && !showDefense))
                firstAvailableItemIndex -= maxAvailableItemSlots;
        }
        else
        {
            firstAvailableItemIndex -= maxAvailableItemSlots;
            if (firstAvailableItemIndex < 0)
                firstAvailableItemIndex = 0;
        }
        OnNewPage();
    }

    public void DefenseGunSwitch()
    {
        showDefense = !showDefense;
        OnNewPage();
    }

    public void SelectItem(int itemIndex)
    {
        if (showDefense && (firstAvailableItemIndex + itemIndex < defensePrefabs.Count))
        {
            if (selectedDefenses.Contains(defensePrefabs[firstAvailableItemIndex + itemIndex]))
                selectedDefenses.Remove(defensePrefabs[firstAvailableItemIndex + itemIndex]);
            else if (selectedDefenses.Count < maxDefenses)
            {
                if (defenseIsAvailable[firstAvailableItemIndex + itemIndex])
                    selectedDefenses.Add(defensePrefabs[firstAvailableItemIndex + itemIndex]);
                else
                    Debug.Log("Esta bloqueada");
            }
            else
                Debug.Log("Maximo de defensas alcanzados, quita para agregar");
        }
        else if (!showDefense && (firstAvailableItemIndex + itemIndex < gunPrefabs.Count))
        {
            if (selectedGuns.Contains(gunPrefabs[firstAvailableItemIndex + itemIndex]))
                selectedGuns.Remove(gunPrefabs[firstAvailableItemIndex + itemIndex]);
            else if (selectedGuns.Count < maxGuns)
            {
                if (gunIsAvailable[firstAvailableItemIndex + itemIndex])
                    selectedGuns.Add(gunPrefabs[firstAvailableItemIndex + itemIndex]);
                else
                    Debug.Log("Esta bloqueada");
            }
            else
                Debug.Log("Maximo de armas alcanzados, quita para agregar");
        }
    }
}
