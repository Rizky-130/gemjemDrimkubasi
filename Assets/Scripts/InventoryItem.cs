using UnityEngine;


public enum ItemTier { Bronze, Silver, Gold }
public enum ItemShape { OneBlock, TwoBlock, LShape }
public class InventoryItem
{
    public InventoryItemView view;
    public string itemName;

    public bool[,] shape;

    public int originX;
    public int originY;
    public bool isInTempStorage = false;  // ← tambah
    public int tempStorageIndex = -1;
    public ItemTier tier;
    public ItemShape shapeType;
     public int score => tier switch
    {
        ItemTier.Bronze => 10,
        ItemTier.Silver => 30,
        ItemTier.Gold   => 90,
        _               => 0
    };

    public InventoryItem(
        string itemName,
        bool[,] shape)
    {
        this.itemName = itemName;
        this.shape = shape;
    }
     public InventoryItem(string itemName, ItemShape shapeType, ItemTier tier = ItemTier.Bronze)
    {
        this.itemName  = itemName;
        this.shapeType = shapeType;
        this.tier      = tier;
        this.shape     = GetShapeFromType(shapeType);
    }
    public static bool[,] GetShapeFromType(ItemShape shapeType)
    {
        return shapeType switch
        {
            ItemShape.OneBlock => new bool[,] { { true } },
            ItemShape.TwoBlock => new bool[,] { { true, true } },
            ItemShape.LShape   => new bool[,]
            {
                { true, false },
                { true, false },
                { true, true  }
            },
            _ => new bool[,] { { true } }
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

    public void DebugShape()
    {
        string output = "";

        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                output += shape[y, x]
                    ? "[X]"
                    : "[ ]";
            }

            output += "\n";
        }

        Debug.Log(output);
    }
}