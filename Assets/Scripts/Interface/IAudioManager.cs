using UnityEngine;

public interface IAudioManager
{
    public void Play(AudioClip audioClip);

    public void Play(AudioClip audioClip, Vector2 position, float volume = 1);

    public void PlayOneShot(AudioClip audioClip);
}
