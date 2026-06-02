using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpriteConfig", menuName = "Inventory/ItemSpriteConfig")]
public class ItemSpriteConfig : ScriptableObject
{
    [Header("Shape Sprites")]
    public Sprite oneBlockSprite;
    public Sprite twoBlockSprite;
    public Sprite twoBlockVSprite; 
    public Sprite lShapeSprite;

<<<<<<< Updated upstream
    [Header("Block Type Colors")]
    public Color blockLColor = new Color(1f, 0.5f, 0.1f);       // Orange
    public Color blockIPlusColor = new Color(0.2f, 0.8f, 0.2f); // Green
    public Color blockIMinusColor = new Color(0.8f, 0.2f, 1f);  // Purple
    public Color blockDotColor = new Color(0.2f, 0.6f, 1f);     // Blue

    [Header("Tier Brightness")]
    [Range(0.5f, 2f)] public float bronzeBrightness = 0.8f;
    [Range(0.5f, 2f)] public float silverBrightness = 1.1f;
    [Range(0.5f, 2f)] public float goldBrightness = 1.5f;
=======
    public Color oneBlockColor = new Color(0.2f, 0.6f, 1f);
    public Color twoBlockColor = new Color(0.2f, 0.8f, 0.2f);
    public Color twoBlockVColor = new Color(0.8f, 0.2f, 0.8f); 
    public Color lShapeColor = new Color(1f, 0.5f, 0.1f);
>>>>>>> Stashed changes

    public Sprite GetSprite(ItemShape shapeType)
    {
        return shapeType switch
        {
            ItemShape.OneBlock => oneBlockSprite,
            ItemShape.TwoBlock => twoBlockSprite,
<<<<<<< Updated upstream
=======
            ItemShape.TwoBlockV => twoBlockVSprite, 
>>>>>>> Stashed changes
            ItemShape.LShape => lShapeSprite,
            _ => null
        };
    }

    public Color GetColor(ItemBlockType blockType, ItemTier tier)
    {
        Color baseColor = blockType switch
        {
            ItemBlockType.BlockL => blockLColor,
            ItemBlockType.BlockIPlus => blockIPlusColor,
            ItemBlockType.BlockIMinus => blockIMinusColor,
            ItemBlockType.BlockDot => blockDotColor,
            _ => Color.white
        };

        float brightness = tier switch
        {
            ItemTier.Bronze => bronzeBrightness,
            ItemTier.Silver => silverBrightness,
            ItemTier.Gold => goldBrightness,
            _ => 1f
        };

        return baseColor * brightness;
    }

    // Keep this so older scripts still work
    public Color GetColor(ItemShape shapeType)
    {
        return shapeType switch
        {
<<<<<<< Updated upstream
            ItemShape.OneBlock => blockDotColor,
            ItemShape.TwoBlock => blockIPlusColor,
            ItemShape.LShape => blockLColor,
=======
            ItemShape.OneBlock => oneBlockColor,
            ItemShape.TwoBlock => twoBlockColor,
            ItemShape.TwoBlockV => twoBlockVColor, 
            ItemShape.LShape => lShapeColor,
>>>>>>> Stashed changes
            _ => Color.white
        };
    }
}