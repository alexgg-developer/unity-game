using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial_Inicial : Tutorial {
	private GameObject _tmpPanel;
	private TutorialManager _tutMan;

	public Tutorial_Inicial() {
		initialState = TutorialManager.STATES.TutInfoIntro1;
		endState = TutorialManager.STATES.TutInfoEnd;
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
	}

	public override void eventDone(TutorialManager.EVENTS ev) {
		switch (_state) {
		case TutorialManager.STATES.TutInfoIntro1:
			if (ev == TutorialManager.EVENTS.EXIT) {
				changeToState (TutorialManager.STATES.END);
			}
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoIntro3:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
				_tutMan._uiInfo.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
			}
			break;
		case TutorialManager.STATES.TutInfoCalen1:
			if (ev == TutorialManager.EVENTS.EXIT) {
				_tutMan._uiInfo.GetComponent<Image>().material = null;
				changeToState (TutorialManager.STATES.TutInfoHouse1);
			}
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoCalen5:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
				_tutMan._calendar.SetActive (true);
			}
			break;
		case TutorialManager.STATES.TutInfoCalen6:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
				_tutMan._calendar.SetActive (false);
				_tutMan._uiInfo.GetComponent<Image>().material = null;
				_tutMan._time1.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
				_tutMan._time2.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
				_tutMan._time3.GetComponent<Image>().material =_tutMan. _tutorialHighlightMat;
			}
			break;
		case TutorialManager.STATES.TutInfoTime:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
				_tutMan._time1.GetComponent<Image>().material = null;
				_tutMan._time2.GetComponent<Image>().material = null;
				_tutMan._time3.GetComponent<Image>().material = null;
			}
			break;
		case TutorialManager.STATES.TutInfoHouse1:
			if (ev == TutorialManager.EVENTS.EXIT) {
				changeToState (TutorialManager.STATES.TutInfoWorkers1);
			}
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				_tutMan._menu.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoHouse2:
			if (ev == TutorialManager.EVENTS.MENU_OPEN) {
				changeToNextState ();
				_tutMan._menu.GetComponent<Image>().material = null;

				_tutMan._SideMenu_Buy.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
				_tutMan._SideMenu_Upgr.GetComponent<Button> ().interactable = false;
				_tutMan._SideMenu_Inv.GetComponent<Button> ().interactable = false;
				_tutMan._SideMenu_Opt.GetComponent<Button> ().interactable = false;
			}
			break;
		case TutorialManager.STATES.TutInfoHouse3:
			if (ev == TutorialManager.EVENTS.MENU_BUY) {
				changeToNextState ();
				_tutMan._SideMenu_Buy.GetComponent<Image>().material = null;
				_tutMan._SideMenu_Upgr.GetComponent<Button> ().interactable = true;
				_tutMan._SideMenu_Inv.GetComponent<Button> ().interactable = true;
				_tutMan._SideMenu_Opt.GetComponent<Button> ().interactable = true;
			}
			break;
		case TutorialManager.STATES.TutInfoHouse5:
			if (ev == TutorialManager.EVENTS.MENU_BUY_HOUSE) {
				cleanPanel ();
			}
			if (ev == TutorialManager.EVENTS.BUILDING_CONFIRMED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoWorkers1:
			if (ev == TutorialManager.EVENTS.EXIT) {
				changeToState (TutorialManager.STATES.TutInfoAction1);
			}
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				_tutMan._WorkerCounter.GetComponent<Image>().material = _tutMan._tutorialHighlightMat;
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoWorkers4:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				_tutMan._WorkerCounter.GetComponent<Image>().material = null;
				GameObject workersPanel = Resources.Load ("Prefabs/WorkerRecruitmentlLostFocusLayer") as GameObject;
				_tmpPanel = GameObject.Instantiate (workersPanel);
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoWorkers8:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				GameObject.Destroy (_tmpPanel);
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoAction1:
			if (ev == TutorialManager.EVENTS.EXIT) {
				changeToState (TutorialManager.STATES.TutInfoEnd);
			}
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoAction2:
			if (ev == TutorialManager.EVENTS.RICE_CHUNK_ACTIONS_OPENED) {
				changeToNextState ();
			}
			break;
		case TutorialManager.STATES.TutInfoAction6:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP) {
				GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager> ().actionPanelClean ();
				changeToNextState ();
			}
			break;
		default:
			if (ev == TutorialManager.EVENTS.NEXT_POPUP)
				changeToNextState ();
			break;
		}
	}
}
