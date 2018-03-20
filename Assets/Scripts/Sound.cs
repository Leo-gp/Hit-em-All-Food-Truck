using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
	[Header("References to attach")]
	public string name;
	public AudioClip clip;
	[Header("Variables")]
	[Range(0f, 1f)] public float volume;
	[Range(0f, 3f)] public float pitch;
	public bool playOnAwake;
	public bool loop;

	[HideInInspector] public AudioSource source;
}