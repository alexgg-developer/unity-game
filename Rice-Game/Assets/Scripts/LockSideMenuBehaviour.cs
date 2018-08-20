using UnityEngine;
using System.Collections;

public class LockSideMenuBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //InvestigationManager.GetInstance().addListenerToCoopInvestigated(unlock);
    }

    private void unlock()
    {
        this.gameObject.SetActive(false);
        //InvestigationManager.GetInstance().removeListenerToCoopInvestigated(unlock);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
