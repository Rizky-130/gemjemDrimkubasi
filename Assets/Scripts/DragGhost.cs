using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour
{
    public static DragGhost Instance;

    private RectTransform rectTransform;
    private Image image;
    private Canvas canvas;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        Hide();
    }

    void Update()
    {
        if (!image.enabled) return;

        // Ikuti cursor
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out localPoint
        );
        rectTransform.anchoredPosition = localPoint;
    }

    public void Show(Sprite sprite, Vector2 size)
    {
        image.sprite = sprite;
        image.enabled = true;
        rectTransform.sizeDelta = size;
        transform.SetAsLastSibling(); // selalu di atas
    }

    public void Hide()
    {
        image.enabled = false;
    }
}