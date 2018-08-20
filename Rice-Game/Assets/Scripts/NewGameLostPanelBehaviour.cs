using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class NewGameLostPanelBehaviour : Panel
{
    void Awake ()
    {
		
	}
    
    public void init(MenuAction action)
    {
		
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

    public void cancelButtonButtonClicked()
    {
        this.kill();
    }

    public void doActionButtonClicked()
    {
		try {
			File.Delete (Application.persistentDataPath + "/savedGames.gd");
		}
		catch(Exception ex) {
			Debug.LogWarning ("saved game did not exist or an error ocurred deleting it");
			Debug.LogWarning (ex.StackTrace);
		}
		NewGame ();
        this.kill();
    }

	private void NewGame()
	{
		PlayerPrefs.SetInt("LoadData", 0);
		SceneManager.LoadScene("loading");
	}
}