using UnityEngine;
using System.Collections;

public class Tutorial_Cope : Tutorial {
	static public bool init=false;

	public Tutorial_Cope() {
		initialState = TutorialManager.STATES.TutCope1;
		endState = TutorialManager.STATES.TutCope11;

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
