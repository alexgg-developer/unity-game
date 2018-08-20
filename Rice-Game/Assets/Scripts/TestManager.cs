using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MyUtils.Pair;

public class TestManager
{
	private static TestManager instance = null;
	
	
	public static TestManager GetInstance()
	{
		if(instance == null)
		{
			instance = new TestManager();
		}
		
		return instance;
	}

	private TestManager() {}

	public void BuildAllModernBuildings()
	{
		BuildingsManager buildingsMan = BuildingsManager.GetInstance();
		WorldTerrain worldTerrain = WorldTerrain.GetInstance();
		Building building = buildingsMan.build(BUILDINGS.CASA, 25, 27);
		worldTerrain.createBuilding(building.getRow(), building.getCol(), building.getVerticalTiles(), building.getHorizontalTiles());
		building = buildingsMan.build(BUILDINGS.PLANTER, 27, 27);
		worldTerrain.createBuilding(building.getRow(), building.getCol(), building.getVerticalTiles(), building.getHorizontalTiles());
		building = buildingsMan.build(BUILDINGS.PLANTA, 29, 25);
		worldTerrain.createBuilding(building.getRow(), building.getCol(), building.getVerticalTiles(), building.getHorizontalTiles());
	}
	public void BuildAllBuildings()
    {
        BuildBuilding(BUILDINGS.CASA, 25, 27);
		BuildBuilding(BUILDINGS.PLANTER, 27, 27);
		BuildBuilding(BUILDINGS.TRILL, 25, 25);
		BuildBuilding(BUILDINGS.ERA, 27, 25);
		BuildBuilding(BUILDINGS.SILO, 29, 25);
    }
	private void BuildBuilding(BUILDINGS bID, uint i, uint j, bool pay=true) {
		WorldTerrain worldTerrain = WorldTerrain.GetInstance();
		BuildingsManager buildingsMan = BuildingsManager.GetInstance();
		Building building = buildingsMan.build(bID, i, j);
		worldTerrain.createBuilding(building.getRow(), building.getCol(), building.getVerticalTiles(), building.getHorizontalTiles());
		if (pay) {
			int buildingPrice = building.getInitialCost ();
			UserDataManager.GetInstance ().gold.espendGold (buildingPrice);
		}
	}

	public void AdvanceTillPhase(TypeFase phase, bool makeActions=true)
	{
		TimeManager tm = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
		PhaseManager pm = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>();

		int oldMonth = (int) tm.getCurrentMonth ();

		uint newDay = pm.getStartDayOfPhase(phase);
		uint newMonth = pm.getStartMonthOfPhase(phase);
		uint year = tm.getCurrentYear();
		tm.setDate(newDay, newMonth, year);
        pm.setCurrentPhase(phase);

		for (int i = 0; i < (newMonth-oldMonth+12)%12; ++i) {
			WorkerManager.GetInstance().payWorkers();
		}

		if (makeActions) {
			MakeActionsTillPhase (phase);
		}
    }

    public void MakeActionsTillPhase(TypeFase phase)
	{
		WorldTerrain wt = WorldTerrain.GetInstance();
        int ID_SIEMBRA_DIRECTA = 6;
        //List<Pair<uint, uint>> positions = WorldTerrain.GetInstance().getChunkTilesPositions(0);        
        foreach(TypeFase currentPhase in Enum.GetValues(typeof(TypeFase))) {
            if (currentPhase == phase) break;
            List<int> actionsCurrentPhase = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().getActionsInPhase(currentPhase);
            /*foreach (Pair<uint, uint> position in positions) {
                RiceTerrainTile tile = WorldTerrain.GetInstance().getRiceTerrain(position.First, position.Second);
                tile.performAction()
            }*/
            foreach(int actionID in actionsCurrentPhase) {
                if (actionID != ID_SIEMBRA_DIRECTA && ActionManager.GetInstance().hasAnActionToBeAddedAsDone(actionID)) {
					for (int i = 0; i < wt.getNumberOfChunks(); ++i) {
						wt.performActionInChunk (actionID, i, new List<int> (), true);
					}
                }
            }
        }        
    }

    public void BuyAllObjects(int quantity)
    {
        List<RiceObject> objects = RiceObjectsManager.GetInstance().getRiceObjectsAvailable();        
        foreach(RiceObject riceObject in objects) {
            for (int i = 0; i < quantity; ++i) {
                RiceObjectsManager.GetInstance().buyRiceObject(riceObject.id);
            }
        }
    }

    public void HireWorkers()
    {
        const int NUMBER_OF_WORKERS = 5;
        for (int i = 0; i < NUMBER_OF_WORKERS; ++i) {
            WorkerManager.GetInstance().hireWorker();
        }
    }

    public void SowPlanter()
    {
        Building_Planter p = (Building_Planter)BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTER);
        p.sembrar();
    }

	public void InvestigateEverything()
	{
		for(int i = 0; i < (int)INVESTIGATIONS_ID.QUANTITY; ++i) {
			InvestigationManager.GetInstance ().InvestigateWithoutWaiting (i);
		}
	}
}