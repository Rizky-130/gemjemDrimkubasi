using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    [Header("HM Block Drop Prefab")]
    public HMBlockWorldDropper hmBlockDropPrefab;

    [Header("Drop Settings")]
    public float dropSpread = 0.3f;

    private void Awake()
    {
        Instance = this;
    }

    public void DropHMBlock(Vector3 enemyPosition, int currentWave)
    {
        if (hmBlockDropPrefab == null)
        {
            Debug.LogError("DropManager has no HMBlockWorldDropper prefab assigned!");
            return;
        }

        ItemBlockType randomBlockType = HMBlockDropTable.GetRandomBlockType();
        ItemTier randomTier = HMBlockDropTable.GetRandomTierByWave(currentWave);

        Vector3 dropPosition = enemyPosition + new Vector3(
            Random.Range(-dropSpread, dropSpread),
            Random.Range(-dropSpread, dropSpread),
            0f
        );

        HMBlockWorldDropper droppedBlock = Instantiate(
            hmBlockDropPrefab,
            dropPosition,
            Quaternion.identity
        );

        droppedBlock.Setup(randomBlockType, randomTier);

        Debug.Log(
            "Dropped " +
            HMItemDatabase.GetDisplayName(randomBlockType, randomTier) +
            " on Wave " +
            currentWave
        );
    }
}