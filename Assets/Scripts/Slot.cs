using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int x, y;
    public bool isOccupied;
    public InventoryItem currentItem;

    private Image image;

    private Color normalColor    = Color.white;
    private Color occupiedColor  = Color.red;
    private Color validColor     = new Color(0f, 1f, 0f, 0.4f);   // hijau
    private Color invalidColor   = new Color(1f, 0f, 0f, 0.4f);   // merah

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void UpdateVisual()
    {
        image.color = isOccupied ? occupiedColor : normalColor;
    }

    public void SetHighlight(bool valid)
    {
        image.color = valid ? validColor : invalidColor;
    }

    public void ClearHighlight()
    {
        UpdateVisual();
    }
}