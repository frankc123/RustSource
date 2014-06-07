using System;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private float nextMusicTime = 5f;
    private static int savedFrameCount = -1;
    public float startDelay;
    public float targetVolume = 0.2f;
    public float timeBetweenTracks = 600f;
    public AudioClip[] tracks;
    private bool wasMuted = true;

    private void Start()
    {
        this.wasMuted = settings.mute;
        this.nextMusicTime = (Time.time + 3f) + this.startDelay;
        base.audio.volume = 0f;
        if ((this.tracks == null) || (this.tracks.Length == 0))
        {
            base.enabled = false;
        }
    }

    private void Update()
    {
        int frameCount = Time.frameCount;
        if (frameCount != savedFrameCount)
        {
            savedFrameCount = frameCount;
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                settings.mute = !settings.mute;
            }
        }
        if (this.wasMuted != settings.mute)
        {
            if (this.wasMuted && (Time.time > this.nextMusicTime))
            {
                this.nextMusicTime = Time.time;
            }
            else
            {
                base.audio.Stop();
                this.nextMusicTime = Time.time;
            }
            this.wasMuted = !this.wasMuted;
        }
        if (!this.wasMuted)
        {
            if (Time.time > this.nextMusicTime)
            {
                if (this.tracks.Length == 0)
                {
                    return;
                }
                AudioClip clip = this.tracks[Random.Range(0, this.tracks.Length)];
                base.audio.Stop();
                base.audio.clip = clip;
                this.nextMusicTime = (Time.time + clip.length) + (this.timeBetweenTracks * Random.RandomRange((float) 0.75f, (float) 1.25f));
                base.audio.Play();
            }
            float num2 = this.targetVolume * sound.music;
            if (base.audio.volume < num2)
            {
                AudioSource audio = base.audio;
                audio.volume += (Time.deltaTime / 3f) * num2;
            }
            if (base.audio.volume > num2)
            {
                base.audio.volume = num2;
            }
        }
    }

    private static class settings
    {
        private static bool _mute = (PlayerPrefs.GetInt("MUSIC_MUTE", 0) != 0);

        public static bool mute
        {
            get
            {
                return _mute;
            }
            set
            {
                if (value != _mute)
                {
                    if (value)
                    {
                        PlayerPrefs.SetInt("MUSIC_MUTE", 1);
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey("MUSIC_MUTE");
                    }
                    _mute = value;
                }
            }
        }
    }
}

