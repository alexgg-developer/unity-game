using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SideMenu : MonoBehaviour {

	public GameObject close;
	public GameObject open;

	public bool opened;

	private TutorialManager _tutMan;
	private LogicManager _logicMan;

	void Start(){
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		_logicMan = GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager> ();
		opened = false;
	}

	void Update () {
		if (opened && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
			closeMenu();
		}
	}

	public void openMenu(){
		if (opened)
			return;
		if (_tutMan.getState () == TutorialManager.STATES.END ||
			_tutMan.getState () == TutorialManager.STATES.TutInfoHouse2 ||
			_tutMan.getState () == TutorialManager.STATES.TutPlantell_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB1_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB2_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB3_NO) {

			if (_logicMan.logicState != LogicManager.LOGIC_STATE.DEFAULT) {
				return;
				//_logicMan.constructionBuildingCancel ();
			}

			opened = true;
			GetComponent<Animator> ().SetTrigger ("OpenMenu");
			open.SetActive (false);
			close.SetActive (true);
			_tutMan.eventDone (TutorialManager.EVENTS.MENU_OPEN);
		}
	}

	public void closeMenu(){
		if (!opened)
			return;
		if (_tutMan.getState () != TutorialManager.STATES.TutInfoHouse3) {
			opened = false;
			GetComponent<Animator> ().SetTrigger ("CloseMenu");
			close.SetActive (false);
			open.SetActive (true);
			//buy.SetActive(false);
			//upgrade.SetActive(false);
			//settings.SetActive(false);
		}
	}
}
