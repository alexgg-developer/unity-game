using UnityEngine;
using System.Collections;

public class TutorialPanel : MonoBehaviour {
	private TutorialManager _tutMan;
	public void Start() {
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
	}

	public void Update() {
		//Debug.Log ("TutorialPanel::Update()");
		//RectTransform panelRectTransform = gameObject.GetComponent<RectTransform> ();
		Transform parentPanelRectTransform = transform.parent.transform.transform.parent;
		if (parentPanelRectTransform != null) parentPanelRectTransform.SetAsLastSibling ();
		//Debug.Log ("Sibling Index: "+transform.parent.transform.transform.parent.GetSiblingIndex());
	}

	public void clickedButtonNext() {
		Debug.Log ("TutorialPanel :: Clicked");
		_tutMan.eventDone (TutorialManager.EVENTS.NEXT_POPUP);
	}

	public void clickedButtonOmitir() {
		Debug.Log ("TutorialPanel :: Clicked");
		_tutMan.eventDone (TutorialManager.EVENTS.EXIT);
	}
}
