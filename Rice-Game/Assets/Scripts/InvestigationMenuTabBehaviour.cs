using UnityEngine;
using System.Collections;

public class InvestigationMenuTabBehaviour : MonoBehaviour {

	public InvestigationMenu.TAB index = InvestigationMenu.TAB.AVAILABLE;
	public InvestigationMenu investigationMenu;

	void Start () {
	}

	public void clicked(){
		Debug.Log ("Tab :: Clicked");
		investigationMenu.changeOfTab (index);
	}
}
