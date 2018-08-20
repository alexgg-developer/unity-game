using UnityEngine;
using System.Collections;

public class Tutorial_Plantell : Tutorial {
	static public bool init=false;

	public Tutorial_Plantell() {
		initialState = TutorialManager.STATES.TutPlantell1;
		endState = TutorialManager.STATES.TutPlantellEnd;

		GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager>().cleanUI ();

		init = true;
	}

	public override void eventDone(TutorialManager.EVENTS ev) {
		switch (_state) {
		//case TutorialManager.STATES.TutIntro1:
		//	break;
		case TutorialManager.STATES.TutPlantell1:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				if (BuildingsManager.GetInstance ().isBuilded (BUILDINGS.PLANTER)) {
					changeToState (TutorialManager.STATES.TutPlantell_OK);
				} else {
					changeToNextState ();
				}
			}
			if (ev == TutorialManager.EVENTS.EXIT)
				changeToState (TutorialManager.STATES.END);
			break;
		case TutorialManager.STATES.TutPlantell_NO:
			if (ev == TutorialManager.EVENTS.MENU_BUY_PLANTELL) {
				cleanPanel ();
			}
			if (ev == TutorialManager.EVENTS.BUILDING_CONFIRMED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutPlantell3:
			if (ev == TutorialManager.EVENTS.PLANTELL_ACTIONS_OPENED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutPlantell4:
			if (ev == TutorialManager.EVENTS.PLANTELL_SEMBRAT) {
				changeToNextState ();
			}
			break;
		default:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP)
				changeToNextState ();
			if (ev == TutorialManager.EVENTS.EXIT)
				changeToState (TutorialManager.STATES.END);
			break;
		}
	}

}
