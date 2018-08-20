using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * Esta clase contiene la informacion de un tile donde se planta arroz.
 * Gestiona tanto la logica como la representacion
 */

public enum RiceTerrainState { SOIL_SOFT, SOIL_AERATED, SOIL_DRY, SOIL_MUDDY, WATER, BURIED_STUBBLE, SOIL_SOFT_FIRE, DISABLED};
public enum RiceState { NOT_PLANTED, SEMBRADO, GROWING, GROWN};
public enum DEPENDENCY_TYPE { ACTION, STATUS, START_YEAR, END_YEAR, ACTION_NOT_DONE};
public enum STATUS_TYPE { WEED, WILD_RICE, PLAGUE, PLANTED, WORKED_ONCE, PLANTER_PLANTED };
public enum PhaseRice { PHASE_1, PHASE_2, PHASE_3, PHASE_4, NONE };

public class RiceTerrainTile {
	public const uint MAX_RICE_UNITS = 10;
	private const uint DAYS_TO_GERMINATE = 45;
	private const uint DAYS_TO_GROWN = DAYS_TO_GERMINATE+30;
    private const uint DAYS_TO_BURN_ROSTOLL = 5;
    private const int ID_REMOVE_WEED = 8;
	private const int ID_REMOVE_WILD_RICE = 9;
	private const int ID_PLANT_WITHOUT_BARQUETA = 18;
	private const int ID_PLANT_WITH_BARQUETA = 25;
	private const int ID_MOW_WITHOUT_BARQUETA = 16;
	private const int ID_MOW_WITH_BARQUETA = 24;
	private const int ID_BURN_ROSTOLL = 21;
	private uint row, col;
    private GameObject _terrainRepresentation;
	private GameObject _riceRepresentation;
	private int _chunkID;

    private bool selected;
	private GameObject _outline;

	private static Dictionary<RiceTerrainState, GameObject> resourcesTerrain;
	private static Dictionary<RiceState, GameObject> resourcesRice;
	private static Dictionary<TypeWeed, GameObject> _resourcesWeed;
    private static Dictionary<TypePlague, GameObject> _resourcesPlague;

	//To maintain the record of actions
	private List<int> _actionsDoneInTheYear; //ids of actions done in the year
	private Weed _weed;
	private PlagueInstance _plague;
    private RiceTerrainState _terrainState;
    private RiceState _riceState;
    private uint _daysPlanted;
    private uint _daysBurning;
    private bool _hasAlreadyBeenWorked;
    private bool _bHasAppliedFangueig;
    private bool _bHasAppliedHerbicide;
    private bool _isBurningRostoll;
	private GameObject _harvestImg;
    private int m_localTileID;

	private TutorialManager _tutMan;

    public RiceTerrainTile()
    {
        GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>().addListerToDayChange(this.newDayCallback);
        _weed = new Weed(this);
		_plague = new PlagueInstance(this);
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
    }
    public RiceTerrainTile (uint row, uint col, bool flooded) {
		this.row = row;
		this.col = col;
		_riceState = RiceState.NOT_PLANTED;
		selected = false;
		_daysPlanted = 0;
		_daysBurning = 0;
		_hasAlreadyBeenWorked = false;
		_weed = new Weed(this);
		_bHasAppliedHerbicide = false;
		_bHasAppliedFangueig = false;
		_isBurningRostoll = false;
		
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().addListerToDayChange(this.newDayCallback);

		_actionsDoneInTheYear = new List<int> ();
		if (flooded) {
			this._terrainState = RiceTerrainState.WATER;
		} else {
			this._terrainState = RiceTerrainState.SOIL_MUDDY;
		}

        _plague = new PlagueInstance(this);

		updateTerrainRepresentation ();


		// Load Outline
		InstantiateImages();
	}

	public void InstantiateImages()
	{
		_outline = instantiateImg("Textures/terrainTiles/RiceTerrainOutline", false);
		_harvestImg = instantiateImg("Textures/terrainTiles/RiceTerrain_HARVEST", false);
	}

	private GameObject instantiateImg(string path, bool active) {
		GameObject gameObject = Resources.Load<GameObject>(path);
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		Vector3 worldPosition = worldTerrain.getTileWorldPosition(getRow(), getCol());
		worldPosition.z = WorldTerrain.BUILDING_Z_LAYER;
		GameObject instance = (GameObject) GameObject.Instantiate(gameObject, worldPosition, Quaternion.identity);
		instance.SetActive (active);
		return instance;
	}


	private void ResetActions() 
	{
		_actionsDoneInTheYear.Clear ();
		_bHasAppliedFangueig = false;
		_bHasAppliedHerbicide = false;
	}

	private void ResetRice() 
	{
		_daysPlanted = 0;
		_riceState = RiceState.NOT_PLANTED;
		updateRiceRepresentation ();
	}

	private void ResetTerrain()
	{
		if (isDisabled()) {
			_terrainState = RiceTerrainState.SOIL_MUDDY;
			updateTerrainRepresentation ();
		}
	}

	public void reset() 
	{
		ResetActions ();
		ResetRice ();
		ResetTerrain ();
	}

	public void disable()
	{
		//_actionsDoneInTheYear.Clear ();
		removeWeed ();
		treatPlague ();

		_riceState = RiceState.NOT_PLANTED;
		//_daysPlanted = 0;
		updateRiceRepresentation ();

		_terrainState = RiceTerrainState.DISABLED;
		updateTerrainRepresentation();

		_harvestImg.SetActive (false);
	}

	public bool isDisabled()
	{
		return (_terrainState == RiceTerrainState.DISABLED);
	}


    public void enable()
    {
        _terrainState = RiceTerrainState.WATER;
        updateTerrainRepresentation();
    }

	public void delete() {
		removeWeed ();
		treatPlague ();
		ResetActions ();
		ResetRice ();
		if (_terrainRepresentation != null) GameObject.Destroy (_terrainRepresentation);
		//if (_riceRepresentation != null) GameObject.Destroy (_riceRepresentation);
	}

	public static void initResources() {
		// Load Terrain Resources
		if (resourcesTerrain == null) {
			resourcesTerrain = new Dictionary<RiceTerrainState, GameObject> ();
			foreach (RiceTerrainState terrainID in Enum.GetValues(typeof(RiceTerrainState))) {
				GameObject terrainGameObject = null;
				String spritePath = "Textures/terrainTiles/RiceTerrain_" + terrainID.ToString ();
				try {
					terrainGameObject = Resources.Load<GameObject> (spritePath);
				} catch (Exception e) {
					Debug.LogError (e.ToString ());
				}
				Debug.Assert (terrainGameObject != null, "Terrain not loaded: " + spritePath);
				resourcesTerrain.Add (terrainID, terrainGameObject);
			}
		}

		// Load Rice Resources
		if (resourcesRice == null) {
			resourcesRice = new Dictionary<RiceState, GameObject> ();
			foreach (RiceState riceID in Enum.GetValues(typeof(RiceState))) {
				if (riceID == RiceState.NOT_PLANTED || riceID == RiceState.SEMBRADO)
					continue;
				GameObject riceGameObject = null;
				String spritePath = "Textures/terrainTiles/RiceTile_" + riceID.ToString ();
				try {
					riceGameObject = Resources.Load<GameObject> (spritePath);
				} catch (Exception e) {
					Debug.LogError (e.ToString ());
				}
				Debug.Assert (riceGameObject != null, "Rice not loaded: " + spritePath);
				resourcesRice.Add (riceID, riceGameObject);
			}
		}

		// Load Rice Resources

		if (_resourcesWeed == null) {
			_resourcesWeed = new Dictionary<TypeWeed, GameObject> ();
			foreach (TypeWeed weedID in Enum.GetValues(typeof(TypeWeed))) {
				if (weedID != TypeWeed.NONE) {
					GameObject weedGameObject = null;
					String spritePath = "Textures/terrainTiles/" + weedID.ToString ();
					try {
						weedGameObject = Resources.Load<GameObject> (spritePath);
					} catch (Exception e) {
						Debug.LogError (e.ToString ());
					}
					Debug.Assert (weedGameObject != null, "Weed not loaded: " + spritePath);
					_resourcesWeed.Add (weedID, weedGameObject);
				}
			}
		}

        if (_resourcesPlague == null)
        {
            _resourcesPlague = new Dictionary<TypePlague, GameObject>();
            foreach (TypePlague plagueID in Enum.GetValues(typeof(TypePlague)))
            {
                if (plagueID != TypePlague.NONE)
                {
                    GameObject plagueGameObject = null;
					String spritePath = "Textures/terrainTiles/" + plagueID.ToString();
                    try
                    {
                        plagueGameObject = Resources.Load<GameObject>(spritePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                    Debug.Assert(plagueGameObject != null, "Plague not loaded: " + spritePath);
                    _resourcesPlague.Add(plagueID, plagueGameObject);
                }
            }
        }
	}

    public uint getRow() {return row;}
	public uint getCol() {return col;}

    public void setRow(uint row) { this.row = row; }
    public void setCol(uint col) { this.col = col; }

    public RiceTerrainState getRiceTerrainState() {
		return _terrainState;
	}

	public RiceState getRiceState() {
		return _riceState;
	}

	public uint getRiceProduced() {
        float penalization = PenalizationManager.GetInstance().getPercentageForChunk(_chunkID);
        uint riceLost = (uint)(Math.Ceiling((float)MAX_RICE_UNITS * penalization * 0.01f));
		float bonusRice = MAX_RICE_UNITS * (float)InvestigationManager.GetInstance().getRicePerChunkBonus() * 0.01f;
		riceLost = Math.Min((uint)bonusRice + MAX_RICE_UNITS, riceLost);
		//UserDataManager.GetInstance().addRiceLost(Math.Min(MAX_RICE_UNITS, riceLost));
        uint riceProduced = Math.Max(MAX_RICE_UNITS + (uint)bonusRice - riceLost, (uint)0);
        return riceProduced;
	}

	public uint getRiceLost() {
		return MAX_RICE_UNITS - getRiceProduced ();
	}

    public void newDayCallback() 
	{
		// ***** generate random bad weeds

        if(_isBurningRostoll) {
            if(++_daysBurning >= DAYS_TO_BURN_ROSTOLL) {
                _terrainState = RiceTerrainState.SOIL_SOFT;
                updateTerrainRepresentation();
                _daysBurning = 0;
                _isBurningRostoll = false;
            }
        }
		if (_riceState != RiceState.NOT_PLANTED) {
			++_daysPlanted;
		}
		if (_riceState == RiceState.SEMBRADO && _daysPlanted >= DAYS_TO_GERMINATE) {
			_riceState = RiceState.GROWING;
			updateRiceRepresentation ();
		}
		if (_riceState == RiceState.GROWING && _daysPlanted >= DAYS_TO_GROWN) {
			_riceState = RiceState.GROWN;
			updateRiceRepresentation ();
		}
	}

	public void select() {
		if (!selected) {
			selected = true;
			_outline.SetActive (true);
		}
	}

	public void unselect() {
		if (selected) {
			selected = false;
			_outline.SetActive (false);
		}
	}

	public void updateTerrainRepresentation() {
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		Vector3 worldPosition = worldTerrain.getTileWorldPosition((uint)this.getRow(), (uint)this.getCol());
		if (_terrainState == RiceTerrainState.WATER) worldPosition.z = WorldTerrain.WATER_Z_LAYER;
		else worldPosition.z = WorldTerrain.TERRAIN_Z_LAYER;
		if (_terrainRepresentation != null)
			GameObject.Destroy (_terrainRepresentation);
		Debug.Assert (resourcesTerrain != null);
		Debug.Assert(resourcesTerrain.ContainsKey(_terrainState));
		_terrainRepresentation = (GameObject) GameObject.Instantiate(resourcesTerrain[_terrainState], worldPosition, Quaternion.identity);

		//select ();//___________
	}
    
    public void markToDelete() {
		_terrainRepresentation.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public void unmarkToDelete() {
		_terrainRepresentation.GetComponent<SpriteRenderer>().color = Color.white;
	}




	// ACTIONS


	private void updateRiceRepresentation () {
		if (_riceState == RiceState.NOT_PLANTED || _riceState == RiceState.SEMBRADO) {
			if (_riceRepresentation != null)
				GameObject.Destroy (_riceRepresentation);
		} else {
			WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
			Vector3 worldPosition = worldTerrain.getTileWorldPosition((uint)this.getRow(), (uint)this.getCol());
			worldPosition.z = WorldTerrain.PLANTS_Z_LAYER;
			if (_riceRepresentation != null)
				GameObject.Destroy (_riceRepresentation);
			Debug.Assert (resourcesRice != null);
			Debug.Assert(resourcesRice.ContainsKey(_riceState));
			_riceRepresentation = (GameObject) GameObject.Instantiate(resourcesRice[_riceState], worldPosition
                 + resourcesRice[_riceState].transform.position, Quaternion.identity);
		}
	}

	public void addActionDone(int actionID)
	{
		_actionsDoneInTheYear.Add(actionID);
		_hasAlreadyBeenWorked = true;
	}
	
	public bool isActionDone(string actionKey)
	{
		int actionID = Convert.ToInt32(actionKey.Substring(actionKey.LastIndexOf('_')+1));
		bool isDone = false;
		int i = 0;		
		while(!isDone && i < _actionsDoneInTheYear.Count) {
			isDone = (actionID == _actionsDoneInTheYear[i++]);
		}
		return isDone;
	}

	public bool isActionDone(int actionID)
	{
		bool isDone = false;
		int i = 0;
		while(!isDone && i < _actionsDoneInTheYear.Count) {
			isDone = (actionID == _actionsDoneInTheYear[i++]);
		}
		return isDone;
	}
	
	public bool isPlanted()
	{
		return (_riceState != RiceState.NOT_PLANTED);
	}

	public bool hasTileAlreadyBeenWorked() 
	{
		return _hasAlreadyBeenWorked;
	}

	public bool checkDependencies(ChunkAction action)
	{
		Dictionary<string, ArrayList> dependencies = action.getDependencies ();
		bool dependenciesOK = true;
		foreach (KeyValuePair<string, ArrayList> kvp in dependencies) {
			if (kvp.Key.Equals (DEPENDENCY_TYPE.ACTION.ToString (), System.StringComparison.Ordinal)) {
				for (uint i = 0; i < kvp.Value.Count; ++i) {	
					//if (checkYearDependency (_action [(string)kvp.Value [(int)i]])) {
						dependenciesOK = dependenciesOK && this.isActionDone ((string)kvp.Value [(int)i]);
					//}
				}
			} else if (kvp.Key.Equals (DEPENDENCY_TYPE.START_YEAR.ToString (), System.StringComparison.Ordinal)) {
				//kvp.Value[0]
				uint currentYear = GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().getCurrentYear ();
				uint dependencyYear = Convert.ToUInt32 (kvp.Value [0]);
				dependenciesOK = dependenciesOK && (dependencyYear <= currentYear);				
			} else if (kvp.Key.Equals (DEPENDENCY_TYPE.END_YEAR.ToString (), System.StringComparison.Ordinal)) {
				//kvp.Value[0]
				dependenciesOK = dependenciesOK && (Convert.ToUInt32 (kvp.Value [0]) > 
					GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().getCurrentYear ());				
			} else if (kvp.Key.Equals (DEPENDENCY_TYPE.STATUS.ToString (), System.StringComparison.Ordinal)) {
				for (int i = 0; i < kvp.Value.Count; ++i) {				
					if (kvp.Value [i].Equals (STATUS_TYPE.PLANTED.ToString ())) {
						dependenciesOK = dependenciesOK && this.isPlanted ();
					} else if (kvp.Value [i].Equals (STATUS_TYPE.WORKED_ONCE.ToString ())) {						
						dependenciesOK = dependenciesOK && this.hasTileAlreadyBeenWorked ();
					} else if (kvp.Value [i].Equals (STATUS_TYPE.WEED.ToString ())) {
						dependenciesOK = dependenciesOK && WorldTerrain.GetInstance().hasChunkWeed(_chunkID);
					} else if (kvp.Value [i].Equals (STATUS_TYPE.WILD_RICE.ToString ())) {			
						dependenciesOK = dependenciesOK && WorldTerrain.GetInstance().hasChunkWildRice(_chunkID);
					} else if (kvp.Value [i].Equals (STATUS_TYPE.PLAGUE.ToString ())) {
                        dependenciesOK = dependenciesOK && WorldTerrain.GetInstance().hasChunkPlague(_chunkID);
                    } else if (kvp.Value[i].Equals(STATUS_TYPE.PLANTER_PLANTED.ToString())) {
                        Building_Planter planter = (Building_Planter)BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTER);
                        dependenciesOK = dependenciesOK && planter.hasPlantsForAChunk();
                    }
                }
			}
			else if (kvp.Key.Equals (DEPENDENCY_TYPE.ACTION_NOT_DONE.ToString (), System.StringComparison.Ordinal)) {
				for (uint i = 0; i < kvp.Value.Count; ++i) {	
					dependenciesOK = dependenciesOK && !this.isActionDone ((string)kvp.Value [(int)i]);
				}
			}
			
		}
		
		return dependenciesOK;
	}


	public TypeWeed getWeedType()
	{
		return _weed.type;
	}
	
	public bool hasTileWildRice()
	{
		return (_weed.type == TypeWeed.WILD_RICE_PHASE_1 || 
				_weed.type == TypeWeed.WILD_RICE_PHASE_2 || 
				_weed.type == TypeWeed.WILD_RICE_PHASE_3); 
	}	
	
	public bool hasTileWeed()
	{
		return (_weed.type == TypeWeed.CIPERACEA); 
	}

    public bool hasTilePlague()
    {
        return _plague.active;
    }

	public bool checkYearDependency(ChunkAction action)
	{
		Dictionary<string, ArrayList> dependencies = action.getDependencies();
		bool dependenciesOK = true;
		foreach (KeyValuePair<string, ArrayList> kvp in dependencies ) {
			if( kvp.Key.Equals(DEPENDENCY_TYPE.START_YEAR.ToString(), System.StringComparison.Ordinal)){
				//kvp.Value[0]
				dependenciesOK = dependenciesOK && (Convert.ToUInt32(kvp.Value[0]) <= 
				                                    GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().getCurrentYear());				
			}
			else if( kvp.Key.Equals(DEPENDENCY_TYPE.END_YEAR.ToString(), System.StringComparison.Ordinal)){
				//kvp.Value[0]
				dependenciesOK = dependenciesOK && (Convert.ToUInt32(kvp.Value[0]) > 
				                                    GameObject.FindGameObjectWithTag ("Logic").GetComponent<TimeManager> ().getCurrentYear());				
			}
		}
		
		return dependenciesOK;
	}

	//actions_id: 1
	public void buryStubble()
	{
		_terrainState = RiceTerrainState.BURIED_STUBBLE;
		updateTerrainRepresentation ();
	}

	//actions_id: 2
	public void flood()
	{
		_terrainState = RiceTerrainState.WATER;
		updateTerrainRepresentation ();
	}

	//actions_id: 3
	public void dry()
	{
		_terrainState = RiceTerrainState.SOIL_DRY;
		updateTerrainRepresentation ();
	}

	//actions_id: 4
	public void aerate()
	{
		_terrainState = RiceTerrainState.SOIL_AERATED;
		updateTerrainRepresentation ();
	}

	//actions_id: 19
	public void mud()
	{
		_terrainState = RiceTerrainState.SOIL_MUDDY;
		updateTerrainRepresentation ();
	}

	
	public void mow() {
		removeWeed ();
		treatPlague ();
		_riceState = RiceState.NOT_PLANTED;
		updateRiceRepresentation ();
		_harvestImg.SetActive (true);
	}

	public void sendRiceToDry()
	{
		uint riceProduced = getRiceProduced ();
		UserDataManager.GetInstance().addRiceProduced(riceProduced);
		UserDataManager.GetInstance().addRiceLost(getRiceLost());
        if (!BuildingsManager.GetInstance().isBuilded(BUILDINGS.PLANTA)) {
			Building_Trill trill = (Building_Trill)BuildingsManager.GetInstance().getBuilding(BUILDINGS.TRILL);
			uint riceOverFlow = (uint) Math.Max ((int) (riceProduced - trill.getCurrentFreeCapacity ()), 0);
			UserDataManager.GetInstance().addRiceLost(riceOverFlow);
			trill.sendRice(riceProduced);
        }
        else {
			Building_Planta planta = (Building_Planta)BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTA);
			uint riceOverFlow = Math.Max (riceProduced - planta.getCurrentFreeCapacity (), 0);
			UserDataManager.GetInstance().addRiceLost(riceOverFlow);
			planta.sendRice(riceProduced);
        }
		_harvestImg.SetActive (false);
	}

	public void sembrar()
	{
		_terrainState = RiceTerrainState.WATER;
		updateTerrainRepresentation();
		_riceState = RiceState.SEMBRADO;
		_daysPlanted = 0;
		updateRiceRepresentation ();
	}

	public void plant()
	{
        _terrainState = RiceTerrainState.WATER;
        updateTerrainRepresentation();
        _riceState = RiceState.GROWING;
		_daysPlanted = DAYS_TO_GERMINATE;
		updateRiceRepresentation ();
	}

	public void removeWeed()
	{
		_weed.delete();
        WorldTerrain.GetInstance().removeWeed(_chunkID, m_localTileID);
    }

    public void treatPlague()
    {
        _plague.active = false;
        _plague.delete();
        GameObject.Find("Logic").GetComponent<PlagueLogic>().ClearOneChunk(_chunkID);
    }

	public void applyHerbicide(bool isEco)
	{
		_weed.delete();
		_bHasAppliedHerbicide = true;
        if (!isEco) {
            CoopManager.GetInstance().loseEcologyWeedControlPoint();
        }

    }

	//actions_id: 20 Anivellar (tradicional)
	public void even()
	{
		removeWeed ();
		_terrainState = RiceTerrainState.SOIL_SOFT;
		updateTerrainRepresentation ();
	}

	public void applyFire()
	{
		removeWeed ();
		treatPlague ();
        _isBurningRostoll = true;
        _terrainState = RiceTerrainState.SOIL_SOFT_FIRE;
		updateTerrainRepresentation ();
        //CoopManager.GetInstance().loseEcologyStubbleManagemenPoint();
    }

	public void removeWeedAndApplyFanguejar()
	{
		removeWeed ();
		_bHasAppliedFangueig = true;
	}

    public void applyFertilizer(bool isEco)
    {
        if (!isEco) {
            CoopManager.GetInstance().loseEcologyFertilizerPoint();
        }
    }

	public void performAction(int id, List<int> actionPartners, bool addAsDone)
	{
		if(addAsDone) {
			this.addActionDone(id);
			for (int i = 0; i < actionPartners.Count; ++i) {
				this.addActionDone(actionPartners[i]);
			}
		}

		switch(id) {
		    case 1:
			    //Enterrar restes
			    buryStubble();
			    break;
		    case 2:
			    //Inundar el camp
			    flood ();			
			    break;
		    case 3:
                //Desecar
			    dry();	
			    break;
		    case 4:
				//Airejar camp
				removeWeed ();
			    aerate();
			    break;
		    case 5:
			    //Inundancio minima
			    flood ();
			    break;
		    case 6:
			    //Siembra directa
			    sembrar();
			    break;
		    case 7:
			    //Aplicar herbicidas
			    applyHerbicide(false);
			    break;
		    case 8: 
			    //Quitar malas hierbas
			    removeWeed();
			    break;
		    case 9: 
			    //Quitar arroz salvaje
			    removeWeed();
			    break;
		    case 10:
                //Adobament de fons
                applyFertilizer(false);
                break;
		    case 11:
                //Adobament de cobertera
                applyFertilizer(false);
                break;
		    case 12:
			    //Incrementar nivell d'aigua amb continua renovacio
			    flood ();
			    break;
		    case 13:
			    //Fanguejar per treure arròs salvatge (tradicional)	
			    removeWeed();	
			    break;
		    case 14:
                //Adobar el camp
                applyFertilizer(true);
                break;			
		    case 15:
			    //Disminuir nivell d'aigua		
			    mud ();			
			    break;
		    case 16:
			    //Segar sense carro
			    mow();
                //this.addActionDone(ID_MOW_WITH_BARQUETA);
                break;
		    case 17:
			    //Transportar la cullita a secar (tradicional)
			    sendRiceToDry();
			    break;			
		    case 18:
			    //Agafar plantes del planter i plantar sense barqueta
			    plant();
			    //this.addActionDone(ID_PLANT_WITH_BARQUETA);
			    break;
		    case 19:
			    //Fanguejar per prevenir males herbes (tradicional)			
			    removeWeedAndApplyFanguejar();
			    break;
		    case 20:
			    //Anivellar (tradicional)
			    even();
			    break;
		    case 21:
			    //Cremar rostoll
			    applyFire();
			    break;
		    case 22:
			    //Fanguejar per treure males herbes (tradicional)
			    removeWeed();
			    break;
		    case 23:
			    //Anivellar per láser
			    even();
			    break;
		    case 24:
			    //Segar amb barqueta
			    mow();
			    //this.addActionDone(ID_MOW_WITHOUT_BARQUETA);
                treatPlague();
                break;
		    case 25:
                //Plantar amb barqueta
                plant();
                //this.addActionDone(ID_PLANT_WITHOUT_BARQUETA);
                break;
		    case 26:
			    //Fanguejar per treure males herbes (tradicional)
			    removeWeed();
			    break;
		    case 27:
			    //Anivellar (modern)
			    even();
			    break;
		    case 28:
			    //Fanguejar per prevenir males herbes (modern)
			    mud();
			    removeWeedAndApplyFanguejar();
			    break;
		    case 29:
			    //Transportar la cullita per secar (modern)
			    sendRiceToDry();
			    break;
		    case 30:
			    //Segar (modern)
			    mow();
                treatPlague();
                break;
		    case 31:
                //Adobament ecològic de fons
                applyFertilizer(true);
                break;
		    case 32:
                //Adobament ecològic de cobertera
                applyFertilizer(true);
                break;
		    case 33:
			    //Fanguejar per treure arròs salvatge (modern)
			    removeWeed();
			    break;
            case 34:
                //Aplicar herbicides ecològics
                applyHerbicide(true);
                break;
            case 35:
                //Eliminar plagues
                treatPlague();
                break;
			case 36:
				//Airejar camp
				removeWeed ();
				aerate();
				break;
        }
	}

	public void setChunkNumber(int chunk)
	{
		_chunkID = chunk;
	}

	public int getChunkNumber()
	{
		return _chunkID;
	}

    public void changePlagueState(TypePlague plague)
    {
        _plague.type = plague;
        _plague.active = true;
        WorldTerrain worldTerrain = WorldTerrain.GetInstance();
        Vector3 worldPosition = worldTerrain.getTileWorldPosition((uint)this.getRow(), (uint)this.getCol());
        worldPosition.z = WorldTerrain.PLANTS_Z_LAYER;
        if (_plague.instance != null)
            GameObject.Destroy(_plague.instance);
        Debug.Assert(_plague != null);
        Debug.Assert(_resourcesPlague.ContainsKey(plague));
        worldPosition += _resourcesPlague[plague].transform.position;
        _plague.instance = (GameObject)GameObject.Instantiate(_resourcesPlague[plague], worldPosition, Quaternion.identity);

		if (!Tutorial_Plagas.init && !isDisabled()) {
			_tutMan.startTuto(new Tutorial_Plagas());
		}
    }

    public void changeWeedState(TypeWeed weed)
	{
		_weed.type = weed;
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		Vector3 worldPosition = worldTerrain.getTileWorldPosition((uint)this.getRow(), (uint)this.getCol());
		worldPosition.z = WorldTerrain.PLANTS_Z_LAYER;
		if (_weed.instance != null)
			GameObject.Destroy (_weed.instance);
		Debug.Assert (_weed != null);
		Debug.Assert(_resourcesWeed.ContainsKey(weed));
		worldPosition += _resourcesWeed [weed].transform.position;
		_weed.instance = (GameObject) GameObject.Instantiate(_resourcesWeed[weed], worldPosition, Quaternion.identity);

		if (!Tutorial_Plagas.init && !isDisabled()) {
			_tutMan.startTuto(new Tutorial_Plagas());
		}
	}

	//this one is for loading
    public void changeWeedState(TypeWeed weed, int day, int penalizationDay)    
	{
		_weed.day = day;
		_weed.PenalizationDay = penalizationDay;
		changeWeedState (weed);
    }

	/* NO REFERENCES
    public void changeWeedState(TypeWeed weed, Vector3 worldPosition)
    {
        _weed.type = weed;
        worldPosition.z = WorldTerrain.PLANTS_Z_LAYER;
        if (_weed.instance != null)
            GameObject.Destroy(_weed.instance);
        Debug.Assert(_weed != null);
        Debug.Assert(_resourcesWeed.ContainsKey(weed));
        worldPosition += _resourcesWeed[weed].transform.position;
        _weed.instance = (GameObject)GameObject.Instantiate(_resourcesWeed[weed], worldPosition, Quaternion.identity);
    }
    */
    public void growWeed()
	{
		_weed.grow ();
	}



	public bool hasAppliedHerbicide()
	{
		return _bHasAppliedHerbicide;
	}

	public bool hasAppliedFangueig()
	{
		return _bHasAppliedFangueig;
	}

	public void actionFinished(int id) 
	{
		if (id == ID_BURN_ROSTOLL) {
			_terrainState = RiceTerrainState.SOIL_SOFT;
			updateRiceRepresentation ();
		}
	}

    public List<int> getActionsDone()
    {
        return _actionsDoneInTheYear;
    }

    public void setActionsDone(List<int> actionsDoneInTheYear)
    {
        _actionsDoneInTheYear = actionsDoneInTheYear;
    }

    public int getPenalizationDay()
    {
        return _weed.PenalizationDay;
    }

    public void resetPenalizationDay()
    {
        _weed.resetPenalizations();
    }

    public void setLocalID(int id)
    {
        m_localTileID = id;
    }

    public int getLocalID()
    {
        return m_localTileID;
    }

	// SETTERS / GETTERS
	public Weed Weed
	{
		get
		{
			return _weed;
		}
		set
		{
			_weed = value;
		}
	}

	public RiceTerrainState TerrainState
	{
		get
		{
			return _terrainState;
		}
		set
		{
			_terrainState = value;
		}
	}

	public RiceState RiceState
	{
		get
		{
			return _riceState;
		}
		set
		{
			_riceState = value;
		}
	}


	public uint DaysPlanted
	{
		get
		{
			return _daysPlanted;
		}
		set
		{
			_daysPlanted = value;
		}
	}

	public uint DaysBurning
	{
		get
		{
			return _daysBurning;
		}
		set
		{
			_daysBurning = value;
		}
	}

	public bool HasAlreadyBeenWorked
	{
		get
		{
			return _hasAlreadyBeenWorked;
		}
		set
		{
			_hasAlreadyBeenWorked = value;
		}
	}


	public bool HasAppliedFangueig
	{
		get
		{
			return _bHasAppliedFangueig;
		}
		set
		{
			_bHasAppliedFangueig = value;
		}
	}

	public bool HasAppliedHerbicide
	{
		get
		{
			return _bHasAppliedHerbicide;
		}
		set
		{
			_bHasAppliedHerbicide = value;
		}
	}

	public bool IsBurningRostoll
	{
		get
		{
			return _isBurningRostoll;
		}
		set
		{
			_isBurningRostoll = value;
		}
	}

	public int LocalTileID
	{
		get
		{
			return m_localTileID;
		}
		set
		{
			m_localTileID = value;
		}
	}

	public PlagueInstance Plague
	{
		get
		{
			return _plague;
		}
		set
		{
			_plague = value;
		}
	}
}
