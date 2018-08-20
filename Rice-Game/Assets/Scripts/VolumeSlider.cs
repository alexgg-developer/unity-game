using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

	public bool isMusicSlider;

	void Start () {
		SoundManager sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		if (isMusicSlider)
			GetComponent<Slider> ().value = sm.getMusicVolume ();
		else
			GetComponent<Slider> ().value = sm.getSoundVolume ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
