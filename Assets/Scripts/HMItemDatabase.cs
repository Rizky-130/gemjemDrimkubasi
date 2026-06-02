using UnityEngine;

[System.Serializable]
public struct HMItemStats
{
    public int damageBonus;
    public float fireRateBonus;
    public int hpBonus;
    public int hmBonus;

    public HMItemStats(int damageBonus, float fireRateBonus, int hpBonus, int hmBonus)
    {
        this.damageBonus = damageBonus;
        this.fireRateBonus = fireRateBonus;
        this.hpBonus = hpBonus;
        this.hmBonus = hmBonus;
    }
}

public static class HMItemDatabase
{
    public static HMItemStats GetStats(ItemBlockType blockType, ItemTier tier)
    {
        switch (blockType)
        {
            case ItemBlockType.BlockL:
                return GetBlockLStats(tier);

            case ItemBlockType.BlockIPlus:
                return GetBlockIPlusStats(tier);

            case ItemBlockType.BlockIMinus:
                return GetBlockIMinusStats(tier);

            case ItemBlockType.BlockDot:
                return GetBlockDotStats(tier);

            default:
                return new HMItemStats(0, 0f, 0, 0);
        }
    }

    private static HMItemStats GetBlockLStats(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Bronze => new HMItemStats(50, 0f, 150, 15),
            ItemTier.Silver => new HMItemStats(175, 0f, 500, 25),
            ItemTier.Gold   => new HMItemStats(600, 0f, 1750, 40),
            _               => new HMItemStats(0, 0f, 0, 0)
        };
    }

    private static HMItemStats GetBlockIPlusStats(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Bronze => new HMItemStats(30, 0f, 100, 10),
            ItemTier.Silver => new HMItemStats(100, 0f, 350, 20),
            ItemTier.Gold   => new HMItemStats(325, 0f, 1150, 30),
            _               => new HMItemStats(0, 0f, 0, 0)
        };
    }

    private static HMItemStats GetBlockIMinusStats(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Bronze => new HMItemStats(0, 0.5f, 75, -10),
            ItemTier.Silver => new HMItemStats(0, 1.2f, 250, -20),
            ItemTier.Gold   => new HMItemStats(0, 3.0f, 800, -35),
            _               => new HMItemStats(0, 0f, 0, 0)
        };
    }

    private static HMItemStats GetBlockDotStats(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Bronze => new HMItemStats(0, 0.2f, 50, -15),
            ItemTier.Silver => new HMItemStats(0, 0.6f, 175, -30),
            ItemTier.Gold   => new HMItemStats(0, 1.5f, 600, -50),
            _               => new HMItemStats(0, 0f, 0, 0)
        };
    }

    public static string GetDisplayName(ItemBlockType blockType, ItemTier tier)
    {
        string tierName = tier.ToString();

        string blockName = blockType switch
        {
            ItemBlockType.BlockL      => "Block L",
            ItemBlockType.BlockIPlus  => "Block I+",
            ItemBlockType.BlockIMinus => "Block I-",
            ItemBlockType.BlockDot    => "Block Dot",
            _                         => "Unknown Block"
        };

        return tierName + " " + blockName;
    }
}