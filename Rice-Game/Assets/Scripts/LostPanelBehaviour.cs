using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class LostPanelBehaviour : Panel
{

    override protected void initBase()
    {
        this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
    }

    void Awake ()
    {
		PlayerPrefs.SetInt("LoadData", 0);
		TimeManager timeManager = GameObject.Find ("Logic").GetComponent<TimeManager>();
		timeManager.pauseTime (true);
		initBase ();
	}
    
    public new void kill()
    {
        Destroy(this.transform.parent.gameObject);
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

    public void exitButtonClicked()
	{
		PlayerPrefs.SetInt("LoadData", 0);
        SceneManager.LoadScene("main_menu");
        this.kill();
    }

    public void startAgainButtonClicked()
    {
        this.kill();
    }
}