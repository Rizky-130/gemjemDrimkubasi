using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashCan : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Color normalColor  = new Color(1f, 1f, 1f, 0.5f);
    private Color hoverColor   = new Color(1f, 0.3f, 0.3f, 1f); // merah saat item di-drag ke sini

    void Awake()
    {
        image = GetComponent<Image>();
        image.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hanya highlight kalau sedang drag item
        if (eventData.dragging)
            image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        image.color = normalColor;

        // Ambil InventoryItemView dari object yang di-drag
        InventoryItemView itemView =
            eventData.pointerDrag?.GetComponent<InventoryItemView>();

        if (itemView == null) return;

        InventoryItem item = itemView.item;

        // Tandai item sudah di-trash agar OnEndDrag tidak place ulang
        itemView.isTrashed = true;

        // Hapus dari grid (sudah di-remove saat OnBeginDrag)
        // Destroy view
        Destroy(itemView.gameObject);

        Debug.Log($"{item.itemName} dibuang ke trash!");
    }
}