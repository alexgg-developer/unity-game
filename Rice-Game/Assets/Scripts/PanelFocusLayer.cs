using UnityEngine;
using System.Collections;

public class PanelFocusLayer : MonoBehaviour {

	private GameObject panel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPanel(GameObject newPanel) {
		panel = newPanel;
	}

	public void lostFocus() {
		panel.SendMessage ("lostFocus");
	}
}
