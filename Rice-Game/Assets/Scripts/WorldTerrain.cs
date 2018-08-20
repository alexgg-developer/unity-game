using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MyUtils.Pair;
using SimpleJSON;

/* 
 * Esta clase contiene la informacion (tanto logica como representacion) del terreno en general.
 * Basicamente tiene una matriz donde se guarda el tipo de terreno por cada tile
 *   (canales, separadores, building, tile de arroz, etc.)
 * Se encarga de inicializar el mapa.
 * Gestiona las representaciones de los tiles (excepto si es arroz o un building
 *   las cuales la manejas las respectivas clases)
 * Tiene una matriz con los distintos tiles de arroz (RiceTerrainRice)
 */

public enum TileTerrainType {/*NULL=0*/FREE=1, DECORATION, BUILDING, CANAL, SEPARATOR, RICE_TERRAIN};
public enum TileTerrainIDs {
	WATER=1,
	SAND=2,
	SAND_HERBS_1=3,
	HERBS=4,
	FOREST=5,
	WATER_SAND_1=6,
	WATER_SAND_2=7,
	WATER_SAND_3=8,
	WATER_SAND_4=9,
	HERBS_FOREST=10,
	SEPARATOR=34
};
public enum TileOthersIDs {
	CANAL=33,
	RICE_CHUNK=35,
	WATER_SOURCE=36
};
public enum VegID {plant_0=21, plant_1=22, plant_2=23, tree_0=24, tree_1=25,
	cane_1=27, cane_2=28, cane_3=29, cane_4=30, cane_5=31, cane_6=32};

public class WorldTerrain {
	public static int PRICE_RiceChunk = 1800;
	public static int PRICE_Canal = 40;
	public static int PRICE_Clean = 20;

	// SINGLETON
	private static WorldTerrain instance = null;
	
	public static WorldTerrain GetInstance()
	{
		if(instance == null)
		{
			instance = new WorldTerrain();
		}
		
		return instance;
	}
	
	private WorldTerrain() 
	{
	}

	
	private uint numTilesX, numTilesY;
	//private const uint _TILES_OF_RIVER = 3;
	private const int INITIAL_NUMBER_OF_CHUNKS_HORIZONTAL = 4; //cols
	private const int INITIAL_NUMBER_OF_CHUNKS_VERTICAL = 4; //rows
	public const int RICE_CHUNK_W = 3;
	public const int RICE_CHUNK_H = 4;
	public const int RICE_CHUNK_SEPARATION = 1;
	//private const int _TILES_FOR_BUILDINGS = 2;
	private const int TILES_FOREST = 8;
	private const int TILES_HERB = 22; //Margin of herb || must be even

	private const int TILE_WIDTH = 339;
	private const int TILE_HEIGHT = 202;
	private const float UNITS_PER_PIXEL = 1 / 100.0f; // +++++
	public const float TILE_WIDTH_UNITS = TILE_WIDTH * UNITS_PER_PIXEL;
	public const float TILE_HEIGHT_UNITS = TILE_HEIGHT * UNITS_PER_PIXEL;

	public const float WATER_Z_LAYER = -1.0f;
	public const float CLOUDS_Z_LAYER = -2.0f;
	public const float TERRAIN_Z_LAYER = -3.0f;
	public const float PLANTS_Z_LAYER = -5.0f;
	public const float ANIMATION_Z_LAYER = -6.0f;
	public const float BUILDING_Z_LAYER = -5.0f;
	public const float BUILDING_ABOVE_Z_LAYER = -6.0f;

	private TileTerrainType[,] worldTerrainType;
	private TileTerrainIDs[,] worldTerrainID;
	private GameObject[,] worldTerrainRepresentation;
	private RiceTerrainTile[,] riceTerrains;
	private Dictionary<TileTerrainIDs, GameObject> resources;
    private Dictionary<int, List<Pair<uint, uint>>> chunkTilesPos;
	private Dictionary<uintPair, Vegetation> vegetation;
	public CanalManager canalManager;
    public delegate void ChunkChanged();
    public delegate void ChunkChangedWithID(int chunkID);
    private event ChunkChanged m_chunkChangedListener;
    private event ChunkChangedWithID m_chunkAddedListener, m_chunkRemovedListener;
    private int chunkNextID;
    
    private WeedFactory _weedFactory;
    public WeedFactory WeedFactory
    {
        get
        {
            return _weedFactory;
        }
        private set { }
    }

    public void initWorld() {
        RiceTerrainTile.initResources(); // gestiona los recursos de forma eficiente
        initResources(); // gestiona los recursos de forma eficiente
        initWorldData();
        initWorldRepresentation();
    }

	// prepara el terreno y crea un tile donde plantar arroz
	public void createRiceChunk(uint row, uint col, bool flooded) {
		int chunkID = chunkNextID++;
		Debug.Log ("Building Chunk #"+chunkID);
		chunkTilesPos[chunkID] = new List<Pair<uint, uint>> ();

		for (uint i = row - RICE_CHUNK_SEPARATION; i < row + RICE_CHUNK_H + RICE_CHUNK_SEPARATION; ++i) {
			for (uint j = col - RICE_CHUNK_SEPARATION; j < col + RICE_CHUNK_W + RICE_CHUNK_SEPARATION; ++j) {

				if (i < row || i >= row + RICE_CHUNK_H ||
				    j < col || j >= col + RICE_CHUNK_W) {
					if (worldTerrainType [i, j] != TileTerrainType.CANAL) {
						worldTerrainType [i, j] = TileTerrainType.SEPARATOR;
					}
				} else {
                    chunkTilesPos[chunkID].Add(new Pair<uint, uint>(i, j));
                    worldTerrainType [i, j] = TileTerrainType.RICE_TERRAIN;
					riceTerrains [i, j] = new RiceTerrainTile (i, j, flooded);
					riceTerrains [i, j].setChunkNumber (chunkID);
                    riceTerrains[i, j].setLocalID(chunkTilesPos[chunkID].Count - 1);


                    Vegetation veg = getTileVegetation (i, j);
					if (veg != null)
						veg.delete ();

					if (canalManager.tileHasCanal (i, j))
						canalManager.deleteCanal (i, j);
				}

				updateTileRepresentation (i, j);
			}
		}
		canalManager.addRiceChunk (chunkID, row, col, RICE_CHUNK_W, RICE_CHUNK_H);
		canalManager.updateCanals ();
        m_chunkChangedListener();
        m_chunkAddedListener(chunkID);
		_weedFactory.addChunkID (chunkID);
		PenalizationManager.GetInstance ().checkActionsTillCurrentPhase ();

		TypeFase phase = GameObject.FindGameObjectWithTag ("Logic").GetComponent<PhaseManager> ().getCurrentPhase ();
		if (phase != TypeFase.PREWORK_I && phase != TypeFase.NONE) {
			disableChunk (chunkID);
		}
    }

    public void deleteRiceChunk(uint row, uint col) {
		int ID = riceTerrains [row, col].getChunkNumber ();
		//Pair<uint, uint> posIni = chunkTilesPos [ID] [0];
		for (uint i = row; i < row + RICE_CHUNK_H; ++i) {
			for (uint j = col; j < col + RICE_CHUNK_W; ++j) {
				worldTerrainType [i, j] = TileTerrainType.FREE;
				riceTerrains [i, j].delete();
				riceTerrains [i, j] = null;
				updateTileRepresentation (i, j);
			}
		}
		for (uint i = row - RICE_CHUNK_SEPARATION; i < row + RICE_CHUNK_H + RICE_CHUNK_SEPARATION; ++i) {
			for (uint j = col - RICE_CHUNK_SEPARATION; j < col + RICE_CHUNK_W + RICE_CHUNK_SEPARATION; ++j) {
				if (i < row || i >= row + RICE_CHUNK_H ||
				    j < col || j >= col + RICE_CHUNK_W) {
					if (worldTerrainType [i, j] == TileTerrainType.SEPARATOR && !tileCloseTo (i, j, TileTerrainType.RICE_TERRAIN)) {
						worldTerrainType [i, j] = TileTerrainType.FREE;
						updateTileRepresentation (i, j);
					}
				}
			}
		}
		chunkTilesPos.Remove (ID);

		canalManager.removeRiceChunk (row, col, RICE_CHUNK_W, RICE_CHUNK_H);
		canalManager.updateCanals ();
        m_chunkChangedListener();
        m_chunkRemovedListener(ID);

		_weedFactory.removeChunkID (ID);
    }

	// prepara el terreno para colocar el edificio (no llamar directamente ya no construye el edificio,
	//   usar la funcion del logic manager)
	public void createBuilding(uint row, uint col, uint height, uint width) {
		for (int i = 0; i < height; ++i) {
			for (int j = 0; j < width; ++j) {
				worldTerrainType [row+i, col+j] = TileTerrainType.BUILDING;
				updateTileRepresentation ((uint) (row + i), (uint) (col + j));
				//if (worldTerrainRepresentation [row+i, col+j] != null)
				//	GameObject.Destroy (worldTerrainRepresentation [row+i, col+j]);
			}
		}
		clearVegetation (row, col, width, height);
	}

    public void destroyBuilding(uint row, uint col, uint height, uint width)
    {
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                worldTerrainType[row + i, col + j] = worldTerrainType[i, j] = TileTerrainType.FREE;
                updateTileRepresentation((uint)(row + i), (uint)(col + j));
            }
        }
    }


	public bool isEmpty(uint row, uint col, uint height, uint width) {
		for (int i = 0; i < height; ++i) {
			for (int j = 0; j < width; ++j) {
				TileTerrainType terrainType = worldTerrainType [row + i, col + j];
				if (terrainType != TileTerrainType.FREE)
					return false;
			}
		}
		return true;
	}

	public bool isEmptyForRiceChunk(uint row, uint col) {
		for (uint i = row - RICE_CHUNK_SEPARATION; i < row + RICE_CHUNK_H + RICE_CHUNK_SEPARATION; ++i) {
			for (uint j = col - RICE_CHUNK_SEPARATION; j < col + RICE_CHUNK_W + RICE_CHUNK_SEPARATION; ++j) {
				TileTerrainType terrainType = worldTerrainType [row + i, col + j];
				if (i < row || i >= row + RICE_CHUNK_H ||
				    j < col || j >= col + RICE_CHUNK_W) {
					if (!(terrainType == TileTerrainType.FREE || terrainType == TileTerrainType.SEPARATOR || terrainType == TileTerrainType.CANAL))
						return false;
				} else {
					if (!(terrainType == TileTerrainType.SEPARATOR || terrainType == TileTerrainType.CANAL))
						return false;
				}
			}
		}
		return true;
	}

	public bool riceChunkHasWater(int id) {
		return canalManager.riceChunkHasWater (id);
	}

	public bool tileContainsVegetation(uint i, uint j) {
		return vegetation.ContainsKey (new uintPair (i, j));
	}

	public Vegetation getTileVegetation(uint i, uint j) {
		if (!tileContainsVegetation(i, j)) return null;
		return vegetation [new uintPair (i, j)];
	}

	public void clearVegetation(uint i, uint j, uint w, uint h) {
		for (uint ii = i; ii < i + h; ++ii) {
			for (uint jj = j; jj < j + w; ++jj) {
				Vegetation veg = getTileVegetation (ii, jj);
				if (veg != null)
					veg.delete ();
			}
		}
	}

	// Dada una posicion de la matriz, devuelve las coordenadas del juego
	public float getTileWorldPositionX(uint i, uint j)
	{
		//float cameraSize = Camera.main.orthographicSize;
		return TILE_WIDTH_UNITS * 0.5f * j  + TILE_WIDTH_UNITS * 0.5f * i;
	}

	// Dada una posicion de la matriz, devuelve las coordenadas del juego
	public float getTileWorldPositionY(uint i, uint j)
	{
		//float cameraSize = Camera.main.orthographicSize;
		return TILE_HEIGHT_UNITS * 0.5f * j - TILE_HEIGHT_UNITS * 0.5f * i;
	}

	// Dada una posicion de la matriz, devuelve las coordenadas del juego
	public Vector3 getTileWorldPosition(uint i, uint j)
	{
		float x = getTileWorldPositionX (i, j);
		float y = getTileWorldPositionY (i, j);
		return new Vector3 (x, y, 0);//++++z
	}

	public Pair<uint, uint> getMatrixPos(float x, float y) {
		uint row = getMatrixRow (x, y);
		uint col = getMatrixCol (x, y);
		return new Pair<uint, uint> (row, col);
	}

	// Dada una coordenada del juego, devuelve la posicion en la matriz del terreno
	public uint getMatrixRow(float x, float y) { // esta formula sale de coger papel y boli y las funciones de getTileWorldPosition
		x += TILE_WIDTH_UNITS / 2;
		//y += TILE_HEIGHT_UNITS / 2;
		//Debug.Log ("CLICK On: " + x + " , " + y);
		return (uint) (x / TILE_WIDTH_UNITS - y / TILE_HEIGHT_UNITS);
	}

	// Dada una coordenada del juego, devuelve la posicion en la matriz del terreno
	public uint getMatrixCol(float x, float y) { // esta formula sale de coger papel y boli y las funciones de getTileWorldPosition
		x += TILE_WIDTH_UNITS / 2;
		//y += TILE_HEIGHT_UNITS / 2;
		//Debug.Log ("CLICK On: " + x + " , " + y);
		return (uint) (x / TILE_WIDTH_UNITS + y / TILE_HEIGHT_UNITS);
	}

	public uint getNumTilesX() {
		return numTilesX;
	}

	public uint getNumTilesY() {
		return numTilesY;
	}

	public TileTerrainType getTypeTerrainOfTile(uint i, uint j) {
		if (!tilePosInside((int) i, (int) j)) return 0;
		return worldTerrainType [i, j];
	}

	public RiceTerrainTile getRiceTerrain(uint i, uint j) {
		Debug.Assert (riceTerrains [i, j] != null);
		return riceTerrains [i, j];
	}

	public List<Pair<uint, uint>> getChunkTilesPositions(int id) {
		return chunkTilesPos [id];
	}

	public bool tileCloseTo(uint i, uint j, TileTerrainType type) {
		for (int dir_i = -1; dir_i <= 1; ++dir_i) {
			for (int dir_j = -1; dir_j <= 1; ++dir_j) {
				int tile_i = (int) i + dir_i;
				int tile_j = (int) j + dir_j;
				if (!tilePosInside (tile_i, tile_j))
					continue;
				
				if (getTypeTerrainOfTile ((uint) tile_i, (uint) tile_j) == type) {
					return true;
				}
			}
		}
		return false;
	}

	public void selectChunk(int chunk) 
	{
		for (int i = 0; i < chunkTilesPos [chunk].Count; ++i) {
			RiceTerrainTile tile = getRiceTerrain (chunkTilesPos[chunk][i].First, chunkTilesPos[chunk][i].Second);
			tile.select ();
		}
	}

	public void unselectChunk(int chunk) 
	{
		for (int i = 0; i < chunkTilesPos [chunk].Count; ++i) {
			RiceTerrainTile tile = getRiceTerrain (chunkTilesPos[chunk][i].First, chunkTilesPos[chunk][i].Second);
			tile.unselect ();
		}
	}

	public void selectTile(uint i, uint j) {
		TileTerrainType tileType = getTypeTerrainOfTile (i, j);
		if (tileType == TileTerrainType.RICE_TERRAIN) {
			RiceTerrainTile tile = getRiceTerrain (i, j);
			selectChunk (tile.getChunkNumber ());
			//tile.select ();
		}
		else if (tileType == TileTerrainType.BUILDING) {
			BuildingsManager.GetInstance().selectBuildingAt(i, j);
		}
	}
	
	public void unselectTile(uint i, uint j) {
		TileTerrainType tileType = getTypeTerrainOfTile (i, j);
		if (tileType == TileTerrainType.RICE_TERRAIN) {
			RiceTerrainTile tile = getRiceTerrain (i, j);
			unselectChunk (tile.getChunkNumber ());
			//tile.unselect ();
		}
		else if (tileType == TileTerrainType.BUILDING) {
			BuildingsManager.GetInstance().unselectBuildingAt(i, j);
		}
	}

    public void resetEndOfTheYear()
    {
        //private Dictionary<int, List<Pair<uint, uint>>> chunkTilesPos;
        foreach (List<Pair<uint, uint>> tilesPos in chunkTilesPos.Values) {
            foreach (Pair<uint, uint> tilePos in tilesPos) {
                //riceTerrains[tilePos.First, tilePos.Second].enable();
                riceTerrains[tilePos.First, tilePos.Second].reset();
            }
        }
    }

	public bool HasActionBeenDoneOnAllTheChunks(int id)
	{
		foreach (List<Pair<uint, uint>> tilesPos in chunkTilesPos.Values) {
			if (!riceTerrains [tilesPos[0].First, tilesPos[0].Second].isActionDone (id)) {
				return false;
			}
		}
		return true;
	}

	public bool HasActionBeenDoneInSomeChunk(int id)
	{
		foreach (List<Pair<uint, uint>> tilesPos in chunkTilesPos.Values) {
			if (riceTerrains [tilesPos[0].First, tilesPos[0].Second].isActionDone (id)) {
				return true;
			}
		}
		return false;
	}

	public bool todosLosChunksInundados()
	{
		foreach (List<Pair<uint, uint>> tilesPos in chunkTilesPos.Values) {
			if (riceTerrains [tilesPos[0].First, tilesPos[0].Second].getRiceTerrainState() != RiceTerrainState.WATER) {
				return false;
			}
		}
		return true;
	}

    public void newDayCallback()
	{		
		_weedFactory.produceWeeds();
        CheckWeedPenalizationPoints();
		CheckPlaguePenalizationPoints ();
    }

	public bool tilePosInside(int i, int j) {
		if (i < 0) return false;
		if (j < 0) return false;
		if (i >= numTilesY) return false;
		if (j >= numTilesX) return false;
		return true;
	}

	private void initResources() {
		// faltan resources del RiceTerrain +++++++++++++
		resources = new Dictionary<TileTerrainIDs, GameObject> ();
		foreach (TileTerrainIDs terrainID in Enum.GetValues(typeof(TileTerrainIDs))) {
			GameObject terrainGameObject = null;
			//String spritePath = "Textures/terrainTiles/WorldTerrain_"+terrainID.ToString();
			String spritePath = "Textures/terrainTiles/WorldTerrain_"+terrainID.ToString();
			try {
				//terrainGameObject = Resources.Load<GameObject>("WorldTerrain_"+tileType.ToString());
				terrainGameObject = Resources.Load<GameObject> (spritePath);
			} catch (Exception e) {
				Debug.LogError (e.ToString ());
				Debug.LogError ("buildingGameObject exception for " + "asdf!");
				Debug.LogError ("path to building object " + "asdf!");
			}
			Debug.Assert (terrainGameObject != null, "Terrain not loaded: " + spritePath);
			//Debug.Log (terrainGameObject.GetComponent<SpriteRenderer> ().bounds.size);
			resources.Add(terrainID, terrainGameObject);
		}
	}

	private void initWorldData() {
		TextAsset file = Resources.Load("TiledMap/map.json", typeof(TextAsset)) as TextAsset;
		Debug.Assert (file != null);

		JSONNode node = JSON.Parse(file.text);

		numTilesX = (uint) node ["width"].AsInt;
		numTilesY = (uint) node ["height"].AsInt;
		Debug.Log ("MapW: "+numTilesX);
		Debug.Log ("MapH: "+numTilesY);


		Debug.Log ("Map Sice: " + numTilesX + " W, " + numTilesY + " H");
		worldTerrainType = new TileTerrainType[numTilesY, numTilesX];
		worldTerrainID = new TileTerrainIDs[numTilesY, numTilesX];
		worldTerrainRepresentation = new GameObject[numTilesY, numTilesX];
		riceTerrains = new RiceTerrainTile[numTilesY, numTilesX];
		chunkTilesPos = new Dictionary<int, List<Pair<uint, uint>>>();
		vegetation = new Dictionary<uintPair, Vegetation> ();
		canalManager = new CanalManager(numTilesX, numTilesY);
		_weedFactory = new WeedFactory (this, RICE_CHUNK_W * RICE_CHUNK_H);

		chunkNextID = 0;

		// TERRAIN
		for (uint k = 0; k < numTilesX * numTilesY; ++k) {
			uint i = k / numTilesX;
			uint j = k % numTilesX;
			//Debug.Log("Data["+i+","+j+"]="+node ["layers"] [0] ["data"] [k]);
			TileTerrainIDs v = (TileTerrainIDs) node ["layers"] [0] ["data"] [(int)k].AsInt;
			worldTerrainID [i, j] = v;
			worldTerrainType [i, j] = (v == TileTerrainIDs.HERBS) ? TileTerrainType.FREE : TileTerrainType.DECORATION;
		}


		// VEGETATION
		for (uint k = 0; k < numTilesX * numTilesY; ++k) {
			uint i = k / numTilesX;
			uint j = k % numTilesX;
			//Debug.Log("Data["+i+","+j+"]="+node ["layers"] [0] ["data"] [k]);
			int v = node ["layers"] [1] ["data"] [(int)k].AsInt;
			if (v != 0) {
				uintPair matrixPos = new uintPair(i, j);
				vegetation [matrixPos] = new Vegetation(i, j, (VegID) v);
			}
		}

		// CANALS
		for (uint k = 0; k < numTilesX * numTilesY; ++k) {
			uint i = k / numTilesX;
			uint j = k % numTilesX;
			//Debug.Log("Data["+i+","+j+"]="+node ["layers"] [0] ["data"] [k]);
			int v = node ["layers"] [2] ["data"] [(int)k].AsInt;
			if ((TileOthersIDs) v == TileOthersIDs.CANAL) {
				addCanal (i, j);
			}
			else if ((TileOthersIDs) v == TileOthersIDs.WATER_SOURCE) {
				addCanal (i, j);
				canalManager.addWaterSource (3, 6);
			}
		}
		canalManager.updateCanals ();

		//RICE CHUNKS
		for (uint k = 0; k < numTilesX * numTilesY; ++k) {
			uint i = k / numTilesX;
			uint j = k % numTilesX;
			//Debug.Log("Data["+i+","+j+"]="+node ["layers"] [0] ["data"] [k]);
			int v = node ["layers"] [2] ["data"] [(int)k].AsInt;
			if ((TileOthersIDs) v == TileOthersIDs.RICE_CHUNK) {
				createRiceChunk (i, j, true);
			}
		}

		canalManager.updateCanals ();
	}

	public void deleteVegetation(uint i, uint j) {
		Debug.Assert (tileContainsVegetation (i, j));

		uintPair matrixPos = new uintPair(i, j);
		vegetation [matrixPos].delete();
		vegetation.Remove (matrixPos);
	}

	private void addCanal(uint i, uint j, bool update=true) {
		worldTerrainType [i, j] = TileTerrainType.CANAL;
		canalManager.addCanal (i, j, update);
		updateTileRepresentation (i, j);
	}

	public void deleteCanal(uint i, uint j) {
		Debug.Assert (getTypeTerrainOfTile (i, j) == TileTerrainType.CANAL);
		worldTerrainType [i, j] = tileCloseTo(i, j, TileTerrainType.RICE_TERRAIN) ? TileTerrainType.SEPARATOR : TileTerrainType.FREE;
		canalManager.deleteCanal (i, j);
		updateTileRepresentation (i, j);
	}

	public void addRmvCanalTmp(uint i, uint j) {
		if (canalManager.tileHasCanalTmp (i, j)) {
			canalManager.removeCanalTmp (i, j);
		} else {
			canalManager.addCanalTmp (i, j);
		}
	}

	public void confirmCanalsTmp() {
		List<CanalTile> canals = canalManager.getCanalsTmp ();
		foreach (CanalTile canal in canals) {
			worldTerrainType [canal.getRow (), canal.getCol ()] = TileTerrainType.CANAL;
			updateTileRepresentation(canal.getRow (), canal.getCol ());
			//addCanal (canal.getRow (), canal.getCol ());
		}
		canalManager.confirmCanalsTmp ();
	}

	public void cancelCanalsTmp() {
		canalManager.cancelCanalsTmp ();
	}

	public void canalMarkToDelete(uint i, uint j) {
		Debug.Assert (getTypeTerrainOfTile (i, j) == TileTerrainType.CANAL);
		canalManager.markToDeleteCanal (i, j);
	}

	public void canalUnmarkToDelete(uint i, uint j) {
		Debug.Assert (getTypeTerrainOfTile (i, j) == TileTerrainType.CANAL);
		canalManager.unmarkToDeleteCanal (i, j);
	}

	public void riceChunkMarkToDelete(uint i, uint j) {
		int ID = riceTerrains [i, j].getChunkNumber ();
		foreach (Pair<uint, uint> tilePos in chunkTilesPos[ID]) {
			riceTerrains [tilePos.First, tilePos.Second].markToDelete ();
		}
	}

	public void riceChunkUnmarkToDelete(uint i, uint j) {
		int ID = riceTerrains [i, j].getChunkNumber ();
		foreach (Pair<uint, uint> tilePos in chunkTilesPos[ID]) {
			riceTerrains [tilePos.First, tilePos.Second].unmarkToDelete ();
		}
	}

	private void initWorldRepresentation() {
		for (uint i = 0; i < numTilesY; ++i) {
			for (uint j = 0; j < numTilesX; ++j) {
				updateTileRepresentation (i, j);
			}
		}
	}

	private void updateTileRepresentation(uint i, uint j) {
		if (worldTerrainRepresentation[i,j] != null)
			GameObject.Destroy (worldTerrainRepresentation[i,j]);
		
		TileTerrainType tileType = worldTerrainType[i,j];
		if (tileType == TileTerrainType.FREE || tileType == TileTerrainType.DECORATION ||
		    tileType == TileTerrainType.BUILDING) {
			float worldPositionX = getTileWorldPositionX (i, j);
			float worldPositionY = getTileWorldPositionY (i, j);
			Vector3 worldPosition = new Vector3 (worldPositionX, worldPositionY, TERRAIN_Z_LAYER);
			//Debug.Assert(resources.ContainsKey(tileType));
			worldTerrainRepresentation [i, j] = (GameObject) GameObject.Instantiate (resources [worldTerrainID [i, j]], worldPosition, Quaternion.identity);
		} else if (tileType == TileTerrainType.SEPARATOR) {
			float worldPositionX = getTileWorldPositionX (i, j);
			float worldPositionY = getTileWorldPositionY (i, j);
			Vector3 worldPosition = new Vector3 (worldPositionX, worldPositionY, TERRAIN_Z_LAYER);
			worldTerrainRepresentation [i, j] = (GameObject) GameObject.Instantiate (resources [TileTerrainIDs.SEPARATOR], worldPosition, Quaternion.identity);
		}
	}

	public void performActionInChunk(int id, int chunk, List<int> actionPartners, bool addAsDone)
	{
		for (int i = 0; i < chunkTilesPos [chunk].Count; ++i) {
			uint row = chunkTilesPos [chunk] [i].First;
			uint col = chunkTilesPos [chunk] [i].Second;
			riceTerrains [row, col].performAction (id, actionPartners, addAsDone);
		}
	}

	public void performActionInTileOfAChunk(int id, int chunk, int tile, List<int> actionPartners, bool addAsDone)
	{		

		uint row = chunkTilesPos [chunk] [tile-1].First;
		uint col = chunkTilesPos [chunk] [tile-1].Second;
		riceTerrains [row, col].performAction (id, actionPartners, addAsDone);
	}

    public void disableChunk(int chunk)
    {
        for (int i = 0; i < chunkTilesPos[chunk].Count; ++i) {
            uint row = chunkTilesPos[chunk][i].First;
            uint col = chunkTilesPos[chunk][i].Second;
            riceTerrains[row, col].disable();
            //riceTerrains[row, col].reset();
        }
    }

    public bool areAllChunksDisabled()
    {
		try {
	        //for (int j = 0; j < chunkTilesPos.Count; ++j) {
			var keys = chunkTilesPos.Keys;
			foreach(int j in keys) {
	            uint row = chunkTilesPos[j][0].First;
	            uint col = chunkTilesPos[j][0].Second;
				if (!riceTerrains [row, col].isDisabled ())
					return false;
	        }
		}
		catch(Exception e) {
			Debug.Log (e.Message);
			return true;
		}

        return true;
    }

	public int getNumOfChunksDisabled()
	{
		int n = 0;

		foreach (int chunkID in chunkTilesPos.Keys) {
			uint row = chunkTilesPos[chunkID][0].First;
			uint col = chunkTilesPos[chunkID][0].Second;
			if (riceTerrains [row, col].isDisabled ())
				++n;
		}

		return n;
	}

    public void enableChunk(int chunk)
    {
        for (int i = 0; i < chunkTilesPos[chunk].Count; ++i) {
            uint row = chunkTilesPos[chunk][i].First;
            uint col = chunkTilesPos[chunk][i].Second;
            riceTerrains[row, col].enable();
        }
    }

    public int getTotalTilesInAChunk()
	{
		return RICE_CHUNK_H * RICE_CHUNK_W;
	}

	public GameObject getResource(TileTerrainIDs id) {
		return resources [id];
	}

	public void startActionAnimationInAChunk(String id, int duration, int chunk, float remainingDistance=-1.0f)
	{
		ChunkAnimation animation = new ChunkAnimation(id, duration);

		uint row = chunkTilesPos [chunk] [1].First;
		uint col = chunkTilesPos [chunk] [1].Second;
		float worldPositionX = getTileWorldPositionX(row, col);
		float worldPositionY = getTileWorldPositionY(row, col);

		uint rowEnd = chunkTilesPos [chunk] [7].First;
		uint colEnd = chunkTilesPos [chunk] [7].Second;
		float worldPositionXEnd = getTileWorldPositionX(rowEnd, colEnd);
		float worldPositionYEnd = getTileWorldPositionY(rowEnd, colEnd);


		Vector3 worldPosition = new Vector3(worldPositionX, worldPositionY, ANIMATION_Z_LAYER);
		Vector3 worldPositionEnd = new Vector3(worldPositionXEnd, worldPositionYEnd, ANIMATION_Z_LAYER);
		Vector3 dir = worldPositionEnd - worldPosition;
		float distance = dir.magnitude;
		dir.Normalize ();
		animation.setPosition(worldPosition);
		animation.setDirection (dir);
		animation.calculateSpeed (distance);
		animation.setRemainingDistance ( (remainingDistance < 0) ? distance : remainingDistance );
		AnimationManager.GetInstance().addChunkAnimation(chunk, animation);
	}

	public float getTileWidth()
	{
		return worldTerrainRepresentation[0, 0].GetComponent<Renderer>().bounds.size.x;
	}

	public float getTileHeight()
	{
		return worldTerrainRepresentation[0, 0].GetComponent<Renderer>().bounds.size.y;
	}

	public void updateWeedStateInTile(int i, int j, TypeWeed weed) 
	{
		uint row = chunkTilesPos [i] [j].First;
		uint col = chunkTilesPos [i] [j].Second;

		riceTerrains [row, col].changeWeedState (weed);
	}

    public void updatePlagueStateInTile(int i, int j, TypePlague plague)
    {
        uint row = chunkTilesPos[i][j].First;
        uint col = chunkTilesPos[i][j].Second;

        riceTerrains[row, col].changePlagueState(plague);
    }

    public TypeWeed getTypeWeedOnTile(int i, int j) 
	{
		uint row = chunkTilesPos [i] [j].First;
		uint col = chunkTilesPos [i] [j].Second;

		return riceTerrains [row, col].getWeedType ();
	}

	public void growWeedOnTile(int i, int j) 
	{
		uint row = chunkTilesPos [i] [j].First;
		uint col = chunkTilesPos [i] [j].Second;

		riceTerrains [row, col].growWeed ();
	}

	public bool hasChunkWeed(int chunk) 
	{
		bool hasChunkWeed = false;
		int i = 0;
		while (!hasChunkWeed && i < chunkTilesPos [chunk].Count) {
			uint row = chunkTilesPos [chunk] [i].First;
			uint col = chunkTilesPos [chunk] [i].Second;
			hasChunkWeed = riceTerrains [row, col].hasTileWeed();
			++i;
		}

		return hasChunkWeed;
	}

	public bool hasChunkWildRice(int chunk) 
	{
		bool hasChunkWildRice = false;
		int i = 0;
		while (!hasChunkWildRice && i < chunkTilesPos [chunk].Count) {
			uint row = chunkTilesPos [chunk] [i].First;
			uint col = chunkTilesPos [chunk] [i].Second;
			hasChunkWildRice = riceTerrains [row, col].hasTileWildRice();
			++i;
		}

		return hasChunkWildRice;
	}

    public bool hasChunkPlague(int chunk)
    {
        bool hasPlague = false;
        int i = 0;
        while (!hasPlague && i < chunkTilesPos[chunk].Count)
        {
            uint row = chunkTilesPos[chunk][i].First;
            uint col = chunkTilesPos[chunk][i].Second;
            hasPlague = riceTerrains[row, col].hasTilePlague();
            i++;
        }
        return hasPlague;
    }

    public bool isChunkPlanted(int chunk)
    {
        uint row = chunkTilesPos[chunk][0].First;
        uint col = chunkTilesPos[chunk][0].Second;

        return riceTerrains[row, col].isPlanted();
    }

	public bool hasChunkAppliedHerbicide(int chunk) 
	{
		uint row = chunkTilesPos [chunk] [0].First;
		uint col = chunkTilesPos [chunk] [0].Second;

		return riceTerrains [row, col].hasAppliedHerbicide();
	}

	public bool hasChunkAppliedFangueig(int chunk) 
	{
		uint row = chunkTilesPos [chunk] [0].First;
		uint col = chunkTilesPos [chunk] [0].Second;

		return riceTerrains [row, col].hasAppliedFangueig();
	}

	public void actionFinishedInChunk(int chunk, int id)
	{
		uint row = chunkTilesPos [chunk] [0].First;
		uint col = chunkTilesPos [chunk] [0].Second;

		riceTerrains [row, col].actionFinished(id);
	}

    public int getNumberOfChunks()
    {
        return chunkTilesPos.Count;
    }

	public uint getRiceChunkProduction(int chunk)
	{
		uint production = 0;
		foreach(Pair<uint, uint> pos in chunkTilesPos[chunk]) {
			production += riceTerrains [pos.First, pos.Second].getRiceProduced();
		}
		return production;
	}

    public void addListenerToChunkChange(ChunkChanged fun)
    {
        m_chunkChangedListener += fun;
    }

    public void addListenerToChunkAdded(ChunkChangedWithID fun)
    {
        m_chunkAddedListener += fun;
    }

    public void addListenerToChunkRemoved(ChunkChangedWithID fun)
    {
        m_chunkRemovedListener += fun;
    }

    public List<int> getActionsDoneInAChunk(int chunk)
    {
        uint row = chunkTilesPos[chunk][0].First;
        uint col = chunkTilesPos[chunk][0].Second;

        return riceTerrains[row, col].getActionsDone();
    }

    public void CheckWeedPenalizationPoints()
    {
        int daysUntilPoint = PenalizationManager.DAYS_UNTIL_PENALIZATION_POINT;
		foreach (int chunkID in chunkTilesPos.Keys) {
            int penalizationPoints = 0;
			for (int i = 0; i < chunkTilesPos[chunkID].Count; ++i) {
				uint row = chunkTilesPos[chunkID][i].First;
				uint col = chunkTilesPos[chunkID][i].Second;
                int penalizationDay = riceTerrains[row, col].getPenalizationDay();
                if(penalizationDay >= daysUntilPoint) {
                    ++penalizationPoints;
                    riceTerrains[row, col].resetPenalizationDay();
                }
            }
			PenalizationManager.GetInstance().addPenalizationPoints(chunkID, penalizationPoints);
        }
    }

	public void CheckPlaguePenalizationPoints()
	{
		int daysUntilPoint = PenalizationManager.DAYS_UNTIL_PENALIZATION_POINT;
		foreach (int chunkID in chunkTilesPos.Keys) {
			int penalizationPoints = 0;
			for (int i = 0; i < chunkTilesPos[chunkID].Count; ++i) {
				uint row = chunkTilesPos[chunkID][i].First;
				uint col = chunkTilesPos[chunkID][i].Second;
				if (riceTerrains [row, col].hasTilePlague ()) {
					riceTerrains [row, col].Plague.PenalizationDays += 1;
					int penalizationDay = riceTerrains [row, col].Plague.PenalizationDays;
					if(penalizationDay >= daysUntilPoint) {
						++penalizationPoints;
						riceTerrains [row, col].Plague.PenalizationDays = 0;
					}
				}
			}
			PenalizationManager.GetInstance().addPenalizationPoints(chunkID, penalizationPoints);
		}
	}

    public void removeWeed(int chunkID, int localTileID)
    {
        _weedFactory.removeWeed(chunkID, localTileID);
    }

    public void save(WorldTerrainData worldTerrainData)
    {
        worldTerrainData.ChunkTilesPos = chunkTilesPos;
        worldTerrainData.NumTilesX = numTilesX;
        worldTerrainData.NumTilesY = numTilesY;
        worldTerrainData.RiceTerrains = new RiceTerrainTileData[numTilesY, numTilesX];
        for (uint i = 0; i < numTilesY; ++i) {
            for (uint j = 0; j < numTilesX; ++j) {
                if (riceTerrains[i, j] != null) {
                    worldTerrainData.RiceTerrains[i, j] = new RiceTerrainTileData();
                    worldTerrainData.RiceTerrains[i, j].ActionsDoneInTheYear = riceTerrains[i, j].getActionsDone();
                    worldTerrainData.RiceTerrains[i, j].ChunkID = riceTerrains[i, j].getChunkNumber();
                    worldTerrainData.RiceTerrains[i, j].Col = riceTerrains[i, j].getCol();
                    worldTerrainData.RiceTerrains[i, j].DaysBurning = riceTerrains[i, j].DaysBurning;
                    worldTerrainData.RiceTerrains[i, j].DaysPlanted = riceTerrains[i, j].DaysPlanted;
                    worldTerrainData.RiceTerrains[i, j].HasAlreadyBeenWorked = riceTerrains[i, j].HasAlreadyBeenWorked;
                    worldTerrainData.RiceTerrains[i, j].HasAppliedFangueig = riceTerrains[i, j].HasAppliedFangueig;
                    worldTerrainData.RiceTerrains[i, j].HasAppliedHerbicide = riceTerrains[i, j].HasAppliedHerbicide;
                    worldTerrainData.RiceTerrains[i, j].IsBurningRostoll = riceTerrains[i, j].IsBurningRostoll;
                    worldTerrainData.RiceTerrains[i, j].LocalTileID = riceTerrains[i, j].LocalTileID;
                    worldTerrainData.RiceTerrains[i, j].PlagueData = new PlageData(riceTerrains[i, j].Plague.type, riceTerrains[i, j].Plague.active);
                    worldTerrainData.RiceTerrains[i, j].RiceState = riceTerrains[i, j].RiceState;
                    worldTerrainData.RiceTerrains[i, j].Row = riceTerrains[i, j].getRow();
                    worldTerrainData.RiceTerrains[i, j].TerrainState = riceTerrains[i, j].TerrainState;
                    worldTerrainData.RiceTerrains[i, j].WeedData = new WeedData(riceTerrains[i, j].Weed.type, riceTerrains[i, j].Weed.day, riceTerrains[i, j].Weed.PenalizationDay);
                }
                else {
                    worldTerrainData.RiceTerrains[i, j] = null;
                }
            }
        }
        worldTerrainData.Vegetation = new Dictionary<uintPair, VegID>();
        foreach (KeyValuePair<uintPair, Vegetation> veggie in vegetation) {
            worldTerrainData.Vegetation.Add(veggie.Key, veggie.Value.CurrentVegetation);
        }
        worldTerrainData.WorldTerrainID = worldTerrainID;
        worldTerrainData.WorldTerrainType = worldTerrainType;
        worldTerrainData.ChunkNextID = chunkNextID;
    }

    public void load(WorldTerrainData worldTerrainData)
    {
        RiceTerrainTile.initResources(); // gestiona los recursos de forma eficiente
        initResources(); // gestiona los recursos de forma eficiente

        //initWorldData();
        numTilesX = worldTerrainData.NumTilesX;
        numTilesY = worldTerrainData.NumTilesY;
        worldTerrainType = worldTerrainData.WorldTerrainType;
        worldTerrainID = worldTerrainData.WorldTerrainID;
        worldTerrainRepresentation = new GameObject[numTilesY, numTilesX];
        riceTerrains = new RiceTerrainTile[numTilesY, numTilesX]; //construir
        for (uint i = 0; i < numTilesY; ++i) {
            for (uint j = 0; j < numTilesX; ++j) {
                if (worldTerrainData.RiceTerrains[i, j] != null) {
                    riceTerrains[i, j] = new RiceTerrainTile();
                    riceTerrains[i, j].setActionsDone(worldTerrainData.RiceTerrains[i, j].ActionsDoneInTheYear);
                    riceTerrains[i, j].setChunkNumber(worldTerrainData.RiceTerrains[i, j].ChunkID);
                    riceTerrains[i, j].setCol(worldTerrainData.RiceTerrains[i, j].Col);
                    riceTerrains[i, j].DaysBurning = worldTerrainData.RiceTerrains[i, j].DaysBurning;
                    riceTerrains[i, j].DaysPlanted = worldTerrainData.RiceTerrains[i, j].DaysPlanted;
                    riceTerrains[i, j].HasAlreadyBeenWorked = worldTerrainData.RiceTerrains[i, j].HasAlreadyBeenWorked;
                    riceTerrains[i, j].HasAppliedFangueig = worldTerrainData.RiceTerrains[i, j].HasAppliedFangueig;
                    riceTerrains[i, j].HasAppliedHerbicide = worldTerrainData.RiceTerrains[i, j].HasAppliedHerbicide;
                    riceTerrains[i, j].IsBurningRostoll = worldTerrainData.RiceTerrains[i, j].IsBurningRostoll;
                    riceTerrains[i, j].LocalTileID = worldTerrainData.RiceTerrains[i, j].LocalTileID;
                    if(worldTerrainData.RiceTerrains[i, j].PlagueData.Active) {
                        riceTerrains[i, j].changePlagueState(worldTerrainData.RiceTerrains[i, j].PlagueData.Type);
                    }
                    riceTerrains[i, j].RiceState = worldTerrainData.RiceTerrains[i, j].RiceState;
                    riceTerrains[i, j].setRow(worldTerrainData.RiceTerrains[i, j].Row);
                    riceTerrains[i, j].TerrainState = worldTerrainData.RiceTerrains[i, j].TerrainState;
                    if (worldTerrainData.RiceTerrains[i, j].WeedData.Type != TypeWeed.NONE) {
                        riceTerrains[i, j].changeWeedState(worldTerrainData.RiceTerrains[i, j].WeedData.Type, worldTerrainData.RiceTerrains[i, j].WeedData.Day,
                            worldTerrainData.RiceTerrains[i, j].WeedData.PenalizationDay);
                    }
                    riceTerrains[i, j].updateTerrainRepresentation();
					riceTerrains [i, j].InstantiateImages ();
                }
                else {
                    worldTerrainData.RiceTerrains[i, j] = null;
                }
            }
        }
        chunkTilesPos = worldTerrainData.ChunkTilesPos;
        vegetation = new Dictionary<uintPair, Vegetation>(); //construir
        foreach (KeyValuePair<uintPair, VegID> veggie in worldTerrainData.Vegetation) {
            vegetation.Add(veggie.Key, new Vegetation(veggie.Key.First, veggie.Key.Second, veggie.Value));
        }
        canalManager = new CanalManager();
        _weedFactory = new WeedFactory(this, RICE_CHUNK_W * RICE_CHUNK_H);
        chunkNextID = worldTerrainData.ChunkNextID;

        initWorldRepresentation();
    }
}