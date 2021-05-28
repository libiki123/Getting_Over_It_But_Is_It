using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SFX { Shot, Reload, Force }

public class AudioPlayer : MonoBehaviour
{
    public AudioClip shot;
    public AudioClip reload;
    public AudioClip force;


    public AudioSource audioSource;
    public AudioSource backgroundAudioSource;

    public static AudioPlayer Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);
    }

    public void PlaySound(SFX sound)
	{
		switch (sound)
		{
            case SFX.Shot:
                audioSource.PlayOneShot(shot);
                break;
            case SFX.Reload:
                audioSource.PlayOneShot(reload);
                break;
            case SFX.Force:
                audioSource.PlayOneShot(force);
                break;
		}
	}

    public void PlayerBackgroundMusic()
	{
        backgroundAudioSource.Play();
    }
}
