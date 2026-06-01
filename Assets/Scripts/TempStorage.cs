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

    void Awake()
    {
        Instance = this;
        storedItems = new InventoryItem[capacity];
    }

    void Start()
    {
        GenerateSlots();
    }

    void GenerateSlots()
    {
        for (int i = 0; i < capacity; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            slotObj.name = $"TempSlot ({i})";

            Image slotImage = slotObj.GetComponent<Image>();
            if (slotImage != null)
                slotImage.raycastTarget = true;

            // Tambah TempStorageSlot script
            slotObj.AddComponent<TempStorageSlot>();
        }
    }


    public bool HasSpace()
    {
        for (int i = 0; i < capacity; i++)
            if (storedItems[i] == null) return true;
        return false;
    }
    // Ini yang dipanggil saat enemy drop item
    public bool SpawnItemToTempStorage(InventoryItem item)
    {
        if (!HasSpace())
        {
            Debug.Log("Temp storage penuh!");
            return false;
        }

        // Buat view
        Inventory.Instance.CreateItemViewPublic(item);
        Inventory.Instance.UpdateItemViewSize(item);

        // Simpan ke slot
        bool stored = StoreItem(item);
        return stored;
    }
    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < capacity; i++)
            if (storedItems[i] == null) return i;
        return -1;
    }

    public bool StoreItem(InventoryItem item)
    {
        int index = GetEmptySlotIndex();

        if (index == -1)
        {
            Debug.Log("Temp storage penuh!");
            return false;
        }

        storedItems[index] = item;
        item.tempStorageIndex = index;
        item.isInTempStorage = true;

        // Pindahkan view item ke dalam slot
        MoveViewToSlot(item, index);

        Debug.Log($"{item.itemName} disimpan di TempStorage slot {index}");
        return true;
    }

    void MoveViewToSlot(InventoryItem item, int index)
    {
        if (item.view == null) return;

        Transform slot = slotContainer.GetChild(index);
        RectTransform itemRect = item.view.GetComponent<RectTransform>();

        itemRect.SetParent(slot, false);
        itemRect.SetAsLastSibling();

        itemRect.anchorMin = Vector2.zero;
        itemRect.anchorMax = Vector2.one;
        itemRect.offsetMin = Vector2.zero;
        itemRect.offsetMax = Vector2.zero;
        itemRect.pivot = new Vector2(0.5f, 0.5f);

        // Daftarkan item ke slot script
        TempStorageSlot slotScript = slot.GetComponent<TempStorageSlot>();
        if (slotScript != null)
            slotScript.SetItem(item);
    }


    public void RemoveFromStorage(InventoryItem item)
    {
        if (item.tempStorageIndex < 0) return;

        int index = item.tempStorageIndex;

        // Clear slot script
        Transform slot = slotContainer.GetChild(index);
        TempStorageSlot slotScript = slot.GetComponent<TempStorageSlot>();
        if (slotScript != null)
            slotScript.ClearItem();

        storedItems[index] = null;
        item.isInTempStorage = false;
        item.tempStorageIndex = -1;

        if (item.view != null)
        {
            RectTransform itemRect = item.view.GetComponent<RectTransform>();
            itemRect.SetParent(Inventory.Instance.itemContainer, false);
            itemRect.anchorMin = new Vector2(0, 1);
            itemRect.anchorMax = new Vector2(0, 1);
            itemRect.pivot = new Vector2(0, 1);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItemView itemView =
            eventData.pointerDrag?.GetComponent<InventoryItemView>();

        if (itemView == null) return;
        if (!HasSpace())
        {
            Debug.Log("Temp storage penuh!");
            return;
        }

        InventoryItem item = itemView.item;
        itemView.isStoredToTemp = true;

        bool stored = StoreItem(item);

        if (!stored)
            itemView.isStoredToTemp = false;
    }
}