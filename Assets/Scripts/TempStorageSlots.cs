using UnityEngine;
using UnityEngine.EventSystems;

public class TempStorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem storedItem;

    private InventoryItemView itemView;
    private InventoryItemView activeDragView;

    public void SetItem(InventoryItem item)
    {
        storedItem = item;
        itemView = item != null ? item.view : null;
    }

    public void ClearItem()
    {
        storedItem = null;
        itemView = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (storedItem == null)
            return;

        if (storedItem.view == null)
            return;

        InventoryItem itemToDrag = storedItem;
        activeDragView = itemToDrag.view;

        if (TempStorage.Instance != null)
        {
            TempStorage.Instance.RemoveFromStorage(itemToDrag);
        }
        else
        {
            ClearItem();
        }

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.beginDragHandler
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (activeDragView == null)
            return;

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.dragHandler
        );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (activeDragView == null)
            return;

        ExecuteEvents.Execute(
            activeDragView.gameObject,
            eventData,
            ExecuteEvents.endDragHandler
        );

        activeDragView = null;
    }
}