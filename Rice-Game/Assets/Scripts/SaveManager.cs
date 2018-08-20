using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown (KeyCode.S))
			Save ();
		else if (Input.GetKeyDown (KeyCode.L))
			Load ();
	}

	void Save(){
		/*InvestigationManager im = InvestigationManager.GetInstance ();
		im.Save ();

		PlagueManager pm = PlagueManager.GetInstance ();
		pm.Save ();*/
	}

	void Load(){
		/*InvestigationManager im = InvestigationManager.GetInstance ();
		im.Load ();

		PlagueManager pm = PlagueManager.GetInstance ();
		pm.Load ();*/
	}
}
