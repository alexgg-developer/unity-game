using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingBehaviour: MonoBehaviour
{
    void Start()
    {
        //return;
        Invoke("load", 1.0f);
    }

    public void load()
    {
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            SceneManager.LoadScene("UITest");
        }
        else {
			//GameSaveDataManager.LoadFile();
			SceneManager.LoadScene("UITest");
        }
    }
}
