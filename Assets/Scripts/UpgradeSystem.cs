using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CheckUpgrade()
    {
        foreach (ItemShape shapeType in System.Enum.GetValues(typeof(ItemShape)))
        {
            foreach (ItemTier tier in System.Enum.GetValues(typeof(ItemTier)))
            {
                if (tier == ItemTier.Gold) continue;

                List<InventoryItem> matches = FindMatchingItems(shapeType, tier);

                if (matches.Count >= 3)
                {
                    DoUpgrade(matches, shapeType, tier);
                    return;
                }
            }
        }
    }

    List<InventoryItem> FindMatchingItems(ItemShape shapeType, ItemTier tier)
    {
        List<InventoryItem> result = new List<InventoryItem>();
        List<InventoryItem> seen   = new List<InventoryItem>();

        // Cek grid utama
        for (int y = 0; y < Inventory.Instance.height; y++)
        {
            for (int x = 0; x < Inventory.Instance.width; x++)
            {
                Slot slot = Inventory.Instance.GetSlot(x, y);
                if (slot.currentItem == null) continue;
                if (seen.Contains(slot.currentItem)) continue;

                InventoryItem item = slot.currentItem;

                if (item.shapeType == shapeType && item.tier == tier)
                {
                    result.Add(item);
                    seen.Add(item);
                }
            }
        }

        // Cek TempStorage
        for (int i = 0; i < TempStorage.Instance.capacity; i++)
        {
            InventoryItem item = TempStorage.Instance.GetStoredItem(i);
            if (item == null) continue;
            if (seen.Contains(item)) continue;

            if (item.shapeType == shapeType && item.tier == tier)
            {
                result.Add(item);
                seen.Add(item);
            }
        }

        return result;
    }

    void DoUpgrade(List<InventoryItem> items, ItemShape shapeType, ItemTier tier)
    {
        // Simpan posisi spawn dari item pertama yang ada di grid
        int spawnX = 0;
        int spawnY = 0;
        bool spawnFromGrid = false;

        foreach (InventoryItem item in items)
        {
            if (!item.isInTempStorage)
            {
                spawnX = item.originX;
                spawnY = item.originY;
                spawnFromGrid = true;
                break;
            }
        }

        // Hapus 3 item lama
        for (int i = 0; i < 3; i++)
        {
            InventoryItem item = items[i];

            if (item.isInTempStorage)
                TempStorage.Instance.RemoveFromStorage(item);
            else
                Inventory.Instance.RemoveItem(item);

            if (item.view != null)
                Object.Destroy(item.view.gameObject);
        }

        // Buat item hasil upgrade
        ItemTier newTier = tier + 1;
        InventoryItem upgraded = new InventoryItem(
            $"{newTier} {shapeType}",
            shapeType,
            newTier
        );

        // Coba place di posisi spawn
        bool placed = false;

        if (spawnFromGrid)
            placed = Inventory.Instance.PlaceItem(upgraded, spawnX, spawnY);

        // Kalau tidak muat, cari slot kosong lain
        if (!placed)
            placed = TryPlaceAnywhere(upgraded);

        if (placed)
        {
            Inventory.Instance.CreateItemViewPublic(upgraded);
            Inventory.Instance.UpdateItemViewSize(upgraded);
            Inventory.Instance.UpdateItemViewPosition(upgraded);
        }
        else
        {
            // Grid penuh, taruh di TempStorage
            TempStorage.Instance.SpawnItemToTempStorage(upgraded);
        }

        Debug.Log($"Upgrade! {tier} {shapeType} x3 → {newTier} {shapeType}");

        // Cek upgrade berantai
        CheckUpgrade();
    }

    bool TryPlaceAnywhere(InventoryItem item)
    {
        for (int y = 0; y < Inventory.Instance.height; y++)
        {
            for (int x = 0; x < Inventory.Instance.width; x++)
            {
                if (Inventory.Instance.PlaceItem(item, x, y))
                    return true;
            }
        }
        return false;
    }
}