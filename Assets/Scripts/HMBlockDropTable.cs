using UnityEngine;

public static class HMBlockDropTable
{
    public static ItemTier GetRandomTierByWave(int wave)
    {
        float bronzeChance;
        float silverChance;
        float goldChance;

        if (wave <= 2)
        {
            bronzeChance = 90f;
            silverChance = 10f;
            goldChance = 0f;
        }
        else if (wave <= 5)
        {
            bronzeChance = 70f;
            silverChance = 25f;
            goldChance = 5f;
        }
        else if (wave <= 9)
        {
            bronzeChance = 50f;
            silverChance = 40f;
            goldChance = 10f;
        }
        else if (wave <= 14)
        {
            bronzeChance = 35f;
            silverChance = 45f;
            goldChance = 20f;
        }
        else
        {
            bronzeChance = 25f;
            silverChance = 45f;
            goldChance = 30f;
        }

        float roll = Random.Range(0f, 100f);

        if (roll < bronzeChance)
        {
            return ItemTier.Bronze;
        }

        if (roll < bronzeChance + silverChance)
        {
            return ItemTier.Silver;
        }

        return ItemTier.Gold;
    }

    public static ItemBlockType GetRandomBlockType()
    {
        int randomIndex = Random.Range(0, 4);

        switch (randomIndex)
        {
            case 0:
                return ItemBlockType.BlockL;

            case 1:
                return ItemBlockType.BlockIPlus;

            case 2:
                return ItemBlockType.BlockIMinus;

            case 3:
                return ItemBlockType.BlockDot;

            default:
                return ItemBlockType.BlockDot;
        }
    }
}