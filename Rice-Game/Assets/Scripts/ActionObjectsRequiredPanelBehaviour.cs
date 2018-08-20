using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ActionObjectsRequiredPanelBehaviour : MonoBehaviour 
{
	private List<int> _objectsID;
	public Text objectListText;

	public void Awake()
	{
		GetComponent<RectTransform>().SetParent(GameObject.Find ("UICanvas").GetComponent<RectTransform>(), false);
	}
	public void kill()
	{
		Destroy (gameObject);
	}

	public void openBuyMenu()
	{		
		GameObject parent = GameObject.Find ("UICanvas");
		Component[] trs = parent.GetComponentsInChildren (typeof(Transform), true); 
		foreach (Component t in trs) {
			if (t.name.Equals("BuyMenu")) {
				t.gameObject.GetComponent<BuyMenu>().selectObjectTab ();
				t.gameObject.SetActive (true);
				this.kill ();
			}
		}
	}

	public void setObjectsID(List<int> objectsID)
	{
		_objectsID = objectsID;
		string objectListString = "";
		for (int i = 0; i < _objectsID.Count; ++i) {
			objectListString += RiceObjectsManager.GetInstance ().getObjectRiceTitle (_objectsID[i]) + "\n";
		}
		objectListText.text = objectListString;
	}
}

