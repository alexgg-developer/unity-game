using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

// Hay menus que tienen tantos elementos que no caben todos a la vez.
// Con este comportamiento se pueden hacer unos botones para moverse por las
//   diferentes paginas del menu.


public class MenuDirButtBehaviour : MonoBehaviour {
	public enum DIR {LEFT=1, RIGHT, UP, DOWN};

	public DIR dir = 0;
	public GameObject menu;

	public void selected()
    {
		menu.SendMessage ("SelectPageDir", dir);
	}
}
