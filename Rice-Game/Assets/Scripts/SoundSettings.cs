using UnityEngine;
using System.Collections;

public class SoundSettings : MonoBehaviour {

	public bool isBGM;
	private SoundManager sm;

	void Start () {
		sm = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isBGM) 
			GetComponent<AudioSource> ().volume = sm.getMusicVolume ();
		else
			GetComponent<AudioSource> ().volume = sm.getSoundVolume ();
	}
}
