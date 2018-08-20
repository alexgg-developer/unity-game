using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	private float musicVolume;
	private float soundVolume;

	void Start () {
		if (!PlayerPrefs.HasKey ("MusicVolume"))
			PlayerPrefs.SetFloat ("MusicVolume", 1.0f);
		if (!PlayerPrefs.HasKey ("SoundVolume"))
			PlayerPrefs.SetFloat ("SoundVolume", 1.0f);

		musicVolume = PlayerPrefs.GetFloat ("MusicVolume");
		soundVolume = PlayerPrefs.GetFloat ("SoundVolume");
	}

	public void setMusicVolume(Slider s){
		musicVolume = s.value;
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
	}

	public void setSoundVolume(Slider s){
		soundVolume = s.value;
		PlayerPrefs.SetFloat ("SoundVolume", soundVolume);
	}

	public float getMusicVolume(){
		return musicVolume;
	}

	public float getSoundVolume(){
		return soundVolume;
	}
}
