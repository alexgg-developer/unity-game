using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

public class GUI_Pruebas : MonoBehaviour
{
	private string riceCounter, goldCounter;
	public TextAsset pruebaJson;

	void Awake()
	{

	}
	void OnGUI() {
		GUI.Label (new Rect(300, 15, 100, 50), riceCounter);
		GUI.Label (new Rect(300, 80, 100, 50), goldCounter);

		if(GUI.Button(new Rect(15, 15, 100, 50) , "AddRice&Gold")) {
			UserDataManager.GetInstance().gold.addGold(100);
			UserDataManager.GetInstance().rice.addRiceProduced(100);
			riceCounter = "Rice: " + UserDataManager.GetInstance().rice.getTotalRiceProduced();
			goldCounter = "Gold: " + UserDataManager.GetInstance().gold.getGold();
		}

		if(GUI.Button(new Rect(15, 80, 100, 50) , "SubsRice&Gold")) {
			UserDataManager.GetInstance().gold.addGold(-100);
			UserDataManager.GetInstance().rice.addRiceProduced(10);
			riceCounter = "Rice: " + UserDataManager.GetInstance().rice.getTotalRiceProduced();
			goldCounter = "Gold: " + UserDataManager.GetInstance().gold.getGold();
		}

		if(GUI.Button(new Rect(15, 145, 100, 50) , "Save")) {
			//GameSaveDataManager.Save();
		}
		
		if(GUI.Button(new Rect(15, 210, 100, 50) , "Load")) {
			//GameSaveDataManager.Load();
		}

		if(GUI.Button(new Rect(15, 275, 100, 50) , "Print")) {
			//GameSaveDataManager.printData();
		}

		if(GUI.Button(new Rect(15, 340, 100, 50) , "Create terrain")) {
			//Camera.main.GetComponent<RiceTerrain>().createTerrain();
		}

		if(GUI.Button(new Rect(15, 405, 100, 50) , "Print json")) {

			//JSONLoader.loadPhaseActions();

			/*TextAsset file;
			//Debug.Log("Loading from::" + Application.dataPath + "/json/prueba.json");
			//file = (TextAsset) Resources.Load (Application.dataPath + "/prueba");
			//string path = "C:/Users/NOT_FREE/Unity/Rice-Game/Rice-Game/Assets/json/prueba.json";
			//file = (TextAsset) Resources.Load ("json/prueba");
			//file = pruebaJson;
			file = Resources.Load("json/prueba", typeof(TextAsset)) as TextAsset;
			
			if (file == null) {
				Debug.Log("file not loaded");
			}
			else 
			{
				Debug.Log("File Exists"); 
				Debug.Log(file.text);
				JSONNode node = JSON.Parse(file.text);
				string val = node["data"]["sampleArray"][0];
				Debug.Log("val::: " + val);
			}*/
		}
	}
}

