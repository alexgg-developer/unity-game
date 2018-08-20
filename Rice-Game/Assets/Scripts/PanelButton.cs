using UnityEngine;
using System.Collections;

public class PanelButton : MonoBehaviour {
    public GameObject AttachedPanel;
	public int id;

	public void clicked(){
        AttachedPanel.SendMessage("actionClicked", (int) id);
	}
}


