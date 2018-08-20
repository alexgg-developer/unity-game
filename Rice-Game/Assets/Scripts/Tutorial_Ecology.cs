using UnityEngine;
using System.Collections;

public class Tutorial_Ecology : Tutorial {
	static public bool init=false;

	public Tutorial_Ecology() {
		initialState = TutorialManager.STATES.TutEco1;
		endState = TutorialManager.STATES.TutEco5;

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