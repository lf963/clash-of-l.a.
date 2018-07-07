using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager> {

	public AudioSource bgMusic;
	public AudioSource attackMusic;
	private float startVolume;
	// Use this for initialization
	void Start () {
		startVolume = bgMusic.volume;
	}

	public void VolumeLow (AudioSource curMusic) {
		curMusic.volume = startVolume / 3.0f;
	}

	public void VolumeHigh (AudioSource curMusic) {
		curMusic.volume = startVolume;
	}
}
