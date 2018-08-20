using UnityEngine;
using System.Collections;

public class Tutorial_ConstrCanal : Tutorial {
	static public bool init=false;

	public Tutorial_ConstrCanal() {
		initialState = TutorialManager.STATES.TutConstrCanal1;
		endState = TutorialManager.STATES.TutConstrCanal7;

		//GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager>().cleanUI ();

		init = true;
	}

	/*
	public override void startTuto() {
		base.startTuto ();
		Debug.Log ("Tutorial_ConstrCanal::startTuto()");
		RectTransform panelRectTransform = _panel.GetComponent<RectTransform> ();
		panelRectTransform.SetAsLastSibling ();
	}
	*/

	public override void eventDone(TutorialManager.EVENTS ev) {
		if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
			changeToNextState ();
		} else if (ev == TutorialManager.EVENTS.EXIT) {
			changeToState (TutorialManager.STATES.END);
		}
	}
}
