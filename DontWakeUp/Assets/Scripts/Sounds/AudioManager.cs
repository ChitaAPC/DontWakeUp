using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    public Sound[] songs;

    public Sound[] FSXs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in songs)
        {
            s.source =  gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private Sound GetSongByName(string name)
    {
        Sound sound = Array.Find(songs, s => s.name == name);
        if (sound == null)
        {
            Debug.LogError($"Could not find sound named \"{name}\"");
            return null;
        }
        return sound;
    }

    private Sound GetPlayingSong()
    {
        return Array.Find(songs, s => s.source.isPlaying);
    }

    public void PlaySong(string name)
    {
        GetSongByName(name).source.Play();
    }

    public void PlaySongWithDelay(string name, float delay)
    {
        GetSongByName(name).source.PlayDelayed(delay);
    }

    public float GetSongLength(string name)
    {
        return GetSongByName(name).source.clip.length;
    }

    public void PlaySFX(string name)
    {
        //todo
    }

    public void StopAllMusic()
    {
        foreach (Sound s in songs)
        {
            s.source.Stop();
        }
    }

    public void ChangeSongs(string name, bool shouldPause)
    {
        Sound songToPlay = GetSongByName(name);
        Sound playingSong = GetPlayingSong();

        if (playingSong != null)
        {
            if (shouldPause)
            {
                playingSong.source.Pause();
            }
            else
            {
                playingSong.source.Stop();
            }
        }
        songToPlay.source.Play();
    }


    public void FadeSongSynced(string name)
    {
        Sound songToPlay = GetSongByName(name);
        Sound playingSong = GetPlayingSong();

        if (playingSong != null)
        {
            songToPlay.source.time = playingSong.source.time;
            songToPlay.source.timeSamples = playingSong.source.timeSamples;
            StartCoroutine(FadeOutSong(playingSong, 2f));
        }
        songToPlay.source.Play();
    }

    private IEnumerator FadeOutSong(Sound playingSong, float fadeDiration)
    {
        float t = 0;
        float wantedVol = playingSong.source.volume;
        while (t < fadeDiration)
        {
            t += Time.unscaledDeltaTime;
            playingSong.source.volume -= ((1f/fadeDiration)*Time.unscaledDeltaTime);
            yield return null;
        }
        playingSong.source.Stop();
        playingSong.source.volume = wantedVol;
    }

    public void SetWantedFadeInSong(string song)
    {
        songToFadeIn = song;
    }

    public void PlayTwoSongsInSync(string song1, string song2)
    {
        GetSongByName(song1).source.Play();
        GetSongByName(song2).source.Play();
        GetSongByName(song2).source.volume = 0f;
        shouldFadeSongs = true;
        songToFadeIn = song1;
    }

    public void SetFadeStop()
    {
        shouldFadeSongs = false;
    }

    private bool shouldFadeSongs = false;
    private string songToFadeIn;

    private void Update()
    {
        if (shouldFadeSongs)
        {
            foreach (Sound s in songs)
            {
                if (s.source.isPlaying)
                {
                    if (s.name == songToFadeIn)
                    {
                        if (s.source.volume < 1f)
                            s.source.volume += 0.5f * Time.unscaledDeltaTime;
                    }
                    else
                    {
                        if (s.source.volume > 0f)
                            s.source.volume -= 0.5f * Time.unscaledDeltaTime;
                    }
                }
            }
        }
        
        
    }

}
