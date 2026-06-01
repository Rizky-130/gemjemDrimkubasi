using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemView : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem item;
    private Inventory inventory;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private int savedOriginX; // ← tambah
    private int savedOriginY;
    public bool isStoredToTemp = false;
    private Vector2 originalPosition;
    public bool isTrashed = false;
    private bool[,] savedShape; // ← tambah field ini


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        inventory = FindObjectOfType<Inventory>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log($"[BeginDrag] {item.itemName} | isInTempStorage: {item.isInTempStorage}");

        originalPosition = rectTransform.anchoredPosition;
        savedOriginX = item.originX;
        savedOriginY = item.originY;
        savedShape = item.CloneShape();

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();

        if (item.isInTempStorage)
        {
            Debug.Log($"[BeginDrag] RemoveFromStorage dipanggil");
            TempStorage.Instance.RemoveFromStorage(item);
        }
        else
        {
            Debug.Log($"[BeginDrag] RemoveItem dari grid dipanggil");
            inventory.RemoveItem(item);
        }

        isTrashed = false;
        isStoredToTemp = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        // Item ikut cursor
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;

        // Cek input rotasi
        if (Input.GetKeyDown(KeyCode.R))
        {
            item.Rotate();
            inventory.UpdateItemViewSize(item); // update ukuran visual
        }

        // Highlight slot yang valid
        Vector2Int? gridPos = inventory.GetGridPositionFromPointer(eventData);
        if (gridPos.HasValue)
        {
            inventory.HighlightSlots(item, gridPos.Value.x, gridPos.Value.y);
        }
        else
        {
            inventory.ClearHighlight();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (isTrashed) return;
        if (isStoredToTemp) return;

        Debug.Log($"[EndDrag] {item.itemName} | isTrashed: {isTrashed} | isStoredToTemp: {isStoredToTemp}");

        canvasGroup.blocksRaycasts = true;
        inventory.ClearHighlight();

        Vector2Int? gridPos = inventory.GetGridPositionFromPointer(eventData);

        Debug.Log($"[EndDrag] gridPos: {gridPos}");

        bool placed = false;

        if (gridPos.HasValue)
            placed = inventory.PlaceItem(item, gridPos.Value.x, gridPos.Value.y);

        Debug.Log($"[EndDrag] placed: {placed}");

        if (placed)
        {
            inventory.UpdateItemViewSize(item);
            inventory.UpdateItemViewPosition(item);
        }
        else
        {
            item.shape = savedShape;
            bool returned = inventory.PlaceItem(item, savedOriginX, savedOriginY);

            Debug.Log($"[EndDrag] returned ke grid: {returned}");

            if (returned)
            {
                inventory.UpdateItemViewSize(item);
                rectTransform.anchoredPosition = originalPosition;
            }
            else
            {
                bool stored = TempStorage.Instance.StoreItem(item);
                Debug.Log($"[EndDrag] fallback ke TempStorage: {stored}");
            }
        }
    }

}