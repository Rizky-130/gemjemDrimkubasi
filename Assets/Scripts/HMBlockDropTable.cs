using UnityEngine;

public static class HMBlockDropTable
{
    public static ItemTier GetRandomTierByWave(int wave)
    {
        // Wave 1-2: only Tier 1 / Bronze
        if (wave <= 2)
        {
            return ItemTier.Bronze;
        }

        // Wave 3-5: Tier 2 starts appearing
        if (wave <= 5)
        {
            float roll = Random.Range(0f, 100f);

            if (roll < 65f)
                return ItemTier.Bronze;

            return ItemTier.Silver;
        }

        // Wave 6-8: Tier 3 starts appearing
        if (wave <= 8)
        {
            float roll = Random.Range(0f, 100f);

            if (roll < 35f)
                return ItemTier.Bronze;

            if (roll < 80f)
                return ItemTier.Silver;

            return ItemTier.Gold;
        }

        // Wave 9: pre-endgame, more Gold
        if (wave == 9)
        {
            float roll = Random.Range(0f, 100f);

            if (roll < 20f)
                return ItemTier.Bronze;

            if (roll < 65f)
                return ItemTier.Silver;

            return ItemTier.Gold;
        }

        // Wave 10: final wave, highest Gold chance
        {
            float roll = Random.Range(0f, 100f);

            if (roll < 10f)
                return ItemTier.Bronze;

            if (roll < 50f)
                return ItemTier.Silver;

            return ItemTier.Gold;
        }
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