using UnityEngine;

public enum ItemTier
{
    Bronze,
    Silver,
    Gold
}

public enum ItemShape
{
    OneBlock,
    TwoBlock,
    LShape
}

public enum ItemBlockType
{
    LBlock,
    IPositive,
    INegative,
    DotBlock
}

public class InventoryItem
{
    public InventoryItemView view;
    public string itemName;

    public bool[,] shape;

    public int originX;
    public int originY;

    public bool isInTempStorage = false;
    public int tempStorageIndex = -1;

    public ItemTier tier;
    public ItemShape shapeType;
    public ItemBlockType blockType;

    public int damageBonus;
    public float fireRateBonus;
    public int hpBonus;
    public int hmValue;

    public InventoryItem(string itemName, ItemShape shapeType, ItemTier tier, ItemBlockType blockType)
    {
        this.itemName = itemName;
        this.shapeType = shapeType;
        this.tier = tier;
        this.blockType = blockType;
        this.shape = GetShapeFromType(shapeType);

        ApplyStats();
    }

    public static InventoryItem CreateLootBlock(ItemBlockType blockType, ItemTier tier)
    {
        ItemShape shape = GetShapeFromBlockType(blockType);
        string name = GetItemName(blockType, tier);

        return new InventoryItem(name, shape, tier, blockType);
    }

    private static string GetItemName(ItemBlockType blockType, ItemTier tier)
    {
        string tierName = "Tier 1";

        if (tier == ItemTier.Silver)
            tierName = "Tier 2";
        else if (tier == ItemTier.Gold)
            tierName = "Tier 3";

        if (blockType == ItemBlockType.LBlock)
            return "Block L " + tierName;

        if (blockType == ItemBlockType.IPositive)
            return "Block I+ " + tierName;

        if (blockType == ItemBlockType.INegative)
            return "Block I- " + tierName;

        return "Block Dot " + tierName;
    }

    private static ItemShape GetShapeFromBlockType(ItemBlockType blockType)
    {
        if (blockType == ItemBlockType.LBlock)
            return ItemShape.LShape;

        if (blockType == ItemBlockType.IPositive)
            return ItemShape.TwoBlock;

        if (blockType == ItemBlockType.INegative)
            return ItemShape.TwoBlock;

        return ItemShape.OneBlock;
    }

    private void ApplyStats()
    {
        damageBonus = 0;
        fireRateBonus = 0f;
        hpBonus = 0;
        hmValue = 0;

        if (blockType == ItemBlockType.LBlock)
        {
            if (tier == ItemTier.Bronze)
            {
                damageBonus = 50;
                hpBonus = 150;
                hmValue = 15;
            }
            else if (tier == ItemTier.Silver)
            {
                damageBonus = 175;
                hpBonus = 500;
                hmValue = 25;
            }
            else
            {
                damageBonus = 600;
                hpBonus = 1750;
                hmValue = 40;
            }
        }
        else if (blockType == ItemBlockType.IPositive)
        {
            if (tier == ItemTier.Bronze)
            {
                damageBonus = 30;
                hpBonus = 100;
                hmValue = 10;
            }
            else if (tier == ItemTier.Silver)
            {
                damageBonus = 100;
                hpBonus = 350;
                hmValue = 20;
            }
            else
            {
                damageBonus = 325;
                hpBonus = 1150;
                hmValue = 30;
            }
        }
        else if (blockType == ItemBlockType.INegative)
        {
            if (tier == ItemTier.Bronze)
            {
                fireRateBonus = 0.5f;
                hpBonus = 75;
                hmValue = -10;
            }
            else if (tier == ItemTier.Silver)
            {
                fireRateBonus = 1.2f;
                hpBonus = 250;
                hmValue = -20;
            }
            else
            {
                fireRateBonus = 3.0f;
                hpBonus = 800;
                hmValue = -35;
            }
        }
        else if (blockType == ItemBlockType.DotBlock)
        {
            if (tier == ItemTier.Bronze)
            {
                fireRateBonus = 0.2f;
                hpBonus = 50;
                hmValue = -15;
            }
            else if (tier == ItemTier.Silver)
            {
                fireRateBonus = 0.6f;
                hpBonus = 175;
                hmValue = -30;
            }
            else
            {
                fireRateBonus = 1.5f;
                hpBonus = 600;
                hmValue = -50;
            }
        }
    }

    public static bool[,] GetShapeFromType(ItemShape shapeType)
    {
        if (shapeType == ItemShape.OneBlock)
        {
            return new bool[,]
            {
                { true }
            };
        }

        if (shapeType == ItemShape.TwoBlock)
        {
            return new bool[,]
            {
                { true, true }
            };
        }

        if (shapeType == ItemShape.LShape)
        {
            return new bool[,]
            {
                { true, false },
                { true, false },
                { true, true  }
            };
        }

        return new bool[,]
        {
            { true }
        };
    }

    public void Rotate()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        bool[,] rotated = new bool[cols, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                rotated[x, rows - 1 - y] = shape[y, x];
            }
        }

        shape = rotated;
    }

    public bool[,] CloneShape()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        bool[,] copy = new bool[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                copy[y, x] = shape[y, x];
            }
        }

        return copy;
    }
}