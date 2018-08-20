using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public delegate void DoMenuAction( );
public delegate void PerformTileAction(int id);
public delegate void PerformChunkAction(ChunkAction action);

public class MenuAction 
{
	public bool enabled;
	public bool isRequired;
	public bool hasPenalization;
	public int id; 
	public string title;
	public string info;
	public string iconImgPath;
    public int workersNeeded;
    protected List<int> _objectRequired;
	public DoMenuAction doMenuAction;
	public int priority;


	public List<int> getListOfObjectsRequired()
	{
		return _objectRequired;
	}

}

public class ChunkAction: MenuAction {
	public PerformChunkAction performChunkAction;
	private Dictionary<string, ArrayList> _dependencies;
	public int chunk;
	public int duration;
	public int tilesWorked;
	public int hoursElapsed;
	public bool isFinished;
	public bool addAsDone;
    public bool needCanal;
	private List<string> _animation;
	private List<int> _investigationRequired;
	private List<int> _actionPartners;


	public ChunkAction(int aID, bool aIsRequired, bool aHasPenalization, string aTitle, string aInfo, int aDuration, bool hasToAddAsDone, bool isCanalNeeded, int neededWorkers, int aPriotity) 
	{		
		id = aID;
		enabled = true;
		isRequired = aIsRequired;
		hasPenalization = aHasPenalization;
		title = aTitle;
		info = aInfo;
		iconImgPath = "";
		performChunkAction = null;
		_dependencies = new Dictionary<string, ArrayList>();
		doMenuAction = performAction;
		chunk = -1;
		duration = aDuration;
		tilesWorked = 0;
		hoursElapsed = 0;
		isFinished = false;
		addAsDone = hasToAddAsDone;
        needCanal = isCanalNeeded;
        workersNeeded = neededWorkers;
        _objectRequired = new List<int> ();
		_animation = new List<string>();
		_investigationRequired = new List<int> ();
		_actionPartners = new List<int> ();
		priority = aPriotity;
	}

    public bool hasAnimation()
    {
        return _animation.Count > 0;
    }

	public void addDependency(string key, string value)
	{
		//_dependencies.Add(newKey, 
		if(!_dependencies.ContainsKey(key)) {
			ArrayList values = new ArrayList();
			values.Add(value);
			_dependencies.Add(key, values);
		}
		else {
			_dependencies[key].Add(value);
		}
	}

	public Dictionary<string, ArrayList> getDependencies()
	{
		return _dependencies;
	}

	public ChunkAction copyWithCallback(PerformChunkAction callback, int chunk) 
	{
		ChunkAction newAction = new ChunkAction (this.id, this.isRequired, this.hasPenalization, this.title, this.info, this.duration, this.addAsDone, this.needCanal, this.workersNeeded, this.priority);
		newAction.performChunkAction = callback;
		newAction._objectRequired = this._objectRequired;
		newAction._animation = this._animation;
        newAction._investigationRequired = this._investigationRequired;
        newAction._actionPartners = this._actionPartners;
        newAction.chunk = chunk;
		newAction.priority = priority;

		return newAction;
	}

	public void performAction()
	{
		performChunkAction (this);
		//traditionalAnimation = "animations/airear/airear_animation"; //TEMP
		//traditionalAnimation = "Textures/terrainTiles/RiceTerrain_WATER"; //TEMP
		if (_animation.Count > 0) {
			//ChunkAnimation animation = new ChunkAnimation (_animation [0], duration);
			WorldTerrain.GetInstance ().startActionAnimationInAChunk (_animation [0], duration, chunk);
		}
		this.useObjects ();
        WorkerManager.GetInstance().BusyWorkers += this.workersNeeded;
	}

    public void starAnimationWithRemainingDistance(float remainingDistance)
    {
        //ChunkAnimation animation = new ChunkAnimation(_animation[0], duration);
        //animation.remainingDistance = remainingDistance;
		WorldTerrain.GetInstance().startActionAnimationInAChunk(_animation [0], duration, chunk, remainingDistance);

    }

	public void addAnHourWorked()
	{
		//Debug.Log ("Action::AddAnHourWorked ID="+id);
		++hoursElapsed;
		int tilesChunk = WorldTerrain.GetInstance ().getTotalTilesInAChunk ();
		if (((tilesChunk * hoursElapsed) / duration) > tilesWorked) {
			//Debug.Log (".... ++tilesWorked");
			++tilesWorked;
			WorldTerrain.GetInstance ().performActionInTileOfAChunk (id, chunk, tilesWorked, _actionPartners, addAsDone);
			if (tilesWorked == tilesChunk) {
				isFinished = true;
				this.returnObjects ();
				WorkerManager.GetInstance().BusyWorkers -= this.workersNeeded;
				//Debug.Log (".... ActionFinished!");
            }
		}
	}

	public void addObjectRequired(int id)
	{
		_objectRequired.Add (id);
	}

	public void addAnimation(string anAnimation)
	{
		_animation.Add (anAnimation);
	}

	public void addInvestigationRequired(int id)
	{
		_investigationRequired.Add (id);
	}

	public void addActionPartner(int id)
	{
		_actionPartners.Add (id);
	}

	public void useObjects()
	{
		for (int i = 0; i < _objectRequired.Count; ++i) {
			RiceObjectsManager.GetInstance ().useObject (_objectRequired[i]);
		}
	}

	public void returnObjects()
	{
		for (int i = 0; i < _objectRequired.Count; ++i) {
			RiceObjectsManager.GetInstance ().returnObject (_objectRequired[i]);
		}
	}

	public List<int> getInvestigationRequired()
	{
		return _investigationRequired;
	}

	public List<int> getActionPartners()
	{
		return _actionPartners;
	}

}

public class ActionManager {
	// SINGLETON
	private static ActionManager instance = null;
	private Dictionary<int, ChunkAction> _action;
	private List<ChunkAction> _actionInProgress;

    public static ActionManager GetInstance()
	{
		if(instance == null)
		{
			instance = new ActionManager();
		}
		
		return instance;
	}
	
	private ActionManager() 
	{
		_action = JSONLoader.readActions ();
		_actionInProgress = new List<ChunkAction> ();
		GameObject.Find ("Logic").GetComponent<TimeManager>().addListenerToHourChange(newHourCallback);
	}

	// --- PUBLIC GETTERS ---
	public List<MenuAction> getBuildingActions(BUILDINGS buildingType)
	{
		switch (buildingType) {
		    case BUILDINGS.CASA:
			    return getBCasaActions();
		    case BUILDINGS.ESTABLO:
			    return getBEstabloActions();
            //case BUILDINGS.GARAJE:
            //    return getBGarageActions();
            case BUILDINGS.PLANTER:
			    return getBPlanterActions();
		    case BUILDINGS.TRILL:
			    return getBTrillActions();
		    case BUILDINGS.ERA:
			    return getBEraActions();
		    case BUILDINGS.SILO:
			    return getBSiloActions();
			default:
				return new List<MenuAction> ();
		    }
	}

	public List<MenuAction> getRiceTerrainActions(RiceTerrainTile riceTerrain)
    {
		//Debug.Log ("Actions for Terrain=" + riceTerrain.getChunkNumber ());
        List<MenuAction> actionsAvailable = new List<MenuAction>();
        if (!isActionInProgress(riceTerrain.getChunkNumber())) {
            List<int> actionsCurrentPhase = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().getActionsInCurrentPhase();
            for (uint i = 0; i < actionsCurrentPhase.Count; ++i) {
                int currentAction = actionsCurrentPhase[(int)i];

                bool areDependenciesOk = riceTerrain.checkDependencies(_action[currentAction]);
                bool hasBeenInvestigated = InvestigationManager.GetInstance().areInvestigated(_action[currentAction].getInvestigationRequired());
				bool hasWater = WorldTerrain.GetInstance().riceChunkHasWater(riceTerrain.getChunkNumber ()) || !_action[currentAction].needCanal;
				bool hasActionDone = riceTerrain.isActionDone (currentAction);

				if (!hasActionDone && hasBeenInvestigated && areDependenciesOk && hasWater) {
                    ChunkAction newAction = _action[currentAction];
                    PerformChunkAction callback = this.addActionInProgress;
                    ChunkAction t = newAction.copyWithCallback(callback, riceTerrain.getChunkNumber());
                    actionsAvailable.Add((MenuAction)t);
                }
				//Debug.Log("  Action "+currentAction+" Dep="+areDependenciesOk+" Inv="+hasBeenInvestigated+" Water="+hasWater+" !Done="+!hasActionDone);
            }
        }
		actionsAvailable.Sort((x, y) => x.priority.CompareTo(y.priority));
		return actionsAvailable;
	}

	// Getters Building Actions
	private List<MenuAction> getBCasaActions()
	{
		List<MenuAction> actions = new List<MenuAction> ();
        Building_Home casa = (Building_Home)BuildingsManager.GetInstance ().getBuilding (BUILDINGS.CASA);
        MenuAction action = new MenuAction();
        action.title = Dictionary.getString("HIRE");
        action.enabled = true;
        action.doMenuAction = new DoMenuAction(casa.openHirePanel);
        actions.Add(action);

        return actions;
	}

	private List<MenuAction> getBEstabloActions()
	{

        List<MenuAction> actions = new List<MenuAction>();
        Building_Stable building = (Building_Stable)BuildingsManager.GetInstance().getBuilding(BUILDINGS.ESTABLO);
        MenuAction action = new MenuAction();
        action.title = Dictionary.getString("BUY");
        action.enabled = true;
        action.doMenuAction = new DoMenuAction(building.openBuyMenu);
        actions.Add(action);

        return actions;
    }

    private List<MenuAction> getBGarageActions()
    {
        //List<MenuAction> actions = new List<MenuAction>();
        //Building_Garage building = (Building_Garage)BuildingsManager.GetInstance().getBuilding(BUILDINGS.GARAJE);
        //MenuAction action = new MenuAction();
        //action.title = Dictionary.getString("BUY");
        //action.enabled = true;
        //action.doMenuAction = new DoMenuAction(building.openBuyMenu);
        //actions.Add(action);

        //return actions;
        return new List<MenuAction>();
    }

    private List<MenuAction> getBPlanterActions()
	{
        List<MenuAction> actions = new List<MenuAction>();
        Building_Planter building = (Building_Planter)BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTER);
		MenuAction actSembrar = new MenuAction();
		actSembrar.title = Dictionary.getString("SOW");
        actSembrar.info =  Dictionary.getString("SOWING_RICE");
        bool isActionEnabled = !building.estaSembrat();
		isActionEnabled = isActionEnabled && building.esEpocaDeSembrar();
        actSembrar.enabled = isActionEnabled;
		actSembrar.doMenuAction = new DoMenuAction(building.sembrar);
		actions.Add(actSembrar);

		return actions;
	}

	private List<MenuAction> getBTrillActions()
	{
		List<MenuAction> actions = new List<MenuAction> ();
		Building_Trill building = (Building_Trill) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.TRILL);
		Building_Era buildingEra = (Building_Era) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.ERA);
		MenuAction actSendRice = new MenuAction();
		if (buildingEra.getCurrentFreeCapacity () == 0) {
			actSendRice.title = Dictionary.getString ("ERA_FILLED");
		} else {
			actSendRice.title = Dictionary.getString ("SEND_RICE_TO_ERA");
		}
		uint riceToSend = Math.Min(buildingEra.getCurrentFreeCapacity(), building.getRicePrepared());
		actSendRice.info = Dictionary.getString("SEND") + " " + riceToSend + " " + Dictionary.getString("KG_RICE_TERRAIN");
		actSendRice.enabled = (riceToSend > 0);
		actSendRice.doMenuAction = new DoMenuAction(ActSendRiceToEra);
		actions.Add(actSendRice);
		return actions;
	}

	private List<MenuAction> getBEraActions()
	{
		List<MenuAction> actions = new List<MenuAction> ();
		Building_Era building = (Building_Era) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.ERA);	
		Building_Silo buildingSilo = (Building_Silo) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.SILO);	
		MenuAction actSendRice = new MenuAction();
		actSendRice.title = Dictionary.getString ("SEND_RICE_TO_SILO");
		uint riceToSend = Math.Min(buildingSilo.getCurrentFreeCapacity(), building.getRicePrepared());
		actSendRice.info = Dictionary.getString("SEND") + " " + riceToSend + " " + Dictionary.getString("KG_RICE_SILO");
        actSendRice.enabled = (building.getRicePrepared() > 0);
		actSendRice.doMenuAction = new DoMenuAction(ActSendRiceToSilo);
		actions.Add(actSendRice);

		return actions;
	}

	private List<MenuAction> getBSiloActions()
	{
		List<MenuAction> actions = new List<MenuAction> ();
		return actions;
	}

	// --- DoMenuActions--- 
	private void ActUpgradeCasa() { upgradeBuilding (BUILDINGS.CASA); }
	private void ActUpgradeEstablo() { upgradeBuilding (BUILDINGS.ESTABLO); }
	private void ActUpgradePlanter() { upgradeBuilding (BUILDINGS.PLANTER); }
	private void ActUpgradeTrill() { upgradeBuilding (BUILDINGS.TRILL); }
	private void ActUpgradeEra() { upgradeBuilding (BUILDINGS.ERA); }
	private void ActUpgradeSilo() { upgradeBuilding (BUILDINGS.SILO); }

	private void ActSendRiceToEra() {
		Building_Trill trill = (Building_Trill) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.TRILL);
		Building_Era era = (Building_Era) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.ERA);
		//uint ricePrepared = trill.getAndEraseRicePrepared ();
		uint riceToSend = Math.Min(era.getCurrentFreeCapacity(), trill.getRicePrepared());
		trill.takeRicePrepared(riceToSend);
		era.sendRice (riceToSend);
	}
    

	private void ActSendRiceToSilo() {
		Building_Era era = (Building_Era) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.ERA);
		Building_Silo silo = (Building_Silo) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.SILO);
		//uint ricePrepared = era.getAndEraseRicePrepared ();
		uint riceToSend = Math.Min(silo.getCurrentFreeCapacity(), era.getRicePrepared());
		era.takeRicePrepared(riceToSend);
		silo.sendRice (riceToSend);
	}


	// ---AUX ---
	private void upgradeBuilding(BUILDINGS buildingType) {
		Building b = BuildingsManager.GetInstance ().getBuilding (buildingType);
		GoldReserve goldReserve = UserDataManager.GetInstance().gold;
		goldReserve.espendGold (b.getNextLevelCost ());
		b.upgrade ();
	}

	public void addActionInProgress(ChunkAction action) 
	{
		_actionInProgress.Add (action);
        //actions that have consequences per chunk not per tile
        if (action.id == 18 || action.id == 25) {
            Building_Planter planter = (Building_Planter) BuildingsManager.GetInstance ().getBuilding (BUILDINGS.PLANTER);
            planter.agafarPlanta ();
        }

    }

    public bool isActionInProgressInAChunk(int chunk) 
	{
		bool found = false;
		int i = 0;
		while (!found && i < _actionInProgress.Count) {
			found = _actionInProgress [i].chunk == chunk;
            ++i;
		}

		return found;  // -_-'
	}

    public int getActionInProgressInAChunk(int chunk)
    {
        bool found = false;
        int i = 0;
        while (!found && i < _actionInProgress.Count) {
            found = _actionInProgress[i].chunk == chunk;
            ++i;
        }
        if (found) {
            return _actionInProgress[i-1].id;
        }
        else {
            return -1; // -_-'
        }
    }

    public void newHourCallback()
	{
		for (int i = 0; i < _actionInProgress.Count; ++i) {
			_actionInProgress [i].addAnHourWorked ();
			if (_actionInProgress [i].isFinished) {
				WorldTerrain.GetInstance ().actionFinishedInChunk (_actionInProgress [i].chunk, _actionInProgress [i].id);
				_actionInProgress.RemoveAt (i);
			}
		}
	}

	public List<int> getActionPartnersOfAnAction(int id)
	{
		return _action [id].getActionPartners ();
	}

	public List<int> getInvestigationRequired(int id)
	{
		return _action [id].getInvestigationRequired ();
	}

    private bool isActionInProgress(int chunk)
    {
        bool isActionInProgressNow = false;
        foreach(ChunkAction action in _actionInProgress) {
            isActionInProgressNow = action.chunk == chunk;
            if(isActionInProgressNow) {
                break;
            }
        }
        return isActionInProgressNow; // -_-'
    }

    public bool hasAnActionPenalization(int action)
    {
        return _action[action].hasPenalization;
    }

    public bool isAnActionRequired(int action)
    {
        return _action[action].isRequired;
    }


    public bool hasAnActionToBeAddedAsDone(int action)
    {
        return _action[action].addAsDone;
    }

    public void stopActionChunk(int chunk)
    {
        
        for (int i = 0; i < _actionInProgress.Count; ++i) {
            ChunkAction action = _actionInProgress[i];
            bool isActionInProgressNow = action.chunk == chunk;
			if (isActionInProgressNow) {
				action.returnObjects ();
				WorkerManager.GetInstance ().BusyWorkers -= action.workersNeeded;
				_actionInProgress.RemoveAt(i);
                break;
			}
        }
    }

    public void save(ActionManagerData actionManagerData)
    {
		foreach (ChunkAction chunkAction in _actionInProgress) {
            float remainingDistance = 0.0f;
            if (chunkAction.hasAnimation()) {
                ChunkAnimation animation = AnimationManager.GetInstance().getCurrentAnimationInAChunk(chunkAction.chunk);
                remainingDistance = animation.remainingDistance;
            }            
            actionManagerData.ActionsInProgress.Add(new ActionInProgressData(chunkAction.id, chunkAction.hoursElapsed, chunkAction.chunk, remainingDistance));
        }
    }

    public void load(ActionManagerData actionManagerData)
    {
        //public ChunkAction(int aID, bool aIsRequired, bool aHasPenalization, string aTitle, string aInfo, int aDuration, bool hasToAddAsDone, bool isCanalNeeded, int neededWorkers)
        foreach(ActionInProgressData data in actionManagerData.ActionsInProgress) {
            ChunkAction newAction = _action[data.ID];
            PerformChunkAction callback = this.addActionInProgress;
            ChunkAction t = newAction.copyWithCallback(callback, data.ChunkNumber);
            t.hoursElapsed = data.HoursElapsed;
            if (t.hasAnimation()) {
                t.starAnimationWithRemainingDistance(data.AnimationRemainingDistance);
            }
			t.performChunkAction (t);
        }     
    }
}
