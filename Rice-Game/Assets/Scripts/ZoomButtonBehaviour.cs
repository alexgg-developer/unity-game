using UnityEngine;
using System.Collections;

public class ZoomButtonBehaviour : MonoBehaviour {

    private int size = 8;
	
	public void clicked()
    {
        if (Camera.main.orthographicSize < 16) size = 16;
		else if (Camera.main.orthographicSize < 25) size = 25;
        else size = 8;

        Camera.main.orthographicSize = size;
    }
}
