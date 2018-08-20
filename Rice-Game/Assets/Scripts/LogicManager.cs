using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using MyUtils.Pair;
using System.Threading;

public class LogicManager: MonoBehaviour
{
	// GLOBALS
	public const float PRICE_PER_RICE = 4.1f;

    //public enum DEPENDENCY_TYPE { ACTION, STATUS, START_YEAR, END_YEAR };
    //public enum STATUS_TYPE { WEED, WILD_RICE, PLAGUE, PLANTED, WORKED_ONCE };
    private const int MONTHS_TO_LOSE = 12;
    private const int MONTHS_UNTIL_SHOWING_PANEL = 12;
    
	public enum LOGIC_STATE { DEFAULT, BUILDING, DELETING};
	public LOGIC_STATE logicState;

	public bool testMode = false;

	private uint[] _tileSelectedPosition;
	private bool _hasTileSelected;
	//private RiceTerrainLogic _terrainLogic;
	private GameObject _actionPanel;
	private GameObject _buildingPanel;
    private int m_monthsInRed;
    private int m_monthsSinceIShowedInRedPopup;
    private bool b_warningInRedPopupShown;
	private bool b_firstAutoSave = false;

	private TutorialManager _tutMan;

    private CloudSpawner _cloudSpawner;

	private static class BUILDING_INFO {
		static public TileTerrainType terrainID;
		static public BUILDINGS buildingID; 
		static public List<GameObject> representation;
		static public bool esPotConstruir;

		static public void clearRepresentation() {
			if (representation != null) {
				for (int i = 0; i < representation.Count; ++i)
					GameObject.Destroy (representation [i]);
				representation.Clear ();
			} else {
				representation = new List<GameObject> ();
			}
		}
	}

    private List<uintPair> tilesToDelete;

	void Awake() 
	{
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		Dictionary.init();

        //PlayerPrefs.SetInt("LoadData", 1);
        //PlayerPrefs.SetInt("LoadData", 0);
		if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
			init();
        }
		else {
			//_tutMan.startTuto (new Tutorial_Inicial());
            GameSaveDataManager.Load();
        }
        
    }
	
	void Start () 
	{
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            gameObject.GetComponent<TimeManager>().addListenerToYearChange(this.happyNewYear);
            gameObject.GetComponent<TimeManager>().addListenerToMonthChange(happyNewMonth);
			gameObject.GetComponent<TimeManager>().addListerToDayChange(this.newDayCallback);
			gameObject.GetComponent<PhaseManager> ().addListenerToPhaseChange (this.newPhaseCallback);
		}
		b_firstAutoSave = !PlayerPrefs.HasKey ("FirstAutoSaveDone");
		if (!b_firstAutoSave) {
			GameObject.Find ("Logic").GetComponent<LogicManager> ().initAutoSave ();
		}
		if (testMode) {
			prepareTest ();
			//FaunaFelizManager.GetInstance ();
		}
	}
	
	void Update()
	{
		_cloudSpawner.update (Time.deltaTime);
		AnimationManager.GetInstance ().update (Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.Escape))
			ExitGame ();

		if (testMode) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				TestManager.GetInstance ().AdvanceTillPhase (TypeFase.HARVEST);
			}
			if (Input.GetKeyDown (KeyCode.Z)) {
				gameObject.GetComponent<TimeManager> ().calendar._currentYear = 2996;
				TestManager.GetInstance ().InvestigateEverything ();
				//UserDataManager.GetInstance ().addGold(-18000.0f);
				UserDataManager.GetInstance ().addGold(30000.0f);
				//UserDataManager.GetInstance ().addRiceProduced (123);
				//UserDataManager.GetInstance ().sellRice(5000);
				//UserDataManager.GetInstance ().rice.setTotalRiceProduced (5000);
				//UserDataManager.GetInstance ().addGold(3000.0f);
			}
		}
    }

	public void ExitGame() {
		Application.Quit ();
	}

	public void calculate(float amount)
	{
		object amountRef = amount;
		int balance = (int)amountRef;
		Debug.Log(balance);
	}
	
	public bool init()
	{        
        //GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().readJSON();
        _tileSelectedPosition = new uint[2] { 28, 25 };
		_hasTileSelected = false;
		m_monthsInRed = 0;
		m_monthsSinceIShowedInRedPopup = 0;
		b_warningInRedPopupShown = false;
		//_terrainLogic = new RiceTerrainLogic();
		ActionManager.GetInstance ();
        CoopManager.GetInstance();
        PenalizationManager.GetInstance();
		WorldTerrain.GetInstance ().initWorld ();
		BuildingsManager.GetInstance ().init ();
		UserDataManager.GetInstance ().init ();
		UserDataManager.GetInstance ().gold.setGold (6800);
		RiceObjectsManager.GetInstance ();
		_cloudSpawner = new CloudSpawner (WorldTerrain.CLOUDS_Z_LAYER);
        
		_tutMan.startTuto (new Tutorial_Inicial());
        GameSaveDataManager.init();

        return true;
	}

    
    public void clickOn(float x, float y) {
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		uint i = (uint)worldTerrain.getMatrixRow (x, y);
		uint j = (uint)worldTerrain.getMatrixCol (x, y);

		if (logicState == LOGIC_STATE.DEFAULT) {
			//TileTerrainType terrainType = worldTerrain.getTypeTerrainOfTile (i, j);
			selectTile (new uint[2]{ i, j });
		} else if (logicState == LOGIC_STATE.BUILDING) {
			selectTile (new uint[2]{ i, j });

			if (BUILDING_INFO.terrainID == TileTerrainType.BUILDING) {
				setBuildingBuildingRepresentation ();
			} else if (BUILDING_INFO.terrainID == TileTerrainType.RICE_TERRAIN) {
				setBuildingRiceChunkRepresentation ();
			} else if (BUILDING_INFO.terrainID == TileTerrainType.CANAL) {
				WorldTerrain wt = WorldTerrain.GetInstance ();
				TileTerrainType tileType = wt.getTypeTerrainOfTile (i, j);
				if ((tileType == TileTerrainType.FREE || tileType == TileTerrainType.SEPARATOR) && !wt.tileContainsVegetation(i, j)) {
					if (!wt.canalManager.tileHasCanalTmp (i, j)) {
						_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (+1, +WorldTerrain.PRICE_Canal);
					} else {
						_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (-1, -WorldTerrain.PRICE_Canal);
					}
					wt.addRmvCanalTmp (i, j);
				}
			}
		} else if (logicState == LOGIC_STATE.DELETING) {
			selectTile (new uint[2]{ i, j },false);
			if (tilesToDelete.Contains (new uintPair (i, j))) {
				unmarkToDelete (i, j);
				tilesToDelete.Remove (new uintPair (i, j));
				foreach (uintPair pos in tilesToDelete)
					Debug.Log (pos);
			} else {
				if (markToDelete (i, j)) {
					tilesToDelete.Add (new uintPair (i, j));
					foreach (uintPair pos in tilesToDelete)
						Debug.Log (pos);
				}
			}
		}
	}
	
	private void selectTile(uint[] tileSelectedPosition, bool changeMaterial=true) {
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		unselectTile ();

		_hasTileSelected = true;
		_tileSelectedPosition[0] = tileSelectedPosition[0];
		_tileSelectedPosition[1] = tileSelectedPosition[1];
		if (changeMaterial) worldTerrain.selectTile(_tileSelectedPosition[0], _tileSelectedPosition[1]);
	}

	private void unselectTile() 
	{
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		if (_hasTileSelected) {
			// UNSELECT			
			worldTerrain.unselectTile(_tileSelectedPosition[0], _tileSelectedPosition[1]);
		}
		_hasTileSelected = false;
	}
	
	public void cameraArriveSelection() {
		if (logicState == LOGIC_STATE.DEFAULT) {
			if (_hasTileSelected) {
				// show Post For Selected Tile
				createActionPanel();
			}
		} else if (logicState == LOGIC_STATE.BUILDING) {
			if (BUILDING_INFO.terrainID != TileTerrainType.CANAL)
				createSingleBuildingPanel ();
		}
	}
	
	//It's a call back called when a new year happens to be
	public void happyNewYear()
	{
		//Debug.Log("Happy new year!");
		/*
		if(InvestigationManager.GetInstance().isInvestigated(INVESTIGATIONS_ID.COOP)) {
        	CoopManager.GetInstance().happyNewYear();        
		}
		*/
        WorldTerrain.GetInstance ().resetEndOfTheYear ();
        BuildingsManager.GetInstance().reset();
        PenalizationManager.GetInstance().reset();
		if (b_firstAutoSave) {
			GameObject.Find ("Logic").GetComponent<LogicManager> ().initAutoSave ();
			b_firstAutoSave = false;
			PlayerPrefs.SetInt ("FirstAutoSaveDone", 1);
		}

		GameObject yearlySumPanel = Resources.Load("Prefabs/YearlySumPanelLostFocusLayer") as GameObject;
		UnityEngine.Object.Instantiate(yearlySumPanel);

		FaunaFelizManager.GetInstance().newYearUpdate();
    }

	public void happyNewMonth()
	{
		if (UserDataManager.GetInstance().gold.inRed()) {
			++m_monthsInRed;
			if (b_warningInRedPopupShown) {
				++m_monthsSinceIShowedInRedPopup;
			}
			if (m_monthsInRed >= MONTHS_TO_LOSE) {
				instantiateLostPanel();
			}
			else if (!b_warningInRedPopupShown ||m_monthsSinceIShowedInRedPopup >= MONTHS_UNTIL_SHOWING_PANEL) {
				instantiateWarningInRedPanel();
				m_monthsSinceIShowedInRedPopup = 0;
				b_warningInRedPopupShown = true;
			}
		}
		else {
			m_monthsInRed = 0;
		}
	}

	public void newDayCallback() 
	{
		BuildingsManager.GetInstance().newDayCallback();
		WorldTerrain.GetInstance ().newDayCallback();
		GameObject.Find("Logic").GetComponent<PlagueLogic>().NewDayCallback();
		//updateActionPanel ();
    }

	public void newPhaseCallback()
	{
		PhaseManager phaseMan = gameObject.GetComponent<PhaseManager> ();
		TypeFase typePhase = phaseMan.getCurrentPhase ();
		Debug.Log ("New Phase: " + typePhase.ToString ());
		//TimeManager timeMan = gameObject.GetComponent<TimeManager> ();
		WorldTerrain wt = WorldTerrain.GetInstance ();
		switch (typePhase) {
		case TypeFase.SOWING:
			if (Tutorial_Plantell.init == false && !wt.areAllChunksDisabled()) {
				_tutMan.startTuto(new Tutorial_Plantell());
			}
			break;
		case TypeFase.HARVEST:
			if (Tutorial_Buildings.init == false && !wt.areAllChunksDisabled()) {
				_tutMan.startTuto(new Tutorial_Buildings());
			}
			break;
		}
	}

    // +++ ACTIONS +++
    private void createActionPanel() {
		if (_actionPanel != null)
			Destroy (_actionPanel);

        WorldTerrain terrain = WorldTerrain.GetInstance();
        TileTerrainType typeOfTileSelected = terrain.getTypeTerrainOfTile(_tileSelectedPosition[0], _tileSelectedPosition[1]);
		if (typeOfTileSelected == TileTerrainType.BUILDING && BuildingsManager.GetInstance().getTypeOfBuildingInPosition(_tileSelectedPosition[0], _tileSelectedPosition[1]) == BUILDINGS.PLANTER) {
			_tutMan.eventDone (TutorialManager.EVENTS.PLANTELL_ACTIONS_OPENED);
		}
		if (typeOfTileSelected == TileTerrainType.RICE_TERRAIN) {
			RiceTerrainTile riceTerrain = terrain.getRiceTerrain (_tileSelectedPosition [0], _tileSelectedPosition [1]);
            //int chunk = terrain.getRiceTerrain(_tileSelectedPosition[0], _tileSelectedPosition[1]).getChunkNumber();
            //if (!PenalizationManager.GetInstance().isChunkDisabled(chunk)) {
			if (!riceTerrain.isDisabled()) {
				GameObject actionPanelTemplate = Resources.Load("Prefabs/ActionPanelLostFocusLayer") as GameObject;
				_actionPanel = Instantiate(actionPanelTemplate);
				_actionPanel.SendMessage ("setRiceTerrain", riceTerrain);
				_tutMan.eventDone (TutorialManager.EVENTS.RICE_CHUNK_ACTIONS_OPENED);
            }
            else {
                GameObject disabledPanel = Resources.Load("Prefabs/DisabledChunkPanelLostFocusLayer") as GameObject;
                Instantiate(disabledPanel);
                return;
            }
        }
		else if (typeOfTileSelected == TileTerrainType.BUILDING) {
			GameObject actionPanelTemplate = Resources.Load("Prefabs/ActionBuildingPanelLostFocusLayer") as GameObject; // OR Planta
			_actionPanel = Instantiate(actionPanelTemplate);
			BUILDINGS b = BuildingsManager.GetInstance().getTypeOfBuildingInPosition(_tileSelectedPosition[0], _tileSelectedPosition[1]);
			_actionPanel.BroadcastMessage("setActionsForBuilding", b);
        }
    }

	public void actionPanelClean() {
		if (_actionPanel != null)
			_actionPanel.BroadcastMessage ("kill");
	}

	public void actionPanelKilled() 
	{
        unselectTile();
	}

	public void cleanUI()
	{
		actionPanelClean ();

		string[] panelsTags = new string[] {
			"UpgradeMenuParent", "CreditsMenuParent", "BuyMenuParent", "CalendarMenuParent", "SettingsMenuParent",
			"RankingMenuParent", "CoopMenuParent", "RiceOverflowPanel", "DisabledChunkPanel", "WorkerRecruitmentPanel"
		};
		for (int i = 0; i < panelsTags.Length; ++i) {
			GameObject gameObjectPanel = GameObject.FindGameObjectWithTag(panelsTags[i]);
			if (gameObjectPanel != null) {
				gameObjectPanel.SetActive (false);
			}
		}

		GameObject.FindGameObjectWithTag ("SideMenu").GetComponent<SideMenu> ().closeMenu ();

		if (logicState != LOGIC_STATE.DEFAULT) {
			constructionBuildingCancel ();
		}
	}

	// +++ BUILDING +++
	public void startBuilding(TileTerrainType terrainID, BUILDINGS buildingId=0) {
		BUILDING_INFO.terrainID = terrainID;
		BUILDING_INFO.buildingID = buildingId;

		if (BUILDING_INFO.terrainID == TileTerrainType.BUILDING) {
			//setBuildingBuildingRepresentation ();
			//createSingleBuildingPanel ();
			createSingleBuildingFirstPanel();
		} else if (BUILDING_INFO.terrainID == TileTerrainType.RICE_TERRAIN) {
			//setBuildingRiceChunkRepresentation ();
			//createSingleBuildingPanel ();
			createSingleBuildingFirstPanel();
		} else if (BUILDING_INFO.terrainID == TileTerrainType.CANAL) {
			createMultipleBuildingPanel ();
		}

		if (_actionPanel != null)
			_actionPanel.SendMessage ("kill");

		logicState = LOGIC_STATE.BUILDING;
	}

	private void setBuildingBuildingRepresentation() {
		BUILDING_INFO.clearRepresentation (); // ++++++++++++++ no reload

		Building building = BuildingsManager.GetInstance ().getBuilding (BUILDING_INFO.buildingID);
		BUILDING_INFO.representation.Add(building.instantiateBuildingRepresentation(_tileSelectedPosition [0], _tileSelectedPosition [1]));

		//es pot construir?
		uint b_w = building.getHorizontalTiles ();
		uint b_h = building.getVerticalTiles ();
		uint b_i = _tileSelectedPosition [0];
		uint b_j = _tileSelectedPosition [1];
		WorldTerrain wt = WorldTerrain.GetInstance ();
		bool esPotConstruir = wt.isEmpty (b_i, b_j, b_w, b_h) && !wt.tileContainsVegetation (b_i, b_j);
		BUILDING_INFO.representation[0].GetComponent<SpriteRenderer>().color = esPotConstruir ? Color.yellow : Color.red;
		BUILDING_INFO.esPotConstruir = esPotConstruir;
	}

	private void setBuildingRiceChunkRepresentation() {
		BUILDING_INFO.clearRepresentation (); // ++++++++++++++ no reload
		BUILDING_INFO.esPotConstruir = true;

		WorldTerrain wt = WorldTerrain.GetInstance ();

		uint iSel = _tileSelectedPosition [0];
		uint jSel = _tileSelectedPosition [1];
		for (uint i = iSel - WorldTerrain.RICE_CHUNK_SEPARATION; i < iSel + WorldTerrain.RICE_CHUNK_H + WorldTerrain.RICE_CHUNK_SEPARATION; ++i) {
			for (uint j = jSel - WorldTerrain.RICE_CHUNK_SEPARATION; j < jSel + WorldTerrain.RICE_CHUNK_W + WorldTerrain.RICE_CHUNK_SEPARATION; ++j) {
				Vector3 worldPosition = wt.getTileWorldPosition(i, j);
				worldPosition.z = WorldTerrain.BUILDING_ABOVE_Z_LAYER;

				bool isSeparator = (i < iSel || i >= iSel + WorldTerrain.RICE_CHUNK_H ||
					j < jSel || j >= jSel + WorldTerrain.RICE_CHUNK_W);
				
				GameObject sprite_template = wt.getResource(isSeparator ? TileTerrainIDs.SEPARATOR : TileTerrainIDs.WATER);
				GameObject sprite_inst = (GameObject)GameObject.Instantiate (sprite_template, worldPosition, Quaternion.identity);
				bool esPotConstruir;
				if (isSeparator) {
					esPotConstruir = wt.getTypeTerrainOfTile (i, j) == TileTerrainType.SEPARATOR || wt.getTypeTerrainOfTile (i, j) == TileTerrainType.FREE || wt.getTypeTerrainOfTile (i, j) == TileTerrainType.CANAL;
				} else {
					esPotConstruir = wt.getTypeTerrainOfTile (i, j) == TileTerrainType.FREE || wt.getTypeTerrainOfTile (i, j) == TileTerrainType.CANAL;
				}
				esPotConstruir = esPotConstruir && !wt.tileContainsVegetation (i, j);
				sprite_inst.GetComponent<SpriteRenderer>().color = esPotConstruir ? Color.yellow : Color.red;

				BUILDING_INFO.esPotConstruir = BUILDING_INFO.esPotConstruir && esPotConstruir;
				BUILDING_INFO.representation.Add(sprite_inst);
			}
		}
	}

	public void constructionBuildingConfirm () {
		if (logicState == LOGIC_STATE.DELETING) {
			deletingConfirm ();
		} else {
			if (BUILDING_INFO.terrainID == TileTerrainType.BUILDING) {
				buildBuilding (BUILDING_INFO.buildingID, _tileSelectedPosition [0], _tileSelectedPosition [1]);

				Building building = BuildingsManager.GetInstance ().getBuilding (BUILDING_INFO.buildingID);
				building.constructAtPos (_tileSelectedPosition [0], _tileSelectedPosition [1]);
			} else if (BUILDING_INFO.terrainID == TileTerrainType.RICE_TERRAIN) {
				WorldTerrain.GetInstance ().createRiceChunk (_tileSelectedPosition [0], _tileSelectedPosition [1], false);
			} else if (BUILDING_INFO.terrainID == TileTerrainType.CANAL) {
				WorldTerrain wt = WorldTerrain.GetInstance ();
				wt.confirmCanalsTmp ();
				UserDataManager.GetInstance().gold.espendGold(_buildingPanel.GetComponent<MultipleBuildingPanel> ().getCost ());
			}
			BUILDING_INFO.clearRepresentation ();
		}

		logicState = LOGIC_STATE.DEFAULT;
	}

	public void constructionBuildingCancel () {
		if (logicState == LOGIC_STATE.DELETING) {
			deletingCancel ();
		} else {
			BUILDING_INFO.clearRepresentation ();

			if (BUILDING_INFO.terrainID == TileTerrainType.BUILDING) {
				Building building = BuildingsManager.GetInstance ().getBuilding (BUILDING_INFO.buildingID);
				UserDataManager.GetInstance ().gold.addGold (building.getInitialCost ());
			} else if (BUILDING_INFO.terrainID == TileTerrainType.RICE_TERRAIN) {
				UserDataManager.GetInstance ().gold.addGold (WorldTerrain.PRICE_RiceChunk);
			} else if (BUILDING_INFO.terrainID == TileTerrainType.CANAL) {
				WorldTerrain wt = WorldTerrain.GetInstance ();
				wt.cancelCanalsTmp ();
			}
		}

		if (_buildingPanel != null)
			_buildingPanel.SendMessage ("kill");
		
		logicState = LOGIC_STATE.DEFAULT;
	}

	private void buildBuilding(BUILDINGS buildingId, uint i, uint j) {
		BuildingsManager buildingsMan = BuildingsManager.GetInstance ();
		Building building = buildingsMan.getBuilding (buildingId);
		buildingsMan.build (buildingId, i, j);
		
		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		worldTerrain.createBuilding(i, j, building.getVerticalTiles(), building.getHorizontalTiles());

        if(buildingId == BUILDINGS.PLANTA) {
            Building_Trill trill = (Building_Trill)buildingsMan.getBuilding(BUILDINGS.TRILL);
            Building_Silo silo = (Building_Silo)buildingsMan.getBuilding(BUILDINGS.SILO);
            Building_Era era = (Building_Era)buildingsMan.getBuilding(BUILDINGS.ERA);

            trill.stopProduction();
            silo.stopProduction();
            era.stopProduction();

            uint rice = trill.getAllTheRice();
            rice += silo.getAllTheRice();
            rice += era.getAllTheRice();
            ((Building_Planta)building).sendRice(rice);

            destroyBuilding(BUILDINGS.TRILL);
            destroyBuilding(BUILDINGS.SILO);
            destroyBuilding(BUILDINGS.ERA);
        }
	}

    public void destroyBuilding(BUILDINGS buildingId)
    {
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
        Building building = buildingsMan.getBuilding(buildingId);
        buildingsMan.destroy(buildingId);

        WorldTerrain worldTerrain = WorldTerrain.GetInstance();
        worldTerrain.destroyBuilding(building.getRow(), building.getCol(), building.getVerticalTiles(), building.getHorizontalTiles());
    }

	private void createSingleBuildingFirstPanel()
	{
		Debug.Log ("createSingleBuildingFirstPanel");
		GameObject buildingPanelTemplate = Resources.Load ("Prefabs/BuildingFirstPanelTemplate") as GameObject;
		_buildingPanel = Instantiate (buildingPanelTemplate);
		_buildingPanel.GetComponent<BuildingConfirmFirstPanel> ().setLogicManager(this);
		//RectTransform canvas = GameObject.Find("UICanvas").GetComponent<RectTransform>();
		//_buildingPanel.GetComponent<RectTransform>().SetParent(canvas, false); 
		//_buildingPanel.GetComponent<BuildingConfirmPanel> ().setLogicManager(this);
	}

    private void createSingleBuildingPanel()
    {
		if (_buildingPanel != null) {
			GameObject.Destroy (_buildingPanel);
		}

		GameObject buildingPanelTemplate = Resources.Load ("Prefabs/BuildingPanelTemplate") as GameObject;
		_buildingPanel = Instantiate (buildingPanelTemplate);
		_buildingPanel.GetComponent<BuildingConfirmPanel> ().setLogicManager(this);

		//es pot construir?
		if (!BUILDING_INFO.esPotConstruir)
			_buildingPanel.SendMessage ("disableConfirm");
	}

	private void createMultipleBuildingPanel()
    {
		GameObject buildingPanelTemplate = Resources.Load ("Prefabs/MultipleBuildingPanelTemplate") as GameObject;
		_buildingPanel = Instantiate (buildingPanelTemplate);
		_buildingPanel.GetComponent<MultipleBuildingPanel> ().setLogicManager(this);
	}

	// DELETING
	public void startDeleting() {
		tilesToDelete = new List<uintPair> ();

		createMultipleBuildingPanel ();

		if (_actionPanel != null)
			_actionPanel.SendMessage ("kill");

		logicState = LOGIC_STATE.DELETING;
	}

	public void deletingConfirm() {
		foreach (uintPair pos in tilesToDelete) {
			uint i = pos.First;
			uint j = pos.Second;
			WorldTerrain wt = WorldTerrain.GetInstance ();
			TileTerrainType type = wt.getTypeTerrainOfTile (i, j);
			if (type == TileTerrainType.FREE && wt.tileContainsVegetation (i, j)) {
				wt.deleteVegetation (i, j);
			}
			else if (type == TileTerrainType.CANAL) {
				wt.deleteCanal (i, j);
			}
			else if (type == TileTerrainType.RICE_TERRAIN) {
				wt.deleteRiceChunk(i, j);
			}
		}
		UserDataManager.GetInstance().gold.espendGold(_buildingPanel.GetComponent<MultipleBuildingPanel> ().getCost ());
		logicState = LOGIC_STATE.DEFAULT;
	}

	public void deletingCancel() {
		foreach (uintPair pos in tilesToDelete) {
			unmarkToDelete (pos.First, pos.Second);
		}
		logicState = LOGIC_STATE.DEFAULT;
	}

	public bool markToDelete(uint i, uint j) {
		WorldTerrain wt = WorldTerrain.GetInstance ();
		TileTerrainType type = wt.getTypeTerrainOfTile (i, j);
		if (type == TileTerrainType.FREE && wt.tileContainsVegetation (i, j)) {
			wt.getTileVegetation (i, j).markToDelete ();
			_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (+1, +WorldTerrain.PRICE_Clean);
			return true;
		}
		/*
		if (type == TileTerrainType.CANAL) {
			wt.canalMarkToDelete(i, j);
			_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (+1, +WorldTerrain.PRICE_Clean);
			return true;
		}
		if (type == TileTerrainType.RICE_TERRAIN) {
			wt.riceChunkMarkToDelete(i, j);
			uintPair clickedPos = new uintPair (i, j);
			foreach(Pair<uint, uint> pos in wt.getChunkTilesPositions(wt.getRiceTerrain(i,j).getChunkNumber())) {
				uintPair terrainPos = new uintPair (pos.First, pos.Second);
				if (clickedPos != terrainPos)
					tilesToDelete.Add (terrainPos);
				_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (+1, +WorldTerrain.PRICE_Clean);
				
			}
			return true;
		}
		*/
		return false;
	}

	public void unmarkToDelete(uint i, uint j) {
		WorldTerrain wt = WorldTerrain.GetInstance ();
		TileTerrainType type = wt.getTypeTerrainOfTile (i, j);
		if (type == TileTerrainType.FREE && wt.tileContainsVegetation (i, j)) {
			wt.getTileVegetation (i, j).unmarkToDelete ();
			_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (-1, -WorldTerrain.PRICE_Clean);
		}
		else if (type == TileTerrainType.CANAL) {
			wt.canalUnmarkToDelete(i, j);
			_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (-1, -WorldTerrain.PRICE_Clean);
		}
		else if (type == TileTerrainType.RICE_TERRAIN) {
			wt.riceChunkUnmarkToDelete(i, j);
			uintPair clickedPos = new uintPair (i, j);
			foreach(Pair<uint, uint> pos in wt.getChunkTilesPositions(wt.getRiceTerrain(i,j).getChunkNumber())) {
				uintPair terrainPos = new uintPair (pos.First, pos.Second);
				if (clickedPos != terrainPos)
					tilesToDelete.Remove (terrainPos);
				_buildingPanel.GetComponent<MultipleBuildingPanel>().addUnits (-1, -WorldTerrain.PRICE_Clean);
			}
		}
	}

    public void instantiateLostPanel()
    {
        GameObject panelTemplate = Resources.Load("Prefabs/LostPanelLostFocusLayer") as GameObject;
        Instantiate(panelTemplate);
    }

    public void instantiateWarningInRedPanel()
    {
        GameObject panelTemplate = Resources.Load("Prefabs/InRedWarningPanelLostFocusLayer") as GameObject;
        Instantiate(panelTemplate);
    }

    public void load(LogicManagerData logicManagerData)
    {
        _tileSelectedPosition = new uint[2] { 26, 15 };
        _hasTileSelected = false;
        ActionManager.GetInstance();
        CoopManager.GetInstance();
        PenalizationManager.GetInstance();
        //WorldTerrain.GetInstance().initWorld();
        //BuildingsManager.GetInstance().init();
        UserDataManager.GetInstance().init();
        RiceObjectsManager.GetInstance();
        _cloudSpawner = new CloudSpawner(WorldTerrain.CLOUDS_Z_LAYER);
        gameObject.GetComponent<TimeManager>().addListenerToYearChange(this.happyNewYear);
        gameObject.GetComponent<TimeManager>().addListenerToMonthChange(happyNewMonth);
        gameObject.GetComponent<TimeManager>().addListerToDayChange(this.newDayCallback);

    }

    private void prepareTest()
	{
		//UserDataManager.GetInstance ().gold.setGold (1);
		//TutorialManager tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		//tutMan.disableTutorial ();
		//TestManager.GetInstance().BuildAllModernBuildings();
        //TestManager.GetInstance().BuyAllObjects(2);
        //TestManager.GetInstance().HireWorkers();
		//TestManager.GetInstance ().InvestigateEverything ();
        //TestManager.GetInstance().SowPlanter();
		//TestManager.GetInstance().MakeActionsTillPhase(TypeFase.HARVEST);
		//TestManager.GetInstance().AdvanceTillPhase(TypeFase.HARVEST);
		//gameObject.GetComponent<TimeManager> ().calendar._currentMonth = 11;
		//gameObject.GetComponent<TimeManager> ().calendar._currentDay = 20;

		/*
		TestManager.GetInstance().BuildAllBuildings();
		gameObject.GetComponent<TimeManager> ().calendar._currentYear = 1995;
		UserDataManager.GetInstance ().sellRice(44390);
		UserDataManager.GetInstance ().rice.setTotalRiceProduced (44390);
		*/
    }

    public void save(LogicManagerData logicManagerData)
    {
        logicManagerData.MonthsInRed = m_monthsInRed;
        logicManagerData.MonthsSinceIShowedPopup = m_monthsSinceIShowedInRedPopup;
        logicManagerData.WarningPopupShown = b_warningInRedPopupShown;
    }

    public void initAutoSave()
    {
        InvokeRepeating("autoSave", 0.0f, 60.0f);
    }

	void OnApplicationQuit() {
		Debug.Log("Application ending after " + Time.time + " seconds");
		autoSave ();
	}

    public void autoSave()
    {
        GameSaveDataManager.Save();
        //Thread oThread = new Thread(new ThreadStart(GameSaveDataManager.Save));
        //oThread.Start();

    }


}

