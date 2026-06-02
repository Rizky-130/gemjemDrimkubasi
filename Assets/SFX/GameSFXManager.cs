using UnityEngine;

public class GameSFXManager : MonoBehaviour
{
    public static GameSFXManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip waveStartClip;
    public AudioClip waveOverClip;
    public AudioClip towerHitClip;
    public AudioClip shootClip;
    public AudioClip enemyHitClip;
    public AudioClip buttonPressClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float masterSFXVolume = 1f;
    [Range(0f, 1f)] public float waveVolume = 1f;
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float shootVolume = 0.8f;
    [Range(0f, 1f)] public float buttonVolume = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    public void PlayWaveStart()
    {
        PlaySFX(waveStartClip, waveVolume);
    }

    public void PlayWaveOver()
    {
        PlaySFX(waveOverClip, waveVolume);
    }

    public void PlayTowerHit()
    {
        PlaySFX(towerHitClip, hitVolume);
    }

    public void PlayShoot()
    {
        PlaySFX(shootClip, shootVolume);
    }

    public void PlayEnemyHit()
    {
        PlaySFX(enemyHitClip, hitVolume);
    }

    public void PlayButtonPress()
    {
        PlaySFX(buttonPressClip, buttonVolume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        if (sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume * masterSFXVolume);
    }
}