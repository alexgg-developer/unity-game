using UnityEngine;
using System.Collections;

public class Tutorial_Buildings : Tutorial {
	static public bool init=false;

	public Tutorial_Buildings() {
		initialState = TutorialManager.STATES.TutBuildingsIntro1;
		endState = TutorialManager.STATES.TutBuildingsInfoEnd;

		GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager>().cleanUI ();

		init = true;
	}

	public override void eventDone(TutorialManager.EVENTS ev) {
		switch (_state) {
		case TutorialManager.STATES.TutBuildingsB1_1:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				if (BuildingsManager.GetInstance ().isBuilded (BUILDINGS.TRILL)) {
					changeToState (TutorialManager.STATES.TutBuildingsB1_OK);
				} else {
					changeToNextState ();
				}
			}
			break;
		case TutorialManager.STATES.TutBuildingsB1_NO:
			if (ev == TutorialManager.EVENTS.MENU_BUY_TRILL) {
				cleanPanel ();
			}
			if (ev == TutorialManager.EVENTS.BUILDING_CONFIRMED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutBuildingsB2_1:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				if (BuildingsManager.GetInstance ().isBuilded (BUILDINGS.ERA)) {
					changeToState (TutorialManager.STATES.TutBuildingsB2_OK);
				} else {
					changeToNextState ();
				}
			}
			break;
		case TutorialManager.STATES.TutBuildingsB2_NO:
			if (ev == TutorialManager.EVENTS.MENU_BUY_ERA) {
				cleanPanel ();
			}
			if (ev == TutorialManager.EVENTS.BUILDING_CONFIRMED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutBuildingsB3_1:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				if (BuildingsManager.GetInstance ().isBuilded (BUILDINGS.SILO)) {
					changeToState (TutorialManager.STATES.TutBuildingsB3_OK);
				} else {
					changeToNextState ();
				}
			}
			break;
		case TutorialManager.STATES.TutBuildingsB3_NO:
			if (ev == TutorialManager.EVENTS.MENU_BUY_SILO) {
				cleanPanel ();
			}
			if (ev == TutorialManager.EVENTS.BUILDING_CONFIRMED) {
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
