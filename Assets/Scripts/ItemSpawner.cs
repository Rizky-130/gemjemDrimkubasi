using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // Dipanggil dari tombol di UI
    public void SpawnRandomItem()
    {
        if (!TempStorage.Instance.HasSpace())
        {
            Debug.Log("Temp storage penuh, tidak bisa spawn!");
            return;
        }

        // Random shape
        ItemShape[] shapes = (ItemShape[])System.Enum.GetValues(typeof(ItemShape));
        ItemShape randomShape = shapes[Random.Range(0, shapes.Length)];

        // Selalu Bronze saat spawn
        InventoryItem newItem = new InventoryItem(
            $"Bronze {randomShape}",
            randomShape,
            ItemTier.Bronze
        );

        TempStorage.Instance.SpawnItemToTempStorage(newItem);

        Debug.Log($"Spawned: Bronze {randomShape}");
    }
}