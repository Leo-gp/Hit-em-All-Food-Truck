using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioController : MonoBehaviour 
{
	public Sound[] sounds;

	public static AudioController instance;

	void Awake ()
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

		foreach (Sound sound in sounds)
		{
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
			if (sound.playOnAwake == true)
				Play(sound.name);
		}
	}

	public void Play (string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s != null)
		{
			s.source.Play();
		}
		else
		{
			Debug.LogError("Sound: " + name + " not found!");
		}
	}

	public void Stop (string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s != null)
		{
			s.source.Stop();
		}
		else
		{
			Debug.LogError("Sound: " + name + " not found!");
		}
	}
}