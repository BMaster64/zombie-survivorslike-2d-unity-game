using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Mixer")]
    public AudioMixerGroup sfxMixerGroup;
    
    [Header("Weapon Sounds")]
    public AudioClip char1GunSound;
    public AudioClip char2GunSound;
    public AudioClip grenadeSound;
    
    [Header("Skill Sounds")]
    public AudioClip char1SkillSound;
    public AudioClip char2SkillSound;
    public AudioClip maggotSkillSound;
    
    [Header("Game Sounds")]
    public AudioClip damagedSound;
    public AudioClip bossSpawnSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip pauseSound;
    public AudioClip selectSound;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    // Specific methods for common sounds
    public void PlayWeaponSound(int characterType)
    {
        switch (characterType)
        {
            case 1:
                PlaySFX(char1GunSound);
                break;
            case 2:
                PlaySFX(char2GunSound);
                break;
        }
    }
    
    public void PlaySkillSound(int characterType)
    {
        switch (characterType)
        {
            case 1:
                PlaySFX(char1SkillSound);
                break;
            case 2:
                PlaySFX(char2SkillSound);
                break;
        }
    }
    
    public void PlayDamageSound()
    {
        PlaySFX(damagedSound);
    }
    
    public void PlayGrenadeSound()
    {
        PlaySFX(grenadeSound);
    }
}
