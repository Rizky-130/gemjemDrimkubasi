using UnityEngine;

public class EnemyBlockDropper : MonoBehaviour
{
    [System.Serializable]
    public class BlockDropOption
    {
        public ItemBlockType blockType;
        public float weight = 1f;
    }

    [Header("Drop Settings")]
    public float dropChance = 1f;

    [Header("Possible Drops")]
    public BlockDropOption[] possibleDrops;

    private WaveSpawner waveSpawner;

    public void Setup(WaveSpawner spawner)
    {
        waveSpawner = spawner;
    }

    public void DropBlock()
    {
        if (TempStorage.Instance == null)
        {
            Debug.LogWarning("No TempStorage found in scene!");
            return;
        }

        if (!TempStorage.Instance.HasSpace())
        {
            Debug.Log("TempStorage is full. Drop was lost.");
            return;
        }

        if (Random.value > dropChance)
        {
            Debug.Log("Enemy did not drop item.");
            return;
        }

        int currentWave = 1;

        if (waveSpawner != null)
            currentWave = waveSpawner.currWave;

        ItemBlockType selectedBlock = GetRandomBlockType();
        ItemTier selectedTier = GetTierByWave(currentWave);

        InventoryItem item = InventoryItem.CreateLootBlock(selectedBlock, selectedTier);

        bool stored = TempStorage.Instance.SpawnItemToTempStorage(item);

        if (stored)
        {
            Debug.Log(
                "Dropped " + item.itemName +
                " | Damage +" + item.damageBonus +
                " | FireRate +" + item.fireRateBonus +
                " | HP +" + item.hpBonus +
                " | HM " + item.hmValue
            );
        }
    }

    private ItemBlockType GetRandomBlockType()
    {
        if (possibleDrops == null || possibleDrops.Length <= 0)
        {
            return ItemBlockType.LBlock;
        }

        float totalWeight = 0f;

        for (int i = 0; i < possibleDrops.Length; i++)
        {
            totalWeight += possibleDrops[i].weight;
        }

        float roll = Random.Range(0f, totalWeight);

        for (int i = 0; i < possibleDrops.Length; i++)
        {
            if (roll < possibleDrops[i].weight)
            {
                return possibleDrops[i].blockType;
            }

            roll -= possibleDrops[i].weight;
        }

        return possibleDrops[0].blockType;
    }

    private ItemTier GetTierByWave(int wave)
    {
        float roll = Random.value;

        // Wave 1-2: Tier 1 only
        if (wave < 3)
        {
            return ItemTier.Bronze;
        }

        // Wave 3-4: Tier 2 starts appearing
        if (wave < 5)
        {
            if (roll < 0.80f) return ItemTier.Bronze;
            return ItemTier.Silver;
        }

        // Wave 5-7: Tier 2 common, Tier 3 starts appearing
        if (wave < 8)
        {
            if (roll < 0.35f) return ItemTier.Bronze;
            if (roll < 0.85f) return ItemTier.Silver;
            return ItemTier.Gold;
        }

        // Wave 8+: Tier 3 becomes common
        if (roll < 0.15f) return ItemTier.Bronze;
        if (roll < 0.55f) return ItemTier.Silver;
        return ItemTier.Gold;
    }
}