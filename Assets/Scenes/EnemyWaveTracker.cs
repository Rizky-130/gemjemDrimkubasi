using UnityEngine;

public class EnemyWaveTracker : MonoBehaviour
{
    private WaveSpawner waveSpawner;
    private bool hasBeenRemoved = false;

    public void Setup(WaveSpawner spawner)
    {
        waveSpawner = spawner;
    }

    private void OnDestroy()
    {
        if (hasBeenRemoved)
            return;

        hasBeenRemoved = true;

        if (waveSpawner != null)
        {
            waveSpawner.RemoveEnemy(gameObject);
        }
    }
}