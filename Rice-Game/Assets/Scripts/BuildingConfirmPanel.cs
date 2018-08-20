using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingConfirmPanel : Panel {
	private bool confirmDisabled;
	private Button _confirmButton;
	private TutorialManager _tutMan;

	void Awake() {
		initBase ();

		_confirmButton = transform.FindChild ("BuildingConfirmPanel").FindChild ("Button_SI").GetComponent<Button>();

		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		if (_tutMan.getState() != TutorialManager.STATES.END) {
			transform.FindChild ("BuildingConfirmPanel").FindChild ("Button_Cancela").GetComponent<Button> ().interactable = false;
		}

		confirmDisabled = false;
	}

	public override void actionClicked(int id) {
		if (id == 0) {
			return;
		}
		switch ((PanelDefaultButton.DEFAULT_ACTIONS) id) {
		case PanelDefaultButton.DEFAULT_ACTIONS.OK:
			if (!confirmDisabled) {
				logicManager.constructionBuildingConfirm ();
				_tutMan.eventDone (TutorialManager.EVENTS.BUILDING_CONFIRMED);
			}
			break;
		case PanelDefaultButton.DEFAULT_ACTIONS.NO:
			break;
		case PanelDefaultButton.DEFAULT_ACTIONS.CANCEL:
			logicManager.constructionBuildingCancel ();
			break;
		}
		kill();
	}

	public void disableConfirm()
    {
		confirmDisabled = true;
		_confirmButton.interactable = false;
	}

	override public void kill()
    {
		Destroy(gameObject);
	}
}
