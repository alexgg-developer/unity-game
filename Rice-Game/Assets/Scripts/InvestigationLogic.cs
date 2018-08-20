using UnityEngine;
using System.Collections;

public class InvestigationLogic : MonoBehaviour {

	TimeManager tm;
	uint year;

	void Start () {
		tm = GetComponent<TimeManager> ();
		year = tm.getCurrentYear ();
	}

	void Update () {
		if (year < tm.getCurrentYear ()) {
			year = tm.getCurrentYear ();
			InvestigationManager.GetInstance ().newYear ();
			GameObject.FindWithTag ("InvestigationMenuParent").GetComponent<InvestigationMenu> ().newYear ();
		}
	}
}
