using UnityEngine;
//using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using MyUtils.Pair;

public struct ConstructionLevel 
{
	public int cost;
	public int capacity1;
	public int capacity2;
	public string imgPath;
	//string info;
}

public class Building
{
	private List<ConstructionLevel> _constructionLevels;
	private int _level;
	private string _info;
	private uint _i, _j; //top-left corner
	private uint _horizontalTiles, _verticalTiles;
	private BUILDINGS _type;
	protected string _currentImgPath; //this should change with the state || to access this from child use base._currentImgPath
	private GameObject _representation;// instance
	private bool _selected;
    private string _description = "";
    public string Description
    {
        get
        {
            return _description;
        }
        private set { }
    }
	private Material _defaultMaterial;
	private Material _outlineMaterial;
    private bool _hasActions;
    public bool HasActions
    {
        get
        {
            return _hasActions;
        }
        private set { }
    }

    protected List<Pair<string, string>> m_specificBuildingInfo = new List<Pair<string, string>>();
    public List<Pair<string, string>> BuildingInfo
    {
        get
        {
            return m_specificBuildingInfo;
        }
        set
        {
            m_specificBuildingInfo = value;
        }
    }

	public Building(JSONNode data, BUILDINGS type) {
		_level = 0;
		_i = _j = 0;
		_info = Dictionary.getString(data ["info"]);
		Debug.Assert (_info != null);
		_horizontalTiles = (uint) data ["size"][0].AsInt;
		_verticalTiles = (uint) data ["size"][1].AsInt;
        _description = Dictionary.getString(data["description"].Value);
        _hasActions = data["hasActions"].AsBool;
        //_constructionLevels = JsonConvert.DeserializeObject<List<ConstructionLevel>> (data ["levels"]);
        _constructionLevels = new List<ConstructionLevel> ();
		for (int i = 0; i < data["levels"].Count; ++i) {
			JSONNode level = data["levels"][i];
			ConstructionLevel cl = new ConstructionLevel();
			cl.cost = level["cost"].AsInt;
			cl.capacity1 = level["capacity1"].AsInt;
			cl.capacity2 = level["capacity2"].AsInt;
			cl.imgPath = level["img"];
			_constructionLevels.Add(cl);
		}
		_currentImgPath = _constructionLevels [0].imgPath;
		_type = type;
		
		_selected = false;
		//_outlineMaterial = Resources.Load("Materials/Outline", typeof(Material)) as Material;
	}

	public virtual void upgrade() {
		++_level;
		Debug.Assert (_level < _constructionLevels.Count);
	}

	public bool hasNextLevel() {
		return _level+1 < _constructionLevels.Count;
	}
	
	public int getInitialCost() {
		Debug.Assert (_constructionLevels.Count > 0);
		return _constructionLevels [0].cost;
	}

	public int getCapacity1() {
        int capacity = _constructionLevels[_level].capacity1;
        Pair<int, int> bonus = CoopManager.GetInstance().getCurrentAnualProductionBonus();
        capacity += (capacity * bonus.First) / 100;
        return capacity;
	}

	public int getCapacity2()
    {
        int capacity = _constructionLevels[_level].capacity2;
        Pair<int, int> bonus = CoopManager.GetInstance().getCurrentAnualProductionBonus();
        capacity += (capacity * bonus.Second) / 100;
        return capacity;
	}

	public int getNextLevelCost() {
		Debug.Assert (_constructionLevels.Count > _level+1);
		return _constructionLevels [_level+1].cost;
	}

    public int getCurrentCapacity()
    {
        return _constructionLevels[_level].capacity1;
    }

	public int getNextLevelCapacity1() {
		Debug.Assert (_constructionLevels.Count > _level+1);
		return _constructionLevels [_level+1].capacity1;
	}
	
	public int getNextLevelCapacity2() {
		Debug.Assert (_constructionLevels.Count > _level+1);
		return _constructionLevels [_level+1].capacity2;
	}

	public string getInfo() {
		return _info;
	}

	public string getImgPath() {
		Debug.Assert (_constructionLevels.Count > _level);
		return _constructionLevels [_level].imgPath;
	}

	public string getNextImgPath() {
		Debug.Assert (_constructionLevels.Count > _level+1);
		return _constructionLevels [_level+1].imgPath;
	}

	public void constructAtPos(uint i, uint j) {
		_i = i;
		_j = j;
		updateRepresentation ();
	}

	public void destroy() {
		Debug.Assert (_representation != null);
		GameObject.Destroy (_representation);
	}

	public uint getRow() { return _i;}

	public uint getCol() { return _j;}

	public uint getHorizontalTiles() { return _horizontalTiles;}
	
	public uint getVerticalTiles() { return _verticalTiles;}

	public BUILDINGS getType() { return _type;}
	
	public virtual void newDayCallback() { }

	public virtual string getCurrentImgPath()
	{
		return _currentImgPath;
	}

	public void select() {
		if (!_selected) {
			_selected = true;
			//_defaultMaterial = _representation.gameObject.GetComponent<Renderer> ().material;
			//_representation.gameObject.GetComponent<Renderer> ().material = _outlineMaterial;
			_representation.transform.GetChild (0).gameObject.SetActive (true);
		}
	}

	public void unselect() {
		if (_selected) {
			_selected = false;
			//_representation.gameObject.GetComponent<Renderer> ().material = _defaultMaterial;
			_representation.transform.GetChild (0).gameObject.SetActive (false);
		}
	}

	public void markToDelete() {
		_representation.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public void unmarkToDelete() {
		_representation.GetComponent<SpriteRenderer>().color = Color.white;
	}

	protected void updateRepresentation()
	{
		if (_representation != null)
			GameObject.Destroy (_representation);
		
		_representation = instantiateBuildingRepresentation (getRow (), getCol ());
	}

	public GameObject instantiateBuildingRepresentation(uint row, uint col) {
		GameObject buildingGameObject = null;
		try {
			buildingGameObject = Resources.Load<GameObject>(this.getCurrentImgPath());		
		}
		catch(Exception e) {
			Debug.LogError (e.ToString());
			Debug.LogError ("buildingGameObject exception for " + _type.ToString());
			Debug.LogError ("path to building object " + this.getCurrentImgPath());
		}
		//RiceTerrain terrainRepresentation = GameObject.FindGameObjectWithTag ("Map").GetComponent<RiceTerrain> ();
		WorldTerrain wt = WorldTerrain.GetInstance ();
		Vector3 worldPosition = wt.getTileWorldPosition(row, col);

		float map_top = wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);
		float map_h = wt.getTileWorldPositionY (0, wt.getNumTilesX () - 1) - wt.getTileWorldPositionY (wt.getNumTilesY () - 1, 0);

		Vector3 buildingSize = buildingGameObject.GetComponent<SpriteRenderer> ().bounds.size;
		worldPosition.x -= buildingSize.x / 2 + WorldTerrain.TILE_WIDTH_UNITS/2;
		worldPosition.y += buildingSize.y / 2;
		worldPosition.x += WorldTerrain.TILE_WIDTH_UNITS * getHorizontalTiles();
		worldPosition.y -= WorldTerrain.TILE_HEIGHT_UNITS/2	 * getVerticalTiles();

		worldPosition.z = WorldTerrain.BUILDING_Z_LAYER - (map_top - (worldPosition.y))/map_h;

		return (GameObject) GameObject.Instantiate(buildingGameObject, worldPosition, Quaternion.identity);
	}

    public int getCurrentLevel()
    {
        return _level;
    }

    public virtual List<Pair<string, string>> getSpecificBuildingInfo()
    {
        return m_specificBuildingInfo;
    }

    public virtual string getWorkInfo()
    {
        return "";
    }
}