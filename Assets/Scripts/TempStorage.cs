using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TempStorage : MonoBehaviour, IDropHandler
{
    public static TempStorage Instance;

    public int capacity = 9;
    public GameObject slotPrefab;
    public Transform slotContainer;

    private InventoryItem[] storedItems;

    private void Awake()
    {
        Instance = this;
        storedItems = new InventoryItem[capacity];
    }

    private void Start()
    {
        GenerateSlots();
    }

    private void GenerateSlots()
    {
        if (slotPrefab == null)
        {
            Debug.LogError("TempStorage slotPrefab is missing!");
            return;
        }

        if (slotContainer == null)
        {
            Debug.LogError("TempStorage slotContainer is missing!");
            return;
        }

        for (int i = 0; i < capacity; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            slotObj.name = $"TempSlot ({i})";

            Image slotImage = slotObj.GetComponent<Image>();

            if (slotImage != null)
            {
                slotImage.raycastTarget = true;
            }

            TempStorageSlot slotScript = slotObj.GetComponent<TempStorageSlot>();

            if (slotScript == null)
            {
                slotScript = slotObj.AddComponent<TempStorageSlot>();
            }
        }
    }

    public bool HasSpace()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (storedItems[i] == null)
                return true;
        }

        return false;
    }

    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (storedItems[i] == null)
                return i;
        }

        return -1;
    }

    public InventoryItem GetStoredItem(int index)
    {
        if (index < 0 || index >= capacity)
            return null;

        return storedItems[index];
    }

    public bool SpawnItemToTempStorage(InventoryItem item)
    {
        if (item == null)
            return false;

        if (!HasSpace())
        {
            Debug.Log("Temp storage penuh!");
            return false;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory.Instance not found. Cannot create temp storage item view.");
            return false;
        }

        Inventory.Instance.CreateItemViewPublic(item);
        Inventory.Instance.UpdateItemViewSize(item);

        bool stored = StoreItem(item);

        return stored;
    }

    public bool StoreItem(InventoryItem item)
    {
        if (item == null)
            return false;

        int index = GetEmptySlotIndex();

        if (index == -1)
        {
            Debug.Log("Temp storage penuh!");
            return false;
        }

        storedItems[index] = item;
        item.tempStorageIndex = index;
        item.isInTempStorage = true;

        MoveViewToSlot(item, index);

        Debug.Log($"{item.itemName} disimpan di TempStorage slot {index}");

        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.CheckUpgrade();
        }

        return true;
    }

    private void MoveViewToSlot(InventoryItem item, int index)
    {
        if (item == null)
            return;

        if (item.view == null)
            return;

        if (slotContainer == null)
            return;

        if (index < 0 || index >= slotContainer.childCount)
            return;

        Transform slot = slotContainer.GetChild(index);
        RectTransform itemRect = item.view.GetComponent<RectTransform>();

        if (itemRect == null)
            return;

        itemRect.SetParent(slot, false);
        itemRect.SetAsLastSibling();

        itemRect.anchorMin = Vector2.zero;
        itemRect.anchorMax = Vector2.one;
        itemRect.offsetMin = Vector2.zero;
        itemRect.offsetMax = Vector2.zero;
        itemRect.pivot = new Vector2(0.5f, 0.5f);

        TempStorageSlot slotScript = slot.GetComponent<TempStorageSlot>();

        if (slotScript != null)
        {
            slotScript.SetItem(item);
        }
    }

    public void RemoveFromStorage(InventoryItem item)
    {
        if (item == null)
            return;

        if (item.tempStorageIndex < 0)
            return;

        int index = item.tempStorageIndex;

        if (index >= 0 && index < storedItems.Length)
        {
            storedItems[index] = null;
        }

        if (slotContainer != null && index >= 0 && index < slotContainer.childCount)
        {
            Transform slot = slotContainer.GetChild(index);
            TempStorageSlot slotScript = slot.GetComponent<TempStorageSlot>();

            if (slotScript != null)
            {
                slotScript.ClearItem();
            }
        }

        item.isInTempStorage = false;
        item.tempStorageIndex = -1;

        if (item.view != null && Inventory.Instance != null)
        {
            RectTransform itemRect = item.view.GetComponent<RectTransform>();

            if (itemRect != null)
            {
                itemRect.SetParent(Inventory.Instance.itemContainer, false);
                itemRect.anchorMin = new Vector2(0, 1);
                itemRect.anchorMax = new Vector2(0, 1);
                itemRect.pivot = new Vector2(0, 1);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItemView itemView =
            eventData.pointerDrag?.GetComponent<InventoryItemView>();

        if (itemView == null)
            return;

        if (!HasSpace())
        {
            Debug.Log("Temp storage penuh!");
            return;
        }

        InventoryItem item = itemView.item;

        if (item == null)
            return;

        itemView.isStoredToTemp = true;

        bool stored = StoreItem(item);

        if (!stored)
        {
            itemView.isStoredToTemp = false;
        }
    }
}