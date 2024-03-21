using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] private List<AudioSource> _audioSources;

    [SerializeField] private AudioClip _gunshot;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _windSound;

    public void PlayMenuSound()
    {
        _audioSources[0].clip = _menuMusic;
        _audioSources[0].Play();
    }

    public void PlayWind()
    {
        _audioSources[2].clip = _windSound;
        _audioSources[2].Play();
    }

    public void StopWind()
    {
        _audioSources[2].Stop();
    }

    public void PlayShotgunSound()
    {
        _audioSources[1].clip = _gunshot;
        _audioSources[1].Play();
    }
}