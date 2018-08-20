using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class InstantiableGenericPanelBehaviour : Panel
{

    override protected void initBase()
    {
        this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
    }

    void Awake ()
    {
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
}