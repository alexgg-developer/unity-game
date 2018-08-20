using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Pair;

public enum CANAL_CONEXION {/*NULL=0*/
	NONE=1,
	UP, DOWN, LEFT, RIGHT,
	I_VERTICAL, I_HORIZONTAL,
	C_LEFT_UP, C_UP_RIGHT, C_RIGHT_DOWN, C_DOWN_LEFT,
	T_UP, T_DOWN, T_LEFT, T_RIGHT,
	X
};

public class CanalManager {
	private CanalTile[,] _canalsMatrix;
	private CanalTile[,] _canalsMatrixTmp;
	private int[,] _riceChunkMatrix;
	private Dictionary<int, bool> _riceChunkHasWater;
	private uint _mapW, _mapH;
	private List<uintPair> _waterSources;

	static readonly Dictionary<CANAL_CONEXION, bool[,]> conexionMatrixs = new Dictionary<CANAL_CONEXION, bool[,]> {
		{CANAL_CONEXION.NONE, new bool[3,3] {{false, false, false}, {false, true, false}, {false, false, false}}},
		{CANAL_CONEXION.UP, new bool[3,3] {{false, true, false}, {false, true, false}, {false, false, false}}},
		{CANAL_CONEXION.DOWN, new bool[3,3] {{false, false, false}, {false, true, false}, {false, true, false}}},
		{CANAL_CONEXION.LEFT, new bool[3,3] {{false, false, false}, {true, true, false}, {false, false, false}}},
		{CANAL_CONEXION.RIGHT, new bool[3,3] {{false, false, false}, {false, true, true}, {false, false, false}}},
		{CANAL_CONEXION.I_VERTICAL, new bool[3,3] {{false, true, false}, {false, true, false}, {false, true, false}}},
		{CANAL_CONEXION.I_HORIZONTAL, new bool[3,3] {{false, false, false}, {true, true, true}, {false, false, false}}},
		{CANAL_CONEXION.C_LEFT_UP, new bool[3,3] {{false, true, false}, {true, true, false}, {false, false, false}}},
		{CANAL_CONEXION.C_UP_RIGHT, new bool[3,3] {{false, true, false}, {false, true, true}, {false, false, false}}},
		{CANAL_CONEXION.C_RIGHT_DOWN, new bool[3,3] {{false, false, false}, {false, true, true}, {false, true, false}}},
		{CANAL_CONEXION.C_DOWN_LEFT, new bool[3,3] {{false, false, false}, {true, true, false}, {false, true, false}}},
		{CANAL_CONEXION.T_UP, new bool[3,3] {{false, true, false}, {true, true, true}, {false, false, false}}},
		{CANAL_CONEXION.T_DOWN, new bool[3,3] {{false, false, false}, {true, true, true}, {false, true, false}}},
		{CANAL_CONEXION.T_LEFT, new bool[3,3] {{false, true, false}, {true, true, false}, {false, true, false}}},
		{CANAL_CONEXION.T_RIGHT, new bool[3,3] {{false, true, false}, {false, true, true}, {false, true, false}}},
		{CANAL_CONEXION.X, new bool[3,3] {{false, true, false}, {true, true, true}, {false, true, false}}}
	};

    public CanalManager()
    {
        CanalTile.initResources();
    }

    public CanalManager(uint w, uint h) {
		_mapW = w;
		_mapH = h;

		_waterSources = new List<uintPair> ();

		_canalsMatrix = new CanalTile[_mapH, _mapW];
		_canalsMatrixTmp = new CanalTile[_mapH, _mapW];

		_riceChunkHasWater = new Dictionary<int, bool> ();

		_riceChunkMatrix = new int[_mapH,_mapW];
		for (int i = 0; i < _mapH; ++i)
			for (int j = 0; j < _mapW; ++j)
				_riceChunkMatrix [i, j] = -1;
	
		CanalTile.initResources ();
	}

	public void addWaterSource(uint i, uint j) {
		_waterSources.Add (new uintPair (i, j));
	}

	// CANAL
	public bool tileHasCanal(uint i, uint j) {
		return _canalsMatrix [i, j] != null;
	}

	private bool tileHasRice(uint i, uint j) {
		return _riceChunkMatrix [i, j] >= 0;
	}

	public void addCanal(uint i, uint j, bool update=true) {
		Debug.Assert (!tileHasCanal (i, j));
		_canalsMatrix [i, j] = new CanalTile (i, j, false);
		if (update) updateCanals ();
	}

	public void deleteCanal(uint i, uint j) {
		_canalsMatrix [i, j].delete ();
		_canalsMatrix [i, j] = null;
		updateCanals ();
	}

	// CANAL TMP
	public bool tileHasCanalTmp(uint i, uint j) {
		return _canalsMatrixTmp [i, j] != null;
	}

    public void addCanalTmp(uint i, uint j, bool update=true) {
		Debug.Log ("CanalManager::addCanalTmp");
		Debug.Assert (!tileHasCanal (i, j));
		Debug.Assert (!tileHasCanalTmp (i, j));
		_canalsMatrixTmp [i, j] = new CanalTile (i, j, true);
		if (update) updateCanals ();
	}

	public void removeCanalTmp(uint i, uint j, bool update=true) {
		_canalsMatrixTmp [i, j].delete ();
		_canalsMatrixTmp [i, j] = null;
		if (update) updateCanals ();
	}

	public void confirmCanalsTmp() {
		for (uint i = 0; i < _mapH; ++i) {
			for (uint j = 0; j < _mapW; ++j) {
				if (tileHasCanalTmp(i, j)) {
					removeCanalTmp (i, j, false);
					addCanal (i, j, false);
					//canalsMatrix [i, j] = canalsMatrixTmp [i, j];
					//canalsMatrixTmp [i, j] = null;
				}
			}
		}
		updateCanals ();
	}

	public void cancelCanalsTmp() {
		Debug.Log ("cancelCanalsTmp");
		for (int i = 0; i < _mapH; ++i) {
			for (int j = 0; j < _mapW; ++j) {
				if (_canalsMatrixTmp [i, j] != null) {
					_canalsMatrixTmp [i, j].delete ();
					_canalsMatrixTmp [i, j] = null;
				}
			}
		}
		updateCanals ();
	}

	public List<CanalTile> getCanalsTmp() {
		List<CanalTile> list = new List<CanalTile> ();

		for (int i = 0; i < _mapH; ++i) {
			for (int j = 0; j < _mapW; ++j) {
				if (_canalsMatrixTmp [i, j] != null) {
					list.Add (_canalsMatrixTmp [i, j]);
				}
			}
		}

		return list;
	}

	// RICE CHUNKS
	public void addRiceChunk(int id, uint i, uint j, uint w, uint h) {
		for (uint ii = i; ii < i + h; ++ii) {
			for (uint jj = j; jj < j + w; ++jj) {
				_riceChunkMatrix [ii, jj] = id;
				if (tileHasCanal (ii, jj)) {
					deleteCanal (ii, jj);
				}
			}
		}
		_riceChunkHasWater [id] = false;
		updateCanals ();
	}

	public void removeRiceChunk(uint i, uint j, uint w, uint h) {
		int id = _riceChunkMatrix [i, j];
		for (uint ii = i; ii < i + h; ++ii) {
			for (uint jj = j; jj < j + w; ++jj) {
				_riceChunkMatrix [ii, jj] = -1;
			}
		}
		_riceChunkHasWater.Remove (id);
		updateCanals ();
	}

	public bool riceChunkHasWater(int id) {
		Debug.Assert (_riceChunkHasWater.ContainsKey (id), "Rice chunk not registered: " + id);

		return _riceChunkHasWater [id];
	}

	// CONEXIONS
	public CANAL_CONEXION getCanalConexion(uint i, uint j) {
		if (_canalsMatrix [i, j] == null)
			return CANAL_CONEXION.NONE;
		return _canalsMatrix [i, j].getConexionType ();
	}

	private CANAL_CONEXION calculateCanalConexion(uint i, uint j) {
		if (j < 9) return CANAL_CONEXION.I_HORIZONTAL;
		if (j == _mapW-1) return CANAL_CONEXION.I_HORIZONTAL;


		// OPT
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.NONE], i, j)) return CANAL_CONEXION.NONE;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.UP], i, j)) return CANAL_CONEXION.UP;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.DOWN], i, j)) return CANAL_CONEXION.DOWN;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.LEFT], i, j)) return CANAL_CONEXION.LEFT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.RIGHT], i, j)) return CANAL_CONEXION.RIGHT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.I_HORIZONTAL], i, j)) return CANAL_CONEXION.I_HORIZONTAL;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.I_VERTICAL], i, j)) return CANAL_CONEXION.I_VERTICAL;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.C_LEFT_UP], i, j)) return CANAL_CONEXION.C_LEFT_UP;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.C_UP_RIGHT], i, j)) return CANAL_CONEXION.C_UP_RIGHT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.C_RIGHT_DOWN], i, j)) return CANAL_CONEXION.C_RIGHT_DOWN;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.C_DOWN_LEFT], i, j)) return CANAL_CONEXION.C_DOWN_LEFT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.T_UP], i, j)) return CANAL_CONEXION.T_UP;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.T_DOWN], i, j)) return CANAL_CONEXION.T_DOWN;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.T_LEFT], i, j)) return CANAL_CONEXION.T_LEFT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.T_RIGHT], i, j)) return CANAL_CONEXION.T_RIGHT;
		if (matchMatrixCanalConnexion(conexionMatrixs[CANAL_CONEXION.X], i, j)) return CANAL_CONEXION.X;

		return 0;
	}

	private bool matchMatrixCanalConnexion(bool[,] conexion_mat, uint i, uint j) {
		int[,] dirs_matrix = new int[5, 2] {
			{ -1,  0 },
			{  0, -1 }, {  0,  0 }, {  0, +1 },
			{ +1,  0 }
		};

		for (int k = 0; k < 5; ++k) {
			int tile_i = (int) i + dirs_matrix [k, 0];
			int tile_j = (int) j + dirs_matrix [k, 1];

			bool tile_outside = tile_i < 0 || tile_j < 0 || tile_i >= _mapH || tile_j >= _mapW;
			if (tile_outside) {
				if (conexion_mat [1 + dirs_matrix [k, 0], 1 + dirs_matrix [k, 1]])
					return false;
			} else {
				bool tileHasAnyCanal =
					tileHasCanal((uint) tile_i, (uint) tile_j) ||
					tileHasCanalTmp((uint) tile_i, (uint) tile_j) ||
					tileHasRice((uint) tile_i, (uint) tile_j);
				if (conexion_mat [1 + dirs_matrix [k, 0], 1 + dirs_matrix [k, 1]] != tileHasAnyCanal)
					return false;
			}
		}

		return true;
	}

	public void updateCanals() {
		for (uint i = 0; i < _mapH; ++i) {
			for (uint j = 0; j < _mapW; ++j) {
				if (tileHasCanal (i, j)) {
					CANAL_CONEXION conexion = calculateCanalConexion (i, j);
					_canalsMatrix [i, j].setHasWater (false, false);
					_canalsMatrix [i, j].setConexionType (conexion, true);
				}
				else if (tileHasCanalTmp (i, j)) {
					CANAL_CONEXION conexion = calculateCanalConexion (i, j);
					_canalsMatrixTmp [i, j].setHasWater (false, false);
					_canalsMatrixTmp [i, j].setConexionType (conexion, true);
				}
			}
		}


		// Calculate Water
		bool[,] visited = new bool[_mapH, _mapW];
		Stack<uintPair> toVisit = new Stack<uintPair> (_waterSources);
		//_riceChunkHasWater.Clear ();
		List<int> riceChunkIDs = new List<int>(_riceChunkHasWater.Keys);
		foreach (int key in riceChunkIDs)
			_riceChunkHasWater [key] = false;

		while (toVisit.Count > 0) {
			uintPair pos = toVisit.Pop ();
			if (visited [pos.First, pos.Second])
				continue;

			visited [pos.First, pos.Second] = true;
			if (tileHasCanal (pos.First, pos.Second))
				_canalsMatrix [pos.First, pos.Second].setHasWater (true, false);
			else if (tileHasCanalTmp (pos.First, pos.Second))
				_canalsMatrixTmp [pos.First, pos.Second].setHasWater (true, false);
			else if (tileHasRice (pos.First, pos.Second)) {
				int riceChunkID = _riceChunkMatrix [pos.First, pos.Second];
				_riceChunkHasWater [riceChunkID] = true;
			}
			else
				continue;

			CANAL_CONEXION conexion = calculateCanalConexion (pos.First, pos.Second);
			int[,] dirs_matrix = new int[4, 2] {
				{ -1,  0 },
				{  0, -1 }, {  0, +1 },
				{ +1,  0 }
			};
			for (int k = 0; k < 4; ++k) {
				int tile_i = (int)pos.First + dirs_matrix [k, 0];
				int tile_j = (int)pos.Second + dirs_matrix [k, 1];

				bool tile_outside = tile_i < 0 || tile_j < 0 || tile_i >= _mapH || tile_j >= _mapW;
				if (!tile_outside && conexionMatrixs[conexion][1+dirs_matrix [k, 0], 1+dirs_matrix [k, 1]]) {
					toVisit.Push (new uintPair ((uint) tile_i, (uint) tile_j));
				}
			}
		}


		for (uint i = 0; i < _mapH; ++i) {
			for (uint j = 0; j < _mapW; ++j) {
				if (tileHasCanal (i, j)) {
					_canalsMatrix [i, j].updateRepresentation();
				} else if (tileHasCanalTmp (i, j)) {
					_canalsMatrixTmp [i, j].updateRepresentation();
				}
			}
		}
	}

	public void markToDeleteCanal(uint i, uint j) {
		Debug.Assert(tileHasCanal(i, j));
		_canalsMatrix [i, j].markToDelete();
	}

	public void unmarkToDeleteCanal(uint i, uint j) {
		Debug.Assert(tileHasCanal(i, j));
		_canalsMatrix [i, j].unmarkToDelete();
    }

    public void save(CanalManagerData canalManagerData)
    {
        canalManagerData.CanalsMatrix = new CanalTileData[_mapH, _mapW];
        for (int i = 0; i < _mapH; ++i) {
            for (int j = 0; j < _mapW; ++j) {
                if (_canalsMatrix[i, j] != null) {
                    uint row = _canalsMatrix[i, j].getRow();
                    uint col = _canalsMatrix[i, j].getCol();
                    bool confirmed = _canalsMatrix[i, j].isConfirmed();
                    canalManagerData.CanalsMatrix[i, j] = new CanalTileData(row, col, confirmed);
                }
                else {
                    canalManagerData.CanalsMatrix[i, j] = null;
                }
            }
        }
        canalManagerData.RiceChunkHasWater = _riceChunkHasWater;
        canalManagerData.RiceChunkMatrix = _riceChunkMatrix;
        canalManagerData.MapWidth = _mapW;
        canalManagerData.MapHeight = _mapH;
        canalManagerData.WaterSources = _waterSources;
    }

    public void load(CanalManagerData canalManagerData)
    {
        _canalsMatrix = new CanalTile[canalManagerData.MapHeight, canalManagerData.MapWidth];
        _canalsMatrixTmp = new CanalTile[canalManagerData.MapHeight, canalManagerData.MapWidth];
        for (int i = 0; i < canalManagerData.MapHeight; ++i) {
            for (int j = 0; j < canalManagerData.MapWidth; ++j) {
                if (canalManagerData.CanalsMatrix[i, j] != null) {
                    uint row = canalManagerData.CanalsMatrix[i, j].Row;
                    uint col = canalManagerData.CanalsMatrix[i, j].Col;
                    bool confirmed = canalManagerData.CanalsMatrix[i, j].Confirmed;
                    _canalsMatrix[i, j] = new CanalTile(row, col, !confirmed);
                }
            }
        }

        _riceChunkHasWater = canalManagerData.RiceChunkHasWater;
        _riceChunkMatrix = canalManagerData.RiceChunkMatrix;
        _mapW = canalManagerData.MapWidth;
        _mapH = canalManagerData.MapHeight;
        _waterSources = canalManagerData.WaterSources;

        updateCanals();
    }
}

public class CanalTile {
	private CANAL_CONEXION conexionType;
	private uint row, col;
	private bool hasWater;
	private bool confirmed;

	private GameObject representation;
	private static Dictionary<CANAL_CONEXION, GameObject> resources;

	public CanalTile() {}

	public CanalTile(uint i, uint j, bool isTmp) {
		row = i;
		col = j;
		hasWater = false;
		confirmed = !isTmp;
	}
    public bool isConfirmed()
    {
        return confirmed;
    }

    public uint getRow() {
		return row;
	}

	public uint getCol() {
		return col;
	}

	public static void initResources() {
		Debug.Log ("CanalTile : Loading Resources ... ");

		resources = new Dictionary<CANAL_CONEXION, GameObject> ();
		foreach (CANAL_CONEXION conexionID in Enum.GetValues(typeof(CANAL_CONEXION))) {
			GameObject conexionGameObject = null;
			String spritePath = "Textures/terrainTiles/Canal_"+conexionID.ToString();
			try {
				conexionGameObject = Resources.Load<GameObject> (spritePath);
			} catch (Exception e) {
				Debug.LogError (e.ToString ());
				Debug.LogError ("buildingGameObject exception for " + "asdf!");
				Debug.LogError ("path to building object " + "asdf!");
			}
			Debug.Assert (conexionGameObject != null, "Terrain not loaded: " + spritePath);
			resources.Add(conexionID, conexionGameObject);
		}
		Debug.Log ("CanalTile : Loading Resources [OK] ");
	}

	public void delete() {
		if (representation != null)
			GameObject.Destroy (representation);
	}

	public void setConexionType(CANAL_CONEXION type, bool update=true) {
		conexionType = type;
		if (update) updateRepresentation ();
	}

	public void setHasWater(bool hasWater, bool update=true) {
		this.hasWater = hasWater;
		if (update) updateRepresentation ();
	}

	public CANAL_CONEXION getConexionType() {
		return conexionType;
	}

	public void updateRepresentation() {
		if (representation != null)
			GameObject.Destroy (representation);

		WorldTerrain worldTerrain = WorldTerrain.GetInstance ();
		Vector3 worldPosition = worldTerrain.getTileWorldPosition(row, col);
		worldPosition.z = WorldTerrain.TERRAIN_Z_LAYER;
		if (!confirmed) {
			worldPosition.z -= 6;
		}

		Debug.Assert (resources != null);
		Debug.Assert(resources.ContainsKey(conexionType), "Resource para Canal no encontrado: "+ conexionType.ToString());
		representation = (GameObject) GameObject.Instantiate(resources[conexionType], worldPosition, Quaternion.identity);

		if (!confirmed) {
			representation.GetComponent<SpriteRenderer> ().color = Color.yellow;
		}

		representation.transform.GetChild (0).gameObject.SetActive(!hasWater);
	}

	public void markToDelete() {
		representation.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public void unmarkToDelete() {
		representation.GetComponent<SpriteRenderer>().color = Color.white;
	}
}
