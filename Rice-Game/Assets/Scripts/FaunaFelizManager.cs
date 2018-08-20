using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Pair;
using System.Linq;

public class FaunaFelizManager {

	private static FaunaFelizManager instance = null;

	public static FaunaFelizManager GetInstance()
	{
		if(instance == null)
		{
			instance = new FaunaFelizManager();
		}

		return instance;
	}

	private const int YEARS_SAMPLED=4;
	private const int MAX_FLAMINGOS=YEARS_SAMPLED*5/*5=max eco score*/;

	private List<uintPair> FlamingoPositions;
	private List<bool> freeFlamingoPositions;
	private Dictionary<uintPair, GameObject> instancesFlamingos;
	private Queue<int> scoresLastYears;

	private FaunaFelizManager()
	{
		FlamingoPositions = getFlamingoPositions ();

		freeFlamingoPositions = Enumerable.Repeat(true, FlamingoPositions.Count).ToList();

		instancesFlamingos = new Dictionary<uintPair, GameObject> ();

		scoresLastYears = new Queue<int> ();
		for (int i = 0; i < YEARS_SAMPLED; ++i) {
			scoresLastYears.Enqueue (0);
		}

		// TEST
		/*
		for (int i = 0; i < MAX_FLAMINGOS; ++i) {
			createFlamingo ();
		}
		*/
	}

	public void newYearUpdate() {
		//int newScore = Math.Min(5, instancesFlamingos.Count + 1); test
		int newScore = CoopManager.GetInstance ().getCurrentEcologyLevel ();
		scoresLastYears.Dequeue ();
		scoresLastYears.Enqueue (newScore);

		int newNumOfFlamingos = 0;
		foreach (int sc in scoresLastYears) {
			newNumOfFlamingos += sc;
		}

		int oldNumOfFlamingos = instancesFlamingos.Count;
		for (int i = 0; i < Math.Abs (newNumOfFlamingos - oldNumOfFlamingos); ++i) {
			if (newNumOfFlamingos > oldNumOfFlamingos) {
				createFlamingo ();
			} else {
				deleteFlamingo ();
			}
		}
	}

	private void createFlamingo() {
		Debug.Log ("Creating Flamingo...");
		int freePosID = getRandomFreePosID ();
		if (freePosID >= 0) {
			createInstFlamingo (FlamingoPositions [freePosID]);
			freeFlamingoPositions [freePosID] = false;
		}
	}

	private void deleteFlamingo() {
		Debug.Log ("Deleting Flamingo...");
		int notFreePosID = getRandomNotFreePosID ();
		if (notFreePosID >= 0) {
			deleteInstFlamingo (FlamingoPositions [notFreePosID]);
			freeFlamingoPositions [notFreePosID] = true;
		}
	}

	private int getRandomFreePosID() {
		List<int> freePoses = new List<int> ();
		for (int i = 0; i < freeFlamingoPositions.Count; ++i) {
			if (freeFlamingoPositions [i]) {
				freePoses.Add(i);
			}
		}
		return freePoses[UnityEngine.Random.Range (0, freePoses.Count ())] ;
	}

	private int getRandomNotFreePosID() {
		List<int> notFreePoses = new List<int> ();
		for (int i = 0; i < freeFlamingoPositions.Count; ++i) {
			if (!freeFlamingoPositions [i]) {
				notFreePoses.Add(i);
			}
		}
		return notFreePoses[UnityEngine.Random.Range (0, notFreePoses.Count ())] ;
	}

	private void createInstFlamingo(uintPair position) {
		try {
			uint row = position.First;
			uint col = position.Second;
			Debug.Log ("  instantiating Flamingo at "+row+" "+col);

			GameObject flamingoGameObject = Resources.Load<GameObject>(getRandomFlamingoPath());	
			WorldTerrain wt = WorldTerrain.GetInstance ();
			Vector3 worldPosition = wt.getTileWorldPosition(row, col);

			float map_top = wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);
			float map_h = wt.getTileWorldPositionY (0, wt.getNumTilesX () - 1) - wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);

			Vector3 flamingoSize = flamingoGameObject.GetComponent<SpriteRenderer> ().bounds.size;
			//worldPosition.x += WorldTerrain.TILE_WIDTH_UNITS/2 - flamingoSize.x / 2;
			worldPosition.y += flamingoSize.y*0.3f;

			worldPosition.z = WorldTerrain.BUILDING_Z_LAYER - (map_top - (worldPosition.y))/map_h;

			instancesFlamingos[position] = GameObject.Instantiate(flamingoGameObject, worldPosition, Quaternion.identity);
		}
		catch(Exception e) {
			Debug.LogError ("Error trying to instantiate flamingo");
			Debug.LogError (e.ToString());
		}

	}

	private void deleteInstFlamingo(uintPair position) {
		Debug.Log ("  deleting Flamingo at "+position.First+" "+position.Second);
		GameObject.Destroy (instancesFlamingos [position]);
		instancesFlamingos.Remove (position);
	}

	private string getRandomFlamingoPath() {
		string[] flamingoNames = new string[] {"flamenco1", "flamenco2", "flamenco3"};
		string flamingoName = flamingoNames [UnityEngine.Random.Range(0, flamingoNames.Count())];
		return "Textures/terrainTiles/"+flamingoName;
	}

	private List<uintPair> getFlamingoPositions() {
		WorldTerrain wt = WorldTerrain.GetInstance ();

		List<uintPair> newFlamingoPositions = new List<uintPair> ();
		while (newFlamingoPositions.Count < MAX_FLAMINGOS) {
			uintPair newPos = new uintPair ((uint) UnityEngine.Random.Range (0, 3), (uint) UnityEngine.Random.Range (0, (int) wt.getNumTilesX()));
			if (!wt.tileContainsVegetation (newPos.First, newPos.Second) && !newFlamingoPositions.Contains (newPos)) {
				newFlamingoPositions.Add (newPos);
			}
		}
		return newFlamingoPositions;
	}
}
