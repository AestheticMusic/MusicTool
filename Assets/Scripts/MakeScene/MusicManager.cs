using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance;

	[HideInInspector]
	public float musicLength = 0f;
	public float musicTime
	{
		get
		{
			return musicSource.time;
		}
		set
		{
			musicSource.time = value;
		}
	}

	private AudioSource musicSource;
	private AudioSource seSource;
	private MakeManager g;



	private void Awake()
	{
		instance = this;

		AudioSource[] sources = this.GetComponents<AudioSource>();
		musicSource = sources[0];
		seSource = sources[1];
	}

	private void Start()
	{
		g = MakeManager.instance;
	}

	private void Update()
	{
		if (g.playMode && !musicSource.isPlaying)
			g.playMode = false;
	}

	public void LoadMusic(AudioClip _music)
	{
		musicSource.clip = _music;
		musicLength = _music.length;
		UIManager.instance.musicBar.maxValue = musicLength;
	}

	public void Play()
	{
		musicSource.Play();
	}

	public void Stop()
	{
		musicSource.Stop();
	}

	public void Pause()
	{
		musicSource.Pause();
	}

	public void UnPause()
	{
		musicSource.UnPause();
	}

	public void ChangePitch(float _pitch)
	{
		musicSource.pitch = _pitch;
	}
}
