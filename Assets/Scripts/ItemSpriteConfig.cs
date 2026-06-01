using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemSpriteConfig", menuName = "Inventory/ItemSpriteConfig")]
public class ItemSpriteConfig : ScriptableObject
{
    public Sprite oneBlockSprite;
    public Sprite twoBlockSprite;
    public Sprite lShapeSprite;

    // Warna placeholder sementara
    public Color oneBlockColor = new Color(0.2f, 0.6f, 1f);   // biru
    public Color twoBlockColor = new Color(0.2f, 0.8f, 0.2f); // hijau
    public Color lShapeColor   = new Color(1f, 0.5f, 0.1f);   // oranye

    public Sprite GetSprite(ItemShape shapeType)
    {
        return shapeType switch
        {
            ItemShape.OneBlock => oneBlockSprite,
            ItemShape.TwoBlock => twoBlockSprite,
            ItemShape.LShape   => lShapeSprite,
            _                  => null
        };
    }

    public Color GetColor(ItemShape shapeType)
    {
        return shapeType switch
        {
            ItemShape.OneBlock => oneBlockColor,
            ItemShape.TwoBlock => twoBlockColor,
            ItemShape.LShape   => lShapeColor,
            _                  => Color.white
        };
    }
}