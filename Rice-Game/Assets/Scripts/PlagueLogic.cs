using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlagueLogic : MonoBehaviour {

	private PlagueManager pm;
	private TimeManager tm;
    private Plague activePlague;
    private int remainingChunks;
	private int plagueCount;
	private bool plagueHappening;
	private uint lastDay, lastMonth;
    private uint plagueInitialDay;
    private bool[] visitedChunk;

	void Start () {
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            
        }
        init();
    }

	void init(){
		pm = PlagueManager.GetInstance ();
		tm = GetComponent<TimeManager> ();

		plagueHappening = false;
		plagueCount = pm.getPlagueCount ();

		lastDay = tm.getCurrentDay ();
		lastMonth = tm.getCurrentMonth ();

        remainingChunks = 0;
	}

	void Update () {
		if (lastMonth != tm.getCurrentMonth ()) {
			lastMonth = tm.getCurrentMonth ();
			pm.resetPlagues ();
		}

		if (!plagueHappening) {
			if (tm.getCurrentDay () != lastDay) {
				lastDay = tm.getCurrentDay ();
				int id = checkPlagues ();
				if (id == -1)
					return;
				executePlague (id);
			}
		} else {
			updateLogic ();
		}
	}

	int checkPlagues(){
		for (int i = 0; i < plagueCount; i++) {
			Plague p = pm.getPlague (i);
			if (p.alreadyHappened())
				continue;
			if (p.getMonthIn () > tm.getCurrentMonth ()+1 || p.getMonthOut () < tm.getCurrentMonth ()+1)
				continue;
			int prob = Random.Range (1, 100);
			if (prob <= p.getSpawnChance () - InvestigationManager.GetInstance().getPlagueDecreaseBonus()) {
				return i;
			}
		}
		return -1;
	}

	public void NewDayCallback()
	{
	}


	void updateLogic(){
		//WorldTerrain wt = WorldTerrain.GetInstance();
		uint diff = tm.getCurrentDay() - plagueInitialDay;
		/*if (((int)diff % (activePlague.getBaseDmg()*checkInsecticida())) == 0){ 
			for (int i = 0; i < wt.getNumberOfChunks(); i++){
				if (!visitedChunk[i] && (activePlague.getID() != 1 || (activePlague.getID() == 1 && !subvencioInvest()))){
					PenalizationManager.GetInstance().addPenalization(i);                    
				}
			}
		}*/
		if (diff > 60)
			cleanPlague ();
	}

    void executePlague(int id) {
        plagueHappening = true;
        activePlague = pm.getPlague(id);
        plagueInitialDay = tm.getCurrentDay();
        remainingChunks = 0;

        WorldTerrain wt = WorldTerrain.GetInstance();
        int tilesPerChunk = wt.getTotalTilesInAChunk();
		visitedChunk = new bool[wt.getNumberOfChunks()];
		for (int i = 0; i < wt.getNumberOfChunks (); i++)
			visitedChunk [i] = true;
        for (int i = 0; i < wt.getNumberOfChunks(); i++)
        {
            int rnd = Random.Range(1, 3);
			if ((rnd < 3 || remainingChunks == 0) && wt.isChunkPlanted(i))
            {
				visitedChunk[i] = false;
                remainingChunks++;
                uint randomTile = (uint)((UnityEngine.Random.value) * (float)(tilesPerChunk - 1));
                wt.updatePlagueStateInTile(i, (int)randomTile, plagueType(activePlague.getID()));
            }
        }
		if (remainingChunks == 0)
			cleanPlague ();
		else
			activePlague.setActive(true);
	}

	void cleanPlague(){
        activePlague.setActive(false);
        activePlague = null;
        plagueHappening = false;
		//WorldTerrain wt = WorldTerrain.GetInstance();
		for (int i = 0; i < visitedChunk.Length; i++)
			visitedChunk [i] = true;
	}

    public void ClearOneChunk(int id)
    {
		if (visitedChunk != null && visitedChunk.Length > id) {
			//TO DO: CAREFUL SOMETIMES THIS IS NULL IDK WHY
			if (!visitedChunk [id]) {
				remainingChunks--;
				visitedChunk [id] = true;

				if (activePlague.getID () == 1)
					CoopManager.GetInstance ().loseEcologyPlageControlPoint ();

				if (remainingChunks == 0) {
					cleanPlague ();
				}
			}
		}
    }

    public TypePlague plagueType(int id)
    {
        switch (id)
        {
        case 0:
            return TypePlague.CARGOL_POMA;
		case 1:
			int rnd = Random.Range (1, 3);
			if (rnd == 1)
				return TypePlague.FLAMENCO1;
			if (rnd == 2)
				return TypePlague.FLAMENCO2;
			return TypePlague.FLAMENCO3;
        case 2:
            return TypePlague.QUIRONOMIDS;
        }
        return TypePlague.NONE;
    }

	public bool plagueIsHappening(){
		return plagueHappening;
	}
}
