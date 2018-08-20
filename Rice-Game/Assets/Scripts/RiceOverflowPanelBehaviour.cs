using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class RiceOverflowPanelBehaviour : Panel
{
    private MenuAction m_action;
    //private uint m_possibleRiceLost;

    override protected void initBase()
    {
        this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
    }

    void Awake ()
    {
		initBase ();		
	}
    
    public void init(MenuAction action)
    {
        m_action = action;
        //m_possibleRiceLost = possibleRiceLost;
    }

    public new void kill()
    {
        Destroy(this.transform.parent.gameObject);
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
        m_action.doMenuAction();
        //UserDataManager.GetInstance().addRiceLost(m_possibleRiceLost);
        this.kill();
    }
}