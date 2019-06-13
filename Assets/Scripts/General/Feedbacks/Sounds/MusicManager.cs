using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicManager 
{
    [Header("References")]
    [SerializeField] AudioSource[] audioSources;
    [SerializeField] float fadeDurationTest = 2;
    Queue<AudioSource> audioSourcesQueue;
    AudioSource currentlyPlayingSource;
    AudioSource currentlyFadingSource;

    public void SetUp()
    {
        audioSourcesQueue = new Queue<AudioSource>();
        foreach (AudioSource source in audioSources)
            audioSourcesQueue.Enqueue(source);
    }

    public void StartMusic(Sound musicToPlay, float fadeDuration)
    {
        if (currentlyPlayingSource != null)
        {
            currentlyFadingSource = currentlyPlayingSource;
            startFadeOutVolume = currentlyFadingSource.volume;
        }

        currentlyPlayingSource = audioSourcesQueue.Dequeue();

        targetFadeInVolume = musicToPlay.volume;
        currentlyPlayingSource.PlaySound(musicToPlay);
        currentlyPlayingSource.volume = 0;

        currentFadeTotalDuration = fadeDuration;
        currentFadeDuration = currentFadeTotalDuration;
    }

    float currentFadeTotalDuration;
    float currentFadeDuration;
    public float GetCurrentFadeInProgression { get { return 1 - (currentFadeDuration / currentFadeTotalDuration); } }
    public float GetCurrentFadeOutProgression { get { return currentFadeDuration / currentFadeTotalDuration; } }

    float targetFadeInVolume;
    float startFadeOutVolume;

    public bool Fading { get { return currentFadeDuration != 0; } }
    public void UpdateMusicManagement()
    {
        currentlyPlayingSource.volume = Mathf.Lerp(0, targetFadeInVolume, GetCurrentFadeInProgression);
        if (currentlyFadingSource != null)
            currentlyFadingSource.volume = Mathf.Lerp(0, startFadeOutVolume, GetCurrentFadeOutProgression);

        if (currentFadeDuration > 0)
            currentFadeDuration -= Time.deltaTime;
        else if (currentFadeDuration < 0)
        {
            currentFadeDuration = 0;
            currentlyPlayingSource.volume = targetFadeInVolume;

            if (currentlyFadingSource != null)
            {
                currentlyFadingSource.Stop();
                audioSourcesQueue.Enqueue(currentlyFadingSource);
                currentlyFadingSource.volume = 0;
                currentlyFadingSource = null;
            }
        }
    }

    [Header("Musics")]
    [SerializeField] Sound mapMusic;
    [SerializeField] float mapMusicFadeDuration = 2;
    public void PlayMapMusic()
    {
        StartMusic(mapMusic, mapMusicFadeDuration);
    }

    [SerializeField] Sound tutorialMusic;
    [SerializeField] float tutorialMusicFadeDuration = 2;
    public void PlayTutorialMusic()
    {
        StartMusic(tutorialMusic, tutorialMusicFadeDuration);
    }

    [SerializeField] Sound arenaMusic;
    [SerializeField] float arenaMusicFadeDuration = 2;
    public void PlayArenaMusic()
    {
        StartMusic(arenaMusic, arenaMusicFadeDuration);
    }

    [SerializeField] Sound victoryMusic;
    [SerializeField] float victoryMusicFadeDuration = 1.2f;
    public void PlayVictoryMusic()
    {
        StartMusic(victoryMusic, victoryMusicFadeDuration);
    }

    [SerializeField] Sound loseMusic;
    [SerializeField] float loseMusicFadeDuration = 0.8f;

    public void PlayLoseMusic()
    {
        StartMusic(loseMusic, loseMusicFadeDuration);
    }
}
