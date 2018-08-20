using UnityEngine;
using System.Collections;

public class Tutorial_ResearchMenu : Tutorial {
	static public bool init=false;

	public Tutorial_ResearchMenu() {
		initialState = TutorialManager.STATES.TutInv1PanelLostFocusLayer;
		endState = TutorialManager.STATES.TutInv4PanelLostFocusLayer;

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
