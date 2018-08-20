using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingConfirmFirstPanel : Panel {
	private TutorialManager _tutMan;

	void Awake() {
		initBase ();
	}

	public override void actionClicked(int id) {
		if (id == 0) {
			return;
		}
		switch ((PanelDefaultButton.DEFAULT_ACTIONS) id) {
		case PanelDefaultButton.DEFAULT_ACTIONS.CANCEL:
			logicManager.constructionBuildingCancel ();
			break;
		}
		kill();
	}

	public void disableConfirm()
	{
	}

	override public void kill()
	{
		Destroy(gameObject);
	}
}
