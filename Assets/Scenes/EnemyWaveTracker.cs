using UnityEngine;

public class EnemyWaveTracker : MonoBehaviour
{
    private WaveSpawner waveSpawner;

    public void Setup(WaveSpawner spawner)
    {
        waveSpawner = spawner;
    }

    private void OnDestroy()
    {
        if (waveSpawner != null)
        {
            waveSpawner.RemoveEnemy(gameObject);
        }
    }
}