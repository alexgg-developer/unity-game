using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using MyUtils.Pair;

public class TutorialManager : MonoBehaviour {
	public enum STATES {
		TutInfoIntro1,
		TutInfoIntro2,
		TutInfoIntro3,
		TutInfoCalen1,
		TutInfoCalen2,
		TutInfoCalen3,
		TutInfoCalen4,
		TutInfoCalen5,
		TutInfoCalen6,
		TutInfoTime,
		TutInfoHouse1,
		TutInfoHouse2,
		TutInfoHouse3,
		TutInfoHouse4,
		TutInfoHouse5,
		TutInfoHouse6,
		TutInfoWorkers1,
		TutInfoWorkers2,
		TutInfoWorkers3,
		TutInfoWorkers4,
		TutInfoWorkers5,
		TutInfoWorkers6,
		TutInfoWorkers7,
		TutInfoWorkers8,
		TutInfoAction1,
		TutInfoAction2,
		TutInfoAction3,
		TutInfoAction4,
		TutInfoAction5,
		TutInfoAction6,
		TutInfoEnd,

		TutPlantell1,
		TutPlantell_NO,
		TutPlantell_OK,
		TutPlantell3,
		TutPlantell4,
		TutPlantellEnd,

		TutBuildingsIntro1,
		TutBuildingsIntro2,
		TutBuildingsB1_1,
		TutBuildingsB1_NO,
		TutBuildingsB1_OK,
		TutBuildingsB2_1,
		TutBuildingsB2_NO,
		TutBuildingsB2_OK,
		TutBuildingsB3_1,
		TutBuildingsB3_NO,
		TutBuildingsB3_OK,
		TutBuildingsInfo1,
		TutBuildingsInfo2,
		TutBuildingsInfo3,
		TutBuildingsInfo4,
		TutBuildingsInfoEnd,

        TutInv1PanelLostFocusLayer,
        TutInv2PanelLostFocusLayer,
        TutInv3PanelLostFocusLayer,
        TutInv4PanelLostFocusLayer,

		TutConstruction1,
		TutConstruction2,
		TutConstruction3,
		TutConstruction4,
		TutConstruction5,

		TutConstrCanal1,
		TutConstrCanal2,
		TutConstrCanal3,
		TutConstrCanal4,
		TutConstrCanal5,
		TutConstrCanal6,
		TutConstrCanal7,

		TutConstrDelete1,
		TutConstrDelete2,
		TutConstrDelete3,
		TutConstrDelete4,
		TutConstrDelete5,
		TutConstrDelete6,

		TutPlagas1,
		TutPlagas2,
		TutPlagas3,
		TutPlagas4,
		TutPlagas5,
		TutPlagas6,

		TutCope1,
		TutCope2,
		TutCope3,
		TutCope4,
		TutCope5,
		TutCope6,
		//TutCope7,
		//TutCope8,
		//TutCope9,
		//TutCope10,
		TutCope11,

		TutEco1,
		TutEco2,
		TutEco3,
		TutEco4,
		TutEco5,

        END
	};
	public enum EVENTS {
		NEXT_POPUP,
		EXIT,
		MENU_OPEN,
		MENU_BUY,
		MENU_BUY_HOUSE,
		BUILDING_CONFIRMED,
		RICE_CHUNK_ACTIONS_OPENED,
		MENU_BUY_PLANTELL,
		PLANTELL_ACTIONS_OPENED,
		PLANTELL_SEMBRAT,
		MENU_BUY_TRILL,
		MENU_BUY_ERA,
		MENU_BUY_SILO
    };

	/*
	// SINGLETON
	private static TutorialManager instance = null;
	public static TutorialManager GetInstance()
	{
		return instance;
	}*/

	void Awake() {
		//instance = this;
		_tutorialHighlightMat = Resources.Load ("Materials/TutorialHighlight") as Material;
		_disabled = false;
	}

	// ATRIBUTS
	public Material _tutorialHighlightMat;

	public GameObject _uiInfo, _calendar, _time1, _time2, _time3, _menu;
	public GameObject _SideMenu_Buy, _SideMenu_Upgr, _SideMenu_Inv, _SideMenu_Coop, _SideMenu_Opt;
	public GameObject _WorkerCounter;

	private Tutorial _tutorial;
	private bool _disabled;

	public void startTuto(Tutorial tuto) {
		if (!_disabled && getState() == STATES.END) {
			_tutorial = tuto;
			_tutorial.startTuto ();
		}
	}

	public TutorialManager.STATES getState() {
		if (_tutorial == null) {
			return STATES.END;
		}
		return _tutorial.getState();
	}

	public void eventDone(EVENTS ev) {
		if (_tutorial != null) {
			_tutorial.eventDone(ev);
		}
	}

	public void disableTutorial() {
		_disabled = true;
	}
	// FUNCIONS

	/*
	private void init () {
		//_uiInfo = GameObject.Find ("Information");
		//_calendar = GameObject.Find ("CalendarPhasePanelLostFocusLayer");
	}
	*/

	public void save(TutorialManagerData tutorialManagerData) {

		tutorialManagerData.TutorialBuildingsInit = Tutorial_Buildings.init;
		tutorialManagerData.TutorialConstructCanalInit = Tutorial_ConstrCanal.init;
		tutorialManagerData.TutorialConstructDeleteInit = Tutorial_ConstrDelete.init;
		tutorialManagerData.TutorialConstructionMenuInit = Tutorial_ConstructionMenu.init;
		tutorialManagerData.TutorialPlagasInit = Tutorial_Plagas.init;
		tutorialManagerData.TutorialPlantellInit = Tutorial_Plantell.init;
		tutorialManagerData.TutorialResearchMenuInit = Tutorial_ResearchMenu.init;
		tutorialManagerData.TutorialCopeInit = Tutorial_Cope.init;
		tutorialManagerData.TutorialEcoInit = Tutorial_Ecology.init;
	}

	public void load(TutorialManagerData tutorialManagerData) {
		Tutorial_Buildings.init = tutorialManagerData.TutorialBuildingsInit;
		Tutorial_ConstrCanal.init = tutorialManagerData.TutorialConstructCanalInit;
		Tutorial_ConstrDelete.init = tutorialManagerData.TutorialConstructDeleteInit;
		Tutorial_ConstructionMenu.init = tutorialManagerData.TutorialConstructionMenuInit;
		Tutorial_Plagas.init = tutorialManagerData.TutorialPlagasInit;
		Tutorial_Plantell.init = tutorialManagerData.TutorialPlantellInit;
		Tutorial_ResearchMenu.init = tutorialManagerData.TutorialResearchMenuInit;
		Tutorial_Cope.init = tutorialManagerData.TutorialCopeInit;
		Tutorial_Ecology.init = tutorialManagerData.TutorialEcoInit;
	}
}

public abstract class Tutorial {
	protected TutorialManager.STATES initialState, endState;
	protected TutorialManager.STATES _state;
	protected GameObject _panel;

	private TimeManager.SPEED_MODE _timeMode;

	public virtual void startTuto() {
		Debug.Log ("Start Tutorial");

		TimeManager timeManager = GameObject.Find ("Logic").GetComponent<TimeManager>();
		_timeMode = timeManager.getCurrentMode ();
		timeManager.pauseTime (true);

		_panel = null;
		_state = initialState;
		loadPanel (_state);
	}

	public TutorialManager.STATES getState() {
		return _state;
	}

	public bool isFinished() {
		return _state != TutorialManager.STATES.END;
	}

	public abstract void eventDone (TutorialManager.EVENTS ev);

	protected void changeToNextState() {
		changeToState (_state + 1);
	}

	protected void changeToState(TutorialManager.STATES st) {
		Debug.Log ("Tutorial : changeToState "+st.ToString());
		_state = st;
		cleanPanel ();
		if (st <= endState) {
			loadPanel (st);
		} else {
			endTutorial ();
		}
	}

	protected void endTutorial() {
		Debug.Log ("Tutorial : End Of Tuto");
		_state = TutorialManager.STATES.END;

		TimeManager timeManager = GameObject.Find ("Logic").GetComponent<TimeManager>();
		//timeManager.startTime (true);
		if (_timeMode != TimeManager.SPEED_MODE.PAUSE) {
			timeManager.changeMode (_timeMode);
		}
	}

	protected void cleanPanel() {
		if (_panel != null)
			GameObject.Destroy (_panel);
	}

	protected void loadPanel(TutorialManager.STATES st) {
		if (_panel != null)
			GameObject.Destroy (_panel);

		GameObject panelTemplate = Resources.Load("Prefabs/Tutorial/"+st.ToString()) as GameObject;
		_panel = GameObject.Instantiate(panelTemplate);
		_panel.GetComponent<RectTransform>().SetParent(GameObject.Find ("UICanvas").GetComponent<RectTransform>(), false); 
	}
}

