using System.Collections.Generic;
using UnityEngine;

public class InventoryStats : MonoBehaviour
{
    public static InventoryStats Instance;

    [Header("Bonus Stats From Inventory")]
    public int totalDamage;
    public int totalHP;
    public float totalFireRate;
    public int totalHM;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Recalculate();
    }

    public void Recalculate()
    {
        totalDamage = 0;
        totalHP = 0;
        totalFireRate = 0f;
        totalHM = 0;

        if (Inventory.Instance == null)
        {
            Debug.LogWarning("Inventory.Instance not found. InventoryStats cannot recalculate.");
            UpdateTowerStats();
            return;
        }

        HashSet<InventoryItem> countedItems = new HashSet<InventoryItem>();

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

                // Prevent multi-block items from being counted multiple times
                if (countedItems.Contains(item))
                    continue;

                countedItems.Add(item);

                totalDamage += item.damage;
                totalHP += item.hpTurret;
                totalFireRate += item.fireRate;
                totalHM += item.hm;
            }
        }

        Debug.Log(
            $"Inventory Bonus Stats | DMG:+{totalDamage} | HP:+{totalHP} | FR:+{totalFireRate} | HM:{totalHM}"
        );

        UpdateTowerStats();
    }

    private void UpdateTowerStats()
    {
        if (TowerStats.Instance != null)
        {
            TowerStats.Instance.RefreshStatsFromInventory();
        }
    }
}