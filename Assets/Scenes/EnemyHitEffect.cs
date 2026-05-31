using System.Collections;
using UnityEngine;

public class EnemyHitEffect : MonoBehaviour
{
    [Header("Sprite Hit Effect")]
    public Color hitColor = Color.white;
    public Color glowColor = new Color(1f, 0.8f, 0.2f, 1f);
    public float flashTime = 0.06f;
    public float glowTime = 0.12f;

    [Header("Scale Punch")]
    public float scaleAmount = 1.12f;

    [Header("Blood Particle")]
    public ParticleSystem bloodParticlePrefab;
    public float bloodZOffset = -0.1f;

    private SpriteRenderer enemySprite;
    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine hitRoutine;

    private void Awake()
    {
        enemySprite = GetComponent<SpriteRenderer>();

        if (enemySprite == null)
            enemySprite = GetComponentInChildren<SpriteRenderer>();

        if (enemySprite != null)
            originalColor = enemySprite.color;
        else
            Debug.LogError(gameObject.name + " has no SpriteRenderer!");

        originalScale = transform.localScale;
    }

    public void PlayHitEffect()
    {
        PlayBloodEffect();

        if (enemySprite == null)
            return;

        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        enemySprite.color = hitColor;
        transform.localScale = originalScale * scaleAmount;

        yield return new WaitForSeconds(flashTime);

        enemySprite.color = glowColor;
        transform.localScale = originalScale;

        yield return new WaitForSeconds(glowTime);

        enemySprite.color = originalColor;
        transform.localScale = originalScale;

        hitRoutine = null;
    }

    private void PlayBloodEffect()
    {
        if (bloodParticlePrefab == null)
        {
            Debug.LogWarning("Blood particle prefab is not assigned on " + gameObject.name);
            return;
        }

        Vector3 spawnPos = transform.position;
        spawnPos.z += bloodZOffset;

        ParticleSystem blood = Instantiate(
            bloodParticlePrefab,
            spawnPos,
            Quaternion.identity
        );

        blood.gameObject.SetActive(true);
        blood.Clear();
        blood.Play();

        Debug.Log("Blood particle spawned!");

        ParticleSystem.MainModule main = blood.main;
        float destroyTime = main.duration + main.startLifetime.constantMax + 0.5f;

        Destroy(blood.gameObject, destroyTime);
    }
}