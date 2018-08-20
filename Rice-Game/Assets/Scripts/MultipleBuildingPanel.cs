using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MultipleBuildingPanel : Panel
{
	private int _unitats, _cost;
	private Text _text;

	//private Button _confirmButton;

	void Start ()
	{
		initBase ();
		_unitats = 0;
		_cost = 0;
		_text = transform.Find ("Text").GetComponent<Text> ();
		//_confirmButton = transform.FindChild("ButtonConfirm").GetComponent<Button>();
		updateText ();
	}

	public void addUnits (int num_unitats, int cost)
	{
		_unitats += num_unitats;
		_cost += cost;
		updateText ();
	}

	private void updateText ()
	{
		_text.text = Dictionary.getString ("BUILD") + "  " + _unitats + " " + Dictionary.getString ("UNITS_PER") + " " + _cost + " " + Dictionary.getString ("COINS") + "?";
	}

	public int getCost ()
	{
		return _cost;
	}

	public void helpButtonClicked ()
	{
		if (logicManager.logicState != LogicManager.LOGIC_STATE.DELETING) {
			TutorialManager tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
			tutMan.startTuto (new Tutorial_ConstrCanal ());
		} else {
			TutorialManager tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
			tutMan.startTuto (new Tutorial_ConstrDelete ());
		}
	}

	override public void actionClicked (int id)
	{
		if (id == 0) {
			Debug.Log ("Building Panel :: Action Clicked ERROR");
			return;
		}
        
		switch ((PanelDefaultButton.DEFAULT_ACTIONS)id) {
		case PanelDefaultButton.DEFAULT_ACTIONS.OK:
			logicManager.constructionBuildingConfirm ();
			break;
		case PanelDefaultButton.DEFAULT_ACTIONS.NO:
			logicManager.constructionBuildingCancel ();
			break;
		case PanelDefaultButton.DEFAULT_ACTIONS.CANCEL:
			logicManager.constructionBuildingCancel ();
			break;
		}
		kill ();
	}

	override public void kill ()
	{
		Destroy (gameObject);
	}
}