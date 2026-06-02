using UnityEngine;

public class HMBlockWorldDropper : MonoBehaviour
{
    [Header("HM Block Data")]
    public string itemName = "HM Block";

    public ItemBlockType blockType = ItemBlockType.BlockDot;
    public ItemTier tier = ItemTier.Bronze;
    public ItemShape shapeType = ItemShape.OneBlock;

    [Header("Visual")]
    public ItemSpriteConfig spriteConfig;

    [Header("Pickup Settings")]
    public bool allowMouseClickPickup = true;

    private SpriteRenderer spriteRenderer;
    private bool collected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        RefreshData();
        UpdateVisual();
    }

    public void Setup(ItemBlockType newBlockType, ItemTier newTier)
    {
        blockType = newBlockType;
        tier = newTier;

        RefreshData();
        UpdateVisual();
    }

    private void RefreshData()
    {
        shapeType = InventoryItem.GetShapeFromBlockType(blockType);
        itemName = HMItemDatabase.GetDisplayName(blockType, tier);
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning(gameObject.name + " has no SpriteRenderer.");
            return;
        }

        if (spriteConfig == null)
        {
            Debug.LogWarning(gameObject.name + " has no ItemSpriteConfig assigned.");
            return;
        }

        Sprite newSprite = spriteConfig.GetSprite(shapeType);

        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }

        spriteRenderer.color = spriteConfig.GetColor(blockType, tier);
    }

    public void CollectToTempStorage()
    {
        if (collected)
            return;

        collected = true;

        TempStorage tempStorage = TempStorage.Instance;

        if (tempStorage == null)
        {
            tempStorage = FindObjectOfType<TempStorage>(true);
        }

        if (tempStorage == null)
        {
            Debug.LogError("TempStorage not found! Create TempStorageManager in the scene and add TempStorage.cs to it.");
            collected = false;
            return;
        }

        if (!tempStorage.HasSpace())
        {
            Debug.Log("TempStorage is full. The HM block stays in the world.");
            collected = false;
            return;
        }

        InventoryItem newItem = new InventoryItem(
            itemName,
            blockType,
            tier
        );

        bool success = tempStorage.SpawnItemToTempStorage(newItem);

        if (success)
        {
            Debug.Log(itemName + " collected into TempStorage.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Failed to store " + itemName);
            collected = false;
        }
    }

    private void OnMouseDown()
    {
        if (!allowMouseClickPickup)
            return;

        CollectToTempStorage();
    }
}