using UnityEngine;
using System.Collections;

public class Tutorial_ConstrDelete : Tutorial {
	static public bool init=false;

	public Tutorial_ConstrDelete() {
		initialState = TutorialManager.STATES.TutConstrDelete1;
		endState = TutorialManager.STATES.TutConstrDelete6;

		//GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager>().cleanUI ();

		init = true;
	}

	public override void eventDone(TutorialManager.EVENTS ev) {
		if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
			changeToNextState ();
		} else if (ev == TutorialManager.EVENTS.EXIT) {
			changeToState (TutorialManager.STATES.END);
		}
	}
}
