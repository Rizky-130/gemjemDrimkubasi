using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int x;
    public int y;

    public bool isOccupied;
    public InventoryItem currentItem;

    private Image image;

    private Color normalColor = Color.white;
    private Color occupiedColor = Color.red;
    private Color validColor = new Color(0f, 1f, 0f, 0.4f);
    private Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private void Awake()
    {
        image = GetComponent<Image>();

        if (image != null)
        {
            normalColor = image.color;
            image.raycastTarget = true;
        }
    }

    public void UpdateVisual()
    {
        if (image == null)
            return;

        image.color = isOccupied ? occupiedColor : normalColor;
    }

    public void SetHighlight(bool valid)
    {
        if (image == null)
            return;

        image.color = valid ? validColor : invalidColor;
    }

    public void ClearHighlight()
    {
        UpdateVisual();
    }
}