using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SettingsPanelBehaviour : MonoBehaviour 
{
	public void Awake()
	{
		if(PlayerPrefs.HasKey("CurrentLanguage")) {
			GameObject.Find("ChangeLanguageDropdown").GetComponent<Dropdown>().value = PlayerPrefs.GetInt("CurrentLanguage");
		}
	}

	public void LanguageChanged(int language)
	{
			PlayerPrefs.SetInt ("CurrentLanguage", language);
	}


}

