using UnityEngine;
using System.Collections;

public class Tutorial_ConstructionMenu : Tutorial
{
	static public bool init = false;

	public Tutorial_ConstructionMenu ()
	{
		initialState = TutorialManager.STATES.TutConstruction1;
		endState = TutorialManager.STATES.TutConstruction5;

		//GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager>().cleanUI ();

		init = true;
	}

	public override void eventDone (TutorialManager.EVENTS ev)
	{
		if (ev == TutorialManager.EVENTS.NEXT_POPUP)
			changeToNextState ();
		if (ev == TutorialManager.EVENTS.EXIT)
			changeToState (TutorialManager.STATES.END);
	}
}
