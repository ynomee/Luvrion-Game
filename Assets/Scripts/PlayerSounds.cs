using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip runSound;
    public AudioClip slidingSound;
    public AudioClip deathSound;
    public AudioClip dashSound;
    public AudioClip fallSound;

    private AudioSource _loopSource; // Для зацикленных звуков (бег, скольжение)
    private AudioSource _oneShotSource; // Для одноразовых звуков (прыжок, даш)

    void Start()
    {
        // Создаём два источника звука
        _loopSource = gameObject.AddComponent<AudioSource>();
        _oneShotSource = gameObject.AddComponent<AudioSource>();
        
        // Настраиваем (громкость, pitch и т.д.)
        _loopSource.loop = true;
        _oneShotSource.loop = false;
        _loopSource.volume = 0.3f;
        _oneShotSource.volume = 0.3f;
    }

    // === Одноразовые звуки ===
    public void PlayJumpSound()
    {
        _oneShotSource.PlayOneShot(jumpSound);
    }

    public void PlayDashSound()
    {
        _oneShotSource.PlayOneShot(dashSound);
    }

    public void PlayDeathSound()
    {
        _oneShotSource.PlayOneShot(deathSound);
    }

    public void PlayFallSound()
    {
        _oneShotSource.PlayOneShot(fallSound);
    }

    // === Зацикленные звуки ===
    public void StartRunSound()
    {
        if (_loopSource.clip != runSound || !_loopSource.isPlaying)
        {
            _loopSource.clip = runSound;
            _loopSource.Play();
        }
    }

    public void StopRunSound()
    {
        if (_loopSource.clip == runSound && _loopSource.isPlaying)
        {
            _loopSource.Stop();
        }
    }

    public void StartSlidingSound()
    {
        if (_loopSource.clip != slidingSound || !_loopSource.isPlaying)
        {
            _loopSource.clip = slidingSound;
            _loopSource.Play();
        }
    }

    public void StopSlidingSound()
    {
        if (_loopSource.clip == slidingSound && _loopSource.isPlaying)
        {
            _loopSource.Stop();
        }
    }
}