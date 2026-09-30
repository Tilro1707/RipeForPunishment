using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Zugriff aus anderen Skripten: MusicManager.Instance.PlayThrowSound();
    public static MusicManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip[] backgroundHitSounds;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip eatingSound;
    [SerializeField] private AudioClip applauseSound;

    [Header("Game Over")]
    [SerializeField] private bool stopMusicOnGameOver = true;
    private bool gameOverSoundPlayed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Es gibt bereits einen MusicManager in der Szene.", this);
            Destroy(this);
            return;
        }

        Instance = this;

        if (sfxSource == null)
            Debug.LogError("MusicManager: SFX Source im Inspector zuweisen.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayThrowSound()
    {
        PlaySfx(throwSound);
    }

    public void PlayBackgroundHitSound()
    {
        if (backgroundHitSounds == null || backgroundHitSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, backgroundHitSounds.Length);
        PlaySfx(backgroundHitSounds[randomIndex]);
    }

    public void PlayPlayerHitSound()
    {
        PlaySfx(playerHitSound);
    }

    public void PlayEatingSound()
    {
        PlaySfx(eatingSound);
    }

    public void PlayGameOverSound()
    {
        if (gameOverSoundPlayed)
            return;

        if (stopMusicOnGameOver && musicSource != null)
            musicSource.Stop();

        PlaySfx(applauseSound);
        gameOverSoundPlayed = true;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (gameOverSoundPlayed || sfxSource == null || clip == null)
            return;

        // Die zentrale Source bleibt bestehen, wenn eine Tomate zerstoert wird.
        sfxSource.PlayOneShot(clip);
    }
}
