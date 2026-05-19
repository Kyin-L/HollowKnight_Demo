using UnityEngine;

public class PlayerAudio
{
    public PlayerAudioConfig audioConfig;

    public AudioSource audio;
    public AudioSource move;

    public PlayerAudio(PlayerAudioConfig audioConfig)
    {
        this.audioConfig = audioConfig;
    }

    public void PlayWalk(bool isPlay)
    {
        if (isPlay)
        {
            move.clip = audioConfig.walk;
            move.Play();
        }
        else
        {
            move.Stop();
        }
    }

    public void PlayRun(bool isPlay)
    {
        if (isPlay)
        {
            move.clip = audioConfig.run;
            move.Play();
        }
        else
        {
            move.Stop();
        }
    }

    public void PlayOneShotJump()
    {
        audio.PlayOneShot(audioConfig.jump, 0.5f);
    }

    public void PlayOneShotDoubleJump()
    {
        audio.PlayOneShot(audioConfig.doubleJump, 0.5f);
    }

    public void PlayOneShotSoftLand()
    {
        audio.PlayOneShot(audioConfig.softLand, 0.5f);
    }

    public void PlayOneShotForwardSlash1()
    {
        audio.PlayOneShot(audioConfig.forwardSlash1);
    }

    public void PlayOneShotForwardSlash2()
    {
        audio.PlayOneShot(audioConfig.forwardSlash1);
    }

    public void PlayOneShotUpSlash()
    {
        audio.PlayOneShot(audioConfig.upSlash);
    }

    public void PlayOneShotDownSlash()
    {
        audio.PlayOneShot(audioConfig.downSlash);
    }

    public void PlayOneShotDamaged()
    {
        audio.PlayOneShot(audioConfig.damaged);
    }
}
