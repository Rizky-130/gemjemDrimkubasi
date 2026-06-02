using System.Collections;
using UnityEngine;

public class TowerHitEffect : MonoBehaviour
{
    [Header("Sprite Flash")]
    public SpriteRenderer[] spriteRenderers;
    public Color hitColor = Color.red;
    public float flashDuration = 0.08f;

    [Header("Glow Effect")]
    public SpriteRenderer glowRenderer;
    public Color glowColor = new Color(1f, 0.2f, 0.1f, 0.8f);
    public float glowDuration = 0.15f;

    [Header("Scale Punch")]
    public Transform targetScaleObject;
    public float scaleAmount = 1.08f;
    public float scaleDuration = 0.08f;

    [Header("Particle Effect")]
    public GameObject hitParticlePrefab;
    public Transform particleSpawnPoint;

    private Color[] originalColors;
    private Vector3 originalScale;

    private Coroutine effectRoutine;

    private void Awake()
    {
        if (targetScaleObject == null)
            targetScaleObject = transform;

        originalScale = targetScaleObject.localScale;

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                originalColors[i] = spriteRenderers[i].color;
        }

        if (glowRenderer != null)
        {
            Color hiddenGlow = glowColor;
            hiddenGlow.a = 0f;
            glowRenderer.color = hiddenGlow;
        }
    }

    public void PlayHitEffect()
    {
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(HitEffectRoutine());

        SpawnParticle();
    }

    private IEnumerator HitEffectRoutine()
    {
        
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = hitColor;
        }

        
        if (glowRenderer != null)
        {
            Color visibleGlow = glowColor;
            glowRenderer.color = visibleGlow;
        }

        
        if (targetScaleObject != null)
        {
            targetScaleObject.localScale = originalScale * scaleAmount;
        }

        yield return new WaitForSeconds(flashDuration);

        
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = originalColors[i];
        }

        
        if (targetScaleObject != null)
        {
            targetScaleObject.localScale = originalScale;
        }

        yield return new WaitForSeconds(glowDuration);

        // Hide glow
        if (glowRenderer != null)
        {
            Color hiddenGlow = glowColor;
            hiddenGlow.a = 0f;
            glowRenderer.color = hiddenGlow;
        }

        effectRoutine = null;
    }

    private void SpawnParticle()
    {
        if (hitParticlePrefab == null)
            return;

        Vector3 spawnPosition = transform.position;

        if (particleSpawnPoint != null)
            spawnPosition = particleSpawnPoint.position;

        Instantiate(
            hitParticlePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}