using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    [Header("Upgrade Settings")]
    public int requiredAmount = 3;
    public bool allowUpgradeFromTempStorage = true;
    public bool allowChainUpgrade = true;

    private bool isUpgrading = false;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckUpgrade()
    {
        if (isUpgrading)
            return;

        if (Inventory.Instance == null)
        {
            Debug.LogWarning("UpgradeSystem cannot find Inventory.Instance.");
            return;
        }

        isUpgrading = true;

        bool upgradedSomething = false;

        foreach (ItemBlockType blockType in System.Enum.GetValues(typeof(ItemBlockType)))
        {
            foreach (ItemTier tier in System.Enum.GetValues(typeof(ItemTier)))
            {
                if (tier == ItemTier.Gold)
                    continue;

                List<InventoryItem> matches = FindMatchingItems(blockType, tier);

                if (matches.Count >= requiredAmount)
                {
                    DoUpgrade(matches, blockType, tier);
                    upgradedSomething = true;
                    break;
                }
            }

            if (upgradedSomething)
                break;
        }

        isUpgrading = false;

        if (upgradedSomething && allowChainUpgrade)
        {
            CheckUpgrade();
        }
    }

    private List<InventoryItem> FindMatchingItems(ItemBlockType blockType, ItemTier tier)
    {
        List<InventoryItem> result = new List<InventoryItem>();
        HashSet<InventoryItem> seen = new HashSet<InventoryItem>();

        // Check main inventory grid
        for (int y = 0; y < Inventory.Instance.height; y++)
        {
            for (int x = 0; x < Inventory.Instance.width; x++)
            {
                Slot slot = Inventory.Instance.GetSlot(x, y);

                if (slot == null)
                    continue;

                if (slot.currentItem == null)
                    continue;

                InventoryItem item = slot.currentItem;

                if (seen.Contains(item))
                    continue;

                seen.Add(item);

                if (item.blockType == blockType && item.tier == tier)
                {
                    result.Add(item);
                }
            }
        }

        // Check temp storage
        if (allowUpgradeFromTempStorage && TempStorage.Instance != null)
        {
            for (int i = 0; i < TempStorage.Instance.capacity; i++)
            {
                InventoryItem item = TempStorage.Instance.GetStoredItem(i);

                if (item == null)
                    continue;

                if (seen.Contains(item))
                    continue;

                seen.Add(item);

                if (item.blockType == blockType && item.tier == tier)
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    private void DoUpgrade(List<InventoryItem> items, ItemBlockType blockType, ItemTier tier)
    {
        if (items == null || items.Count < requiredAmount)
            return;

        ItemTier newTier = GetNextTier(tier);

        int spawnX = 0;
        int spawnY = 0;
        bool spawnFromGrid = false;

        // Try to spawn upgraded item where one old grid item was
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            if (item == null)
                continue;

            if (!item.isInTempStorage)
            {
                spawnX = item.originX;
                spawnY = item.originY;
                spawnFromGrid = true;
                break;
            }
        }

        // Remove old items
        for (int i = 0; i < requiredAmount; i++)
        {
            InventoryItem item = items[i];

            if (item == null)
                continue;

            if (item.isInTempStorage)
            {
                if (TempStorage.Instance != null)
                {
                    TempStorage.Instance.RemoveFromStorage(item);
                }
            }
            else
            {
                Inventory.Instance.RemoveItem(item);
            }

            if (item.view != null)
            {
                Destroy(item.view.gameObject);
            }
        }

        string upgradedName = HMItemDatabase.GetDisplayName(blockType, newTier);

        InventoryItem upgradedItem = new InventoryItem(
            upgradedName,
            blockType,
            newTier
        );

        bool placed = false;

        if (spawnFromGrid)
        {
            placed = Inventory.Instance.PlaceItem(upgradedItem, spawnX, spawnY);
        }

        if (!placed)
        {
            placed = TryPlaceAnywhere(upgradedItem);
        }

        if (placed)
        {
            Inventory.Instance.CreateItemViewPublic(upgradedItem);
            Inventory.Instance.UpdateItemViewSize(upgradedItem);
            Inventory.Instance.UpdateItemViewPosition(upgradedItem);
        }
        else
        {
            if (TempStorage.Instance != null)
            {
                TempStorage.Instance.SpawnItemToTempStorage(upgradedItem);
            }
            else
            {
                Debug.LogWarning("Upgrade result could not be placed. TempStorage.Instance missing.");
            }
        }

        Debug.Log(
            "Upgrade! " +
            requiredAmount + "x " +
            tier + " " +
            blockType +
            " -> " +
            newTier + " " +
            blockType
        );

        RecalculateStats();
    }

    private bool TryPlaceAnywhere(InventoryItem item)
    {
        for (int y = 0; y < Inventory.Instance.height; y++)
        {
            for (int x = 0; x < Inventory.Instance.width; x++)
            {
                if (Inventory.Instance.PlaceItem(item, x, y))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private ItemTier GetNextTier(ItemTier tier)
    {
        if (tier == ItemTier.Bronze)
            return ItemTier.Silver;

        if (tier == ItemTier.Silver)
            return ItemTier.Gold;

        return ItemTier.Gold;
    }

    private void RecalculateStats()
    {
        if (InventoryStats.Instance != null)
        {
            InventoryStats.Instance.Recalculate();
        }
    }
}