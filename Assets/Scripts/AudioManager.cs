using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip correct, wrong, win, flip;
    public AudioSource audioSource;


    // Singleton instance
    public static AudioManager instance = null;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		} else if (instance!= this)
        {
            Destroy(this);
        }
		DontDestroyOnLoad(this);
	}

    public void PlayWrongAudio()
    {
        audioSource.PlayOneShot(wrong);
    }

    public void PlayFlipAudio()
    {
        audioSource.PlayOneShot(flip);
    }

    public void PlayMatchAudio()
    {
        audioSource.PlayOneShot(correct);
    }

    public void PlayWinAudio()
    {
        audioSource.PlayOneShot(win);
    }

}
