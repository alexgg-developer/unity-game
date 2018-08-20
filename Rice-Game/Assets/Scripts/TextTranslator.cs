using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextTranslator : MonoBehaviour {

    public Dictionary.TextType textMode;
    // Use this for initialization
    void Start () {
        Text t = gameObject.GetComponent<Text>();
        switch(textMode) {
            case Dictionary.TextType.NORMAL:
                if (t.text != null) {
                    t.text = Dictionary.getString(t.text);
                }
                break;
            case Dictionary.TextType.FirstUpper:
                t.text = Dictionary.getStringFUC(t.text);
                break;
            case Dictionary.TextType.Upper:
                t.text = Dictionary.getStringUC(t.text);
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
