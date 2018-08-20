using UnityEngine;
using System.Collections;

public abstract class Panel : MonoBehaviour {
	//protected GameObject panelFocusLayer;
	protected LogicManager logicManager;

	virtual protected void initBase () {
        RectTransform canvas = GameObject.Find("UICanvas").GetComponent<RectTransform>();
        GetComponent<RectTransform>().SetParent(canvas, false);        
    }

	public void setLogicManager(LogicManager logicManager) {
		this.logicManager = logicManager;
	}

	abstract public void actionClicked (int id);

	/*
	public void lostFocus() {
		Debug.Log ("Panel::lostFocus");	
		kill ();
	}
	*/

	virtual public void kill() {
        gameObject.transform.parent.gameObject.SetActive(false);
	}
}
