using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public ItemSpriteConfig spriteConfig;
    public int width = 3;
    public int height = 3;

    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public Transform slotContainer;
    public Transform itemContainer;
    private Slot[,] slots;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateGrid();
    }
    public Slot GetSlot(int x, int y)
    {
        return slots[x, y];
    }

    void GenerateGrid()
    {
        slots = new Slot[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotContainer);
                slotObj.name = $"Slot ({x},{y})";
                Slot slot = slotObj.GetComponent<Slot>();
                slot.x = x;
                slot.y = y;
                slots[x, y] = slot;
            }
        }

        // contoh pemakaian
        InventoryItem sword = new InventoryItem("Sword", ItemShape.TwoBlock, ItemTier.Bronze);
        InventoryItem potion = new InventoryItem("Potion", ItemShape.OneBlock, ItemTier.Bronze);
        InventoryItem Cangkul = new InventoryItem("Cangkul", ItemShape.LShape, ItemTier.Bronze);

        PlaceItem(sword, 0, 0);
        PlaceItem(potion, 2, 2);

        CreateItemView(sword);
        UpdateItemViewSize(sword);
        UpdateItemViewPosition(sword);
        CreateItemView(potion);
        UpdateItemViewSize(potion);
        UpdateItemViewPosition(potion);
    }

    public bool CanPlaceItem(InventoryItem item, int startX, int startY)
    {
        int shapeHeight = item.shape.GetLength(0);
        int shapeWidth = item.shape.GetLength(1);

        for (int y = 0; y < shapeHeight; y++)
        {
            for (int x = 0; x < shapeWidth; x++)
            {
                if (!item.shape[y, x]) continue;

                int gridX = startX + x;
                int gridY = startY + y;

                if (gridX >= width || gridY >= height) return false;
                if (slots[gridX, gridY].isOccupied) return false;
            }
        }
        return true;
    }

    public bool PlaceItem(InventoryItem item, int startX, int startY)
    {
        if (!CanPlaceItem(item, startX, startY))
        {
            Debug.Log("Tidak bisa menaruh item!");
            return false;
        }

        item.originX = startX;
        item.originY = startY;

        int shapeHeight = item.shape.GetLength(0);
        int shapeWidth = item.shape.GetLength(1);

        for (int y = 0; y < shapeHeight; y++)
        {
            for (int x = 0; x < shapeWidth; x++)
            {
                if (!item.shape[y, x]) continue;

                int gridX = startX + x;
                int gridY = startY + y;

                Slot slot = slots[gridX, gridY];
                slot.currentItem = item;
                slot.isOccupied = true;
                slot.UpdateVisual();
            }
        }
        // Trigger upgrade check
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.CheckUpgrade();
        if (InventoryStats.Instance != null)
            InventoryStats.Instance.Recalculate();
        return true;
    }

    void CreateItemView(InventoryItem item)
    {
        GameObject obj = Instantiate(itemPrefab, itemContainer);
        RectTransform rect = obj.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        obj.name = item.itemName;

        InventoryItemView view = obj.GetComponent<InventoryItemView>();
        view.item = item;
        item.view = view;

        // Set sprite sesuai shapeType
        if (spriteConfig != null)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null)
            {
                Sprite sprite = spriteConfig.GetSprite(item.shapeType);

                if (sprite != null)
                    img.sprite = sprite;
                else
                    img.color = spriteConfig.GetColor(item.shapeType); // ← pakai warna kalau sprite null
            }
        }
    }

    // Expose untuk UpgradeSystem
    public void CreateItemViewPublic(InventoryItem item)
    {
        CreateItemView(item);
    }

    public void UpdateItemViewSize(InventoryItem item)
    {
        if (item.view == null) return;

        RectTransform rect = item.view.GetComponent<RectTransform>();
        int shapeHeight = item.shape.GetLength(0);
        int shapeWidth = item.shape.GetLength(1);
        float cellSize = 150f;
        float spacing = 5f;

        rect.sizeDelta = new Vector2(
            shapeWidth * cellSize + (shapeWidth - 1) * spacing,
            shapeHeight * cellSize + (shapeHeight - 1) * spacing
        );
    }

    public void UpdateItemViewPosition(InventoryItem item)
    {
        RectTransform itemRect = item.view.GetComponent<RectTransform>();
        float cellSize = 150f;
        float spacing = 5f;

        float posX = item.originX * (cellSize + spacing);
        float posY = -(item.originY * (cellSize + spacing));

        itemRect.anchoredPosition = new Vector2(posX, posY);
    }

    public void RemoveItem(InventoryItem item)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Slot slot = slots[x, y];
                if (slot.currentItem == item)
                {
                    slot.currentItem = null;
                    slot.isOccupied = false;
                    slot.UpdateVisual();
                }
            }
        }
        if (InventoryStats.Instance != null)
            InventoryStats.Instance.Recalculate();
    }

    public Vector2Int? GetGridPositionFromPointer(PointerEventData eventData)
    {
        RectTransform containerRect = slotContainer.GetComponent<RectTransform>();

        Vector2 localPoint;
        bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        if (!hit) return null;

        float cellSize = 150f;
        float spacing = 5f;
        float step = cellSize + spacing;

        float offsetX = containerRect.rect.width * 0.5f;
        float offsetY = containerRect.rect.height * 0.5f;

        float adjustedX = localPoint.x + offsetX;
        float adjustedY = -localPoint.y + offsetY;

        int gridX = Mathf.FloorToInt(adjustedX / step);
        int gridY = Mathf.FloorToInt(adjustedY / step);

        if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
            return null;

        return new Vector2Int(gridX, gridY);
    }

    public void HighlightSlots(InventoryItem item, int startX, int startY)
    {
        ClearHighlight();

        int shapeHeight = item.shape.GetLength(0);
        int shapeWidth = item.shape.GetLength(1);
        bool canPlace = CanPlaceItem(item, startX, startY);

        for (int y = 0; y < shapeHeight; y++)
        {
            for (int x = 0; x < shapeWidth; x++)
            {
                if (!item.shape[y, x]) continue;

                int gridX = startX + x;
                int gridY = startY + y;

                if (gridX >= width || gridY >= height) continue;

                slots[gridX, gridY].SetHighlight(canPlace);
            }
        }
    }

    public void ClearHighlight()
    {
        foreach (Slot slot in slots)
            slot.ClearHighlight();
    }

    public void DebugGrid()
    {
        string output = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                output += slots[x, y].isOccupied ? "[X]" : "[ ]";
            output += "\n";
        }
        Debug.Log(output);
    }
}