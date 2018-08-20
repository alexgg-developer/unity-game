using UnityEngine;
using System.Collections;

public class BuyMenuTabBehaviour : MonoBehaviour {

	public BuyMenu.TAB index = BuyMenu.TAB.BUILDINGS;
	public BuyMenu buyMenu;

	void Start () {
	}

	public void clicked()
    {
		buyMenu.changeOfTab (index);
	}
}
