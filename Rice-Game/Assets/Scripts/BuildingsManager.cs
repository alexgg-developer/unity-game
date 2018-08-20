using System;
//using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public enum BUILDINGS { /* NULL=0 */ CASA=1, ESTABLO, PLANTER, TRILL, ERA, SILO, PLANTA};

public class BuildingsManager
{
	private static BuildingsManager instance = null;

	public static BuildingsManager GetInstance()
	{
		if(instance == null)
		{
			instance = new BuildingsManager();
		}
		
		return instance;
	}
	
	private BuildingsManager() {}

	private Dictionary<BUILDINGS, Building> _buildings;
	private Dictionary<BUILDINGS, bool> _isBuilded;
    public Dictionary<BUILDINGS, bool> IsBuilded
    {
        get
        {
            return _isBuilded;
        }
        private set { }
    }

    public void save(BuildingManagerData buildingManagerData)
    {
        foreach (KeyValuePair<BUILDINGS, bool> builded in IsBuilded) {
            if (builded.Value) {
                BuildingData buildingData = new BuildingData();
                buildingData.ID = builded.Key;
                switch (builded.Key) {
                    case BUILDINGS.CASA: {
                            Building_Home b = (Building_Home)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                        }
                        break;
                    case BUILDINGS.ESTABLO: {
                            Building_Stable b = (Building_Stable)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                        }
                        break;
                    //case BUILDINGS.GARAJE: {
                    //        Building_Garage b = (Building_Garage)getBuilding(builded.Key);
                    //        buildingData.i = b.getRow();
                    //        buildingData.j = b.getCol();
                    //        buildingData.Level = b.getCurrentLevel();
                    //    }
                    //    break;
                    case BUILDINGS.PLANTER: {
                            Building_Planter b = (Building_Planter)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                            buildingData.SpecialSlot1 = b.estaSembrat() ? 1 : 0;
                            buildingData.SpecialSlot2 = (int)b.DiesSembrat;
                            buildingData.SpecialSlot3 = (int)b.Plantes;
                        }
                        break;
                    case BUILDINGS.TRILL: {
                            Building_Trill b = (Building_Trill)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                            buildingData.SpecialSlot1 = (int)b.getRiceUnprepared();
                            buildingData.SpecialSlot2 = (int)b.getRicePrepared();
                        }
                        break;
                    case BUILDINGS.ERA: {
                            Building_Era b = (Building_Era)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                            buildingData.SpecialSlot1 = (int)b.getRiceUnprepared();
                            buildingData.SpecialSlot2 = (int)b.getRicePrepared();
                        }
                        break;
                    case BUILDINGS.SILO: {
                            Building_Silo b = (Building_Silo)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                            buildingData.SpecialSlot1 = (int)b.getAllTheRice();
                        }
                        break;
                    case BUILDINGS.PLANTA: {
                            Building_Planta b = (Building_Planta)getBuilding(builded.Key);
                            buildingData.i = b.getRow();
                            buildingData.j = b.getCol();
                            buildingData.Level = b.getCurrentLevel();
                            buildingData.SpecialSlot1 = (int)b.getRiceUnprepared();
                        }
                        break;
                }
                buildingManagerData.BuildingsBuilded.Add(buildingData);
            }
        }
    }

    public void load(BuildingManagerData buildingManagerData)
    {
        init();
        foreach(BuildingData buildingData in buildingManagerData.BuildingsBuilded) {
            _isBuilded[buildingData.ID] = true;
            Building b = _buildings[buildingData.ID];            
            switch (buildingData.ID) {
                case BUILDINGS.CASA: {
                        for(int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Home)b).upgrade();
                        }
                        ((Building_Home)b).constructAtPos(buildingData.i, buildingData.j);
                    }
                    break;
                case BUILDINGS.ESTABLO: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Stable)b).upgrade();
                        }
                        ((Building_Stable)b).constructAtPos(buildingData.i, buildingData.j);
                    }
                    break;
                //case BUILDINGS.GARAJE: {
                //        for (int i = 0; i < buildingData.Level; ++i) {
                //            ((Building_Garage)b).upgrade();
                //        }
                //        ((Building_Garage)b).constructAtPos(buildingData.i, buildingData.j);
                //    }
                //    break;
                case BUILDINGS.PLANTER: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Planter)b).upgrade();
                        }
                        ((Building_Planter)b).constructAtPos(buildingData.i, buildingData.j);
                        if (buildingData.SpecialSlot1 > 0) {
                            ((Building_Planter)b).sembrar((uint)buildingData.SpecialSlot2, (uint)buildingData.SpecialSlot3);
                        }
                    }
                    break;
                case BUILDINGS.TRILL: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Trill)b).upgrade();
                        }
                        ((Building_Trill)b).constructAtPos(buildingData.i, buildingData.j);
                        ((Building_Trill)b).setRice((uint)buildingData.SpecialSlot1, (uint)buildingData.SpecialSlot2);
                    }
                    break;
                case BUILDINGS.ERA: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Era)b).upgrade();
                        }
                        ((Building_Era)b).constructAtPos(buildingData.i, buildingData.j);
                        ((Building_Era)b).setRice((uint)buildingData.SpecialSlot1, (uint)buildingData.SpecialSlot2);
                    }
                    break;
                case BUILDINGS.SILO: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Silo)b).upgrade();
                        }
                        ((Building_Silo)b).constructAtPos(buildingData.i, buildingData.j);
                        ((Building_Silo)b).setRice((uint)buildingData.SpecialSlot1);
                    }
                    break;
                case BUILDINGS.PLANTA: {
                        for (int i = 0; i < buildingData.Level; ++i) {
                            ((Building_Planta)b).upgrade();
                        }
                        ((Building_Planta)b).constructAtPos(buildingData.i, buildingData.j);
                        ((Building_Planta)b).setRice((uint)buildingData.SpecialSlot1);
                    }
                    break;
            }
        }
    }

    //private List<BuildingAction> _action;

    public bool init() {
		_buildings = new Dictionary<BUILDINGS, Building> ();
		_isBuilded = new Dictionary<BUILDINGS, bool> ();
		if (!loadBuildings()) return false;
		return true;
	}

	private bool loadBuildings()
    {
		string file_path = "json/buildings";
		TextAsset file = Resources.Load(file_path, typeof(TextAsset)) as TextAsset;
		if (file != null) { 
			JSONNode N = JSON.Parse (file.text);
			foreach (BUILDINGS buildingID in Enum.GetValues(typeof(BUILDINGS))) {
				string buildingStr = buildingID.ToString ();
				switch(buildingID) {
				case BUILDINGS.CASA:					
					_buildings [buildingID] = new Building_Home (N [buildingStr], buildingID);
					break;
				case BUILDINGS.ESTABLO:					
					_buildings [buildingID] = new Building_Stable(N [buildingStr], buildingID);
					break;
				//case BUILDINGS.GARAJE:					
				//	_buildings [buildingID] = new Building_Garage(N [buildingStr], buildingID);
				//	break;
				case BUILDINGS.PLANTER:					
					_buildings [buildingID] = new Building_Planter (N [buildingStr], buildingID);
					break;
				case BUILDINGS.TRILL:					
					_buildings [buildingID] = new Building_Trill (N [buildingStr], buildingID);
					break;
				case BUILDINGS.ERA:
					_buildings [buildingID] = new Building_Era (N [buildingStr], buildingID);
					break;
				case BUILDINGS.SILO:					
					_buildings [buildingID] = new Building_Silo (N [buildingStr], buildingID);
					break;
                case BUILDINGS.PLANTA:					
					_buildings [buildingID] = new Building_Planta (N [buildingStr], buildingID);
					break;
				default:
					_buildings [buildingID] = new Building (N [buildingStr], buildingID);
					break;
				}
				_isBuilded [buildingID] = false;
			}
			return true;
		}
		Debug.Log(file_path + " not loaded");
		return false;
	}

	public bool isBuilded(BUILDINGS building) {
        Debug.Assert (_isBuilded.ContainsKey(building));
        return _isBuilded[building];
	}
	
	public Building getBuilding(BUILDINGS building) {
		Debug.Assert (_buildings.ContainsKey(building));
		return _buildings[building];
	}

	public Building build(BUILDINGS building, uint i, uint j) {
		Debug.Assert (!_isBuilded[building]);
		Building b = _buildings [building];
		b.constructAtPos(i, j);
		_isBuilded [building] = true;		

		return _buildings[building];
    }

    public void destroy(BUILDINGS building)
    {
        Debug.Assert(_isBuilded[building]);
        Building b = _buildings[building];
        b.destroy();
        _isBuilded[building] = false;
    }

    public void upgrade(BUILDINGS building) {
		Debug.Assert (_isBuilded[building]);
		Building b = _buildings [building];
		b.upgrade();
	}
	
	public Building getBuildingInPosition(uint row, uint col)
	{
		BUILDINGS buildingID = getTypeOfBuildingInPosition (row, col);
		if (buildingID > 0)
			return _buildings[buildingID];
		return null;
	}
	
	public BUILDINGS getTypeOfBuildingInPosition(uint row, uint col) // *** optimizable en tiempo
	{
		foreach (BUILDINGS buildingID in Enum.GetValues(typeof(BUILDINGS))) {
			if(_isBuilded[buildingID]) {
				Building b = _buildings[buildingID];
				uint colBuilding = b.getCol();
				uint horizontalTiles = b.getHorizontalTiles();
				uint rowBuilding = b.getRow();
				uint verticalTiles = b.getVerticalTiles();
				bool hitCollision = colBuilding <= col  && col < colBuilding + horizontalTiles &&
					rowBuilding <= row && row < rowBuilding + verticalTiles;
				if(hitCollision) {
					return buildingID;
				}
			}
		}
		return 0;
	}

	public void selectBuildingAt(uint row, uint col) {
		Building b = getBuildingInPosition (row, col);
		b.select ();
	}
	
	public void unselectBuildingAt(uint row, uint col) {
		Building b = getBuildingInPosition (row, col);
		b.unselect ();
	}

	public void newDayCallback()
	{
		foreach (BUILDINGS building in Enum.GetValues(typeof(BUILDINGS))) {
			_buildings[building].newDayCallback();
		}
	}
    
	public void reset() {
		if(_isBuilded[BUILDINGS.PLANTER]) {
            ((Building_Planter)_buildings[BUILDINGS.PLANTER]).reset();
        }
	}
}
