using UnityEngine;

<<<<<<< Updated upstream
public enum ItemTier
{
    Bronze, // Tier 1
    Silver, // Tier 2
    Gold    // Tier 3
}

public enum ItemShape
{
    OneBlock,
    TwoBlock,
    LShape
}

public enum ItemBlockType
{
    BlockL,
    BlockIPlus,
    BlockIMinus,
    BlockDot
}
=======
public enum ItemTier { Bronze, Silver, Gold }
public enum ItemShape { OneBlock, TwoBlock, TwoBlockV, LShape }
>>>>>>> Stashed changes

public class InventoryItem
{
    public InventoryItemView view;

    public string itemName;
    public bool[,] shape;
    public int originX;
    public int originY;
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    public bool isInTempStorage = false;
    public int tempStorageIndex = -1;

    public ItemTier tier;
    public ItemShape shapeType;
<<<<<<< Updated upstream
    public ItemBlockType blockType;

    public HMItemStats stats;

    public int score => tier switch
    {
        ItemTier.Bronze => 10,
        ItemTier.Silver => 30,
        ItemTier.Gold   => 90,
        _               => 0
    };

=======

    // Stats
    public int damage;
    public float fireRate;
    public int hpTurret;
    public int hm;

>>>>>>> Stashed changes
    public InventoryItem(string itemName, bool[,] shape)
    {
        this.itemName = itemName;
        this.shape = shape;

        this.blockType = ItemBlockType.BlockDot;
        this.shapeType = ItemShape.OneBlock;
        this.tier = ItemTier.Bronze;
        this.stats = HMItemDatabase.GetStats(blockType, tier);
    }

    public InventoryItem(string itemName, ItemShape shapeType, ItemTier tier = ItemTier.Bronze)
    {
        this.itemName = itemName;
        this.shapeType = shapeType;
        this.tier = tier;
<<<<<<< Updated upstream

        this.blockType = GetDefaultBlockTypeFromShape(shapeType);
        this.shape = GetShapeFromType(shapeType);
        this.stats = HMItemDatabase.GetStats(blockType, tier);
    }

    public InventoryItem(string itemName, ItemBlockType blockType, ItemTier tier = ItemTier.Bronze)
    {
        this.itemName = itemName;
        this.blockType = blockType;
        this.tier = tier;

        this.shapeType = GetShapeFromBlockType(blockType);
        this.shape = GetShapeFromType(shapeType);
        this.stats = HMItemDatabase.GetStats(blockType, tier);
    }

    public static ItemShape GetShapeFromBlockType(ItemBlockType blockType)
    {
        return blockType switch
        {
            ItemBlockType.BlockL      => ItemShape.LShape,
            ItemBlockType.BlockIPlus  => ItemShape.TwoBlock,
            ItemBlockType.BlockIMinus => ItemShape.TwoBlock,
            ItemBlockType.BlockDot    => ItemShape.OneBlock,
            _                         => ItemShape.OneBlock
        };
    }

    public static ItemBlockType GetDefaultBlockTypeFromShape(ItemShape shapeType)
    {
        return shapeType switch
        {
            ItemShape.LShape   => ItemBlockType.BlockL,
            ItemShape.TwoBlock => ItemBlockType.BlockIPlus,
            ItemShape.OneBlock => ItemBlockType.BlockDot,
            _                  => ItemBlockType.BlockDot
        };
=======
        this.shape = GetShapeFromType(shapeType);

        ApplyStats();
    }

    void ApplyStats()
    {
        switch (shapeType)
        {
            case ItemShape.LShape:
                switch (tier)
                {
                    case ItemTier.Bronze:
                        damage = 50; hpTurret = 150; hm = 15; fireRate = 0;
                        break;
                    case ItemTier.Silver:
                        damage = 175; hpTurret = 500; hm = 25; fireRate = 0;
                        break;
                    case ItemTier.Gold:
                        damage = 600; hpTurret = 1750; hm = 40; fireRate = 0;
                        break;
                }
                break;

            case ItemShape.TwoBlockV: // Block I (+)
                switch (tier)
                {
                    case ItemTier.Bronze:
                        damage = 30; hpTurret = 100; hm = 10; fireRate = 0;
                        break;
                    case ItemTier.Silver:
                        damage = 100; hpTurret = 350; hm = 20; fireRate = 0;
                        break;
                    case ItemTier.Gold:
                        damage = 325; hpTurret = 1150; hm = 30; fireRate = 0;
                        break;
                }
                break;

            case ItemShape.TwoBlock: // Block I (-)
                switch (tier)
                {
                    case ItemTier.Bronze:
                        fireRate = 0.5f; hpTurret = 75; hm = -10; damage = 0;
                        break;
                    case ItemTier.Silver:
                        fireRate = 1.2f; hpTurret = 250; hm = -20; damage = 0;
                        break;
                    case ItemTier.Gold:
                        fireRate = 3.0f; hpTurret = 800; hm = -35; damage = 0;
                        break;
                }
                break;

            case ItemShape.OneBlock: // Block (.)
                switch (tier)
                {
                    case ItemTier.Bronze:
                        fireRate = 0.2f; hpTurret = 50; hm = 15; damage = 0;
                        break;
                    case ItemTier.Silver:
                        fireRate = 0.6f; hpTurret = 175; hm = 30; damage = 0;
                        break;
                    case ItemTier.Gold:
                        fireRate = 1.5f; hpTurret = 600; hm = 50; damage = 0;
                        break;
                }
                break;
        }
>>>>>>> Stashed changes
    }

    public static bool[,] GetShapeFromType(ItemShape shapeType)
    {
        return shapeType switch
        {
<<<<<<< Updated upstream
            ItemShape.OneBlock => new bool[,]
            {
                { true }
            },

            ItemShape.TwoBlock => new bool[,]
            {
                { true, true }
            },
=======
            ItemShape.OneBlock => new bool[,] { { true } },
            ItemShape.TwoBlock => new bool[,] { { true, true } },
            ItemShape.TwoBlockV => new bool[,] { { true }, { true },
                                                {false},{true} },
>>>>>>> Stashed changes

            ItemShape.LShape => new bool[,]
            {
                { true, false },
                { true, true },
            },

            _ => new bool[,]
            {
                { true }
            }
        };
    }

    public void Rotate()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        bool[,] rotated = new bool[cols, rows];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                rotated[x, rows - 1 - y] = shape[y, x];
        shape = rotated;
    }

    public bool[,] CloneShape()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        bool[,] copy = new bool[rows, cols];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                copy[y, x] = shape[y, x];
        return copy;
    }

    public void DebugShape()
    {
        string output = "";
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
<<<<<<< Updated upstream
            {
                output += shape[y, x] ? "[X]" : "[ ]";
            }

=======
                output += shape[y, x] ? "[X]" : "[ ]";
>>>>>>> Stashed changes
            output += "\n";
        }
        Debug.Log(output);
    }
}