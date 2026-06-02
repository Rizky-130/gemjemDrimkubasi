using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject tempStoragePanel;
    public GameObject trashPanel;
    public GameObject itemInfoPanel;

    private bool isVisible = true;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleInventoryUI()
    {
        isVisible = !isVisible;

        inventoryPanel.SetActive(isVisible);
        tempStoragePanel.SetActive(isVisible);
        trashPanel.SetActive(isVisible);

        // kalau disembunyikan, info item ikut hilang
        if (!isVisible)
        {
            itemInfoPanel.SetActive(false);
        }
        else
        {
            itemInfoPanel.SetActive(true);
        }
    }

    public void ShowInventoryUI()
    {
        isVisible = true;

        inventoryPanel.SetActive(true);
        tempStoragePanel.SetActive(true);
        trashPanel.SetActive(true);
    }

    public void HideInventoryUI()
    {
        isVisible = false;

        inventoryPanel.SetActive(false);
        tempStoragePanel.SetActive(false);
        trashPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
    }
}