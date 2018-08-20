using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoopLockBehaviour : MonoBehaviour {
    public Button buttonCoop;
    
    public void unlock()
    {
        buttonCoop.interactable = true;
        Destroy(gameObject);
    }
}
