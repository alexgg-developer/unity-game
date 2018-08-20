using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroButtonsBehaviour : MonoBehaviour {

	private float t;
	private bool shown;
	public float waitTime;
	public GameObject playButton;
    public Button loadButton;
    public GameObject exitButton;
    public GameObject confirmationPanel;

    void Start () {
		t = 0.0f;
		shown = false;
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            ((Button)loadButton).interactable = false;
        }
    }
		
	void Update () {
		if (!shown){
			t += Time.deltaTime;
            if (t >= waitTime) {
                shown = true;
                playButton.SetActive(true);
                loadButton.gameObject.SetActive(true);
                exitButton.SetActive (true);
			}
		}
	}

	public void play(){
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            newGame();
        }
        else if(PlayerPrefs.HasKey("LoadData") && PlayerPrefs.GetInt("LoadData") == 1) {
            confirmationPanel.SetActive(true);
        }
    }

    public void newGame()
    {
        PlayerPrefs.SetInt("LoadData", 0);
        SceneManager.LoadScene("loading");
    }

    public void load()
    {
        PlayerPrefs.SetInt("LoadData", 1);
        SceneManager.LoadScene("loading");
    }

	public void exit()
    {
		Debug.Log ("App Quit!");
		Application.Quit ();
	}
}