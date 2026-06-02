using System.Collections.Generic;
using UnityEngine;

public class InventoryStats : MonoBehaviour
{
    public static InventoryStats Instance;

    [Header("Current Total Stats")]
    public int totalDamage;
    public int totalHP;
    public float totalFireRate;
    public int totalHM;

    private void Awake()
    {
        Instance = this;
    }

    public void Recalculate()
    {
        totalDamage = 0;
        totalHP = 0;
        totalFireRate = 0;
        totalHM = 0;

        HashSet<InventoryItem> countedItems =
            new HashSet<InventoryItem>();

        for (int y = 0; y < Inventory.Instance.height; y++)
        {
            for (int x = 0; x < Inventory.Instance.width; x++)
            {
                Slot slot = Inventory.Instance.GetSlot(x, y);

                if (slot.currentItem == null)
                    continue;

                InventoryItem item = slot.currentItem;

                // Hindari item multi-block dihitung berkali-kali
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
            $"DMG:{totalDamage} | HP:{totalHP} | FR:{totalFireRate} | HM:{totalHM}"
        );
    }
}