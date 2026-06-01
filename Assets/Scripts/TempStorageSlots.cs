using UnityEngine;
using UnityEngine.EventSystems;

public class TempStorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem storedItem;
    private InventoryItemView itemView;
    private InventoryItemView activeDragView; // ← tambah ini

    public void SetItem(InventoryItem item)
    {
        storedItem = item;
        itemView = item?.view;
    }

    public void ClearItem()
    {
        storedItem = null;
        itemView = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (itemView == null) return;

        // Simpan reference SEBELUM RemoveFromStorage mengubah parent
        activeDragView = itemView;
        ClearItem(); // clear slot sekarang

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (activeDragView == null) return;

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.dragHandler);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (activeDragView == null) return;

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.endDragHandler);

        activeDragView = null; // reset setelah selesai
    }
}