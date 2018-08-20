using UnityEngine;
using System.Collections;

public class PanelDefaultButton : MonoBehaviour {
	public enum DEFAULT_ACTIONS {OK=1, NO, CANCEL};

	public GameObject panel;
	public DEFAULT_ACTIONS id;

	public void clicked(){
		panel.SendMessage("actionClicked", (int) id);
	}
}
