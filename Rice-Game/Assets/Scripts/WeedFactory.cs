using System.Collections;
using System.Collections.Generic;
using MyUtils.Pair;
using UnityEngine;
using System;

public enum TypeWeed { WILD_RICE_PHASE_1, WILD_RICE_PHASE_2, WILD_RICE_PHASE_3, CIPERACEA, NONE };

public class Weed
{
    public TypeWeed type;
    public GameObject instance;
    public int day;
    public int PenalizationDay;
    private RiceTerrainTile _parent;

    public Weed(RiceTerrainTile theParent)
    {
        type = TypeWeed.NONE;
        instance = null;
        day = 1;
        PenalizationDay = 0;
        _parent = theParent;
    }

    public void grow()
    {
        ++day;
        ++PenalizationDay;
        if (type == TypeWeed.WILD_RICE_PHASE_1 && day > 5) {
            _parent.changeWeedState(TypeWeed.WILD_RICE_PHASE_2);
        }
        else if (type == TypeWeed.WILD_RICE_PHASE_2 && day > 10) {
            _parent.changeWeedState(TypeWeed.WILD_RICE_PHASE_3);
        }
    }

    public void resetPenalizations()
    {
        PenalizationDay = 0;
    }

    public void delete()
    {
        type = TypeWeed.NONE;
        if (instance != null) {
            GameObject.Destroy(instance);
            instance = null;
        }
        day = 1;
        PenalizationDay = 0;
    }
}

public class WeedFactory
{
    public const float WEED_PROBABILITY_CHUNK = 0.005f;
    //public const float WEED_PROBABILITY_CHUNK = 1f;
    public const float WEED_PROBABILITY_EXPANSION = 0.001f;
    //public const float WEED_PROBABILITY_EXPANSION = 1.0f;
    public const float WEED_HERBICIDE_MODIFIER = 0.25f;
    public const float WEED_FANGUEJAR_MODIFIER = 0.75f;
    private const int TYPES_OF_WEED = 2;
    private Dictionary<int, HashSet<uint>> _currentWeed;
    private WorldTerrain _parent;
    private uint _tilesPerChunk;
    private Dictionary<TypeWeed, GameObject> _resources;

    public WeedFactory(WorldTerrain parent, uint tilesPerChunk)
    {
        _parent = parent;
        _currentWeed = new Dictionary<int, HashSet<uint>>();

        _tilesPerChunk = tilesPerChunk;

    }

    //not in use ¿?
    private void initResources()
    {
        _resources = new Dictionary<TypeWeed, GameObject>();

        foreach (TypeWeed weedID in Enum.GetValues(typeof(TypeWeed))) {
            if (weedID != TypeWeed.NONE) {
                GameObject weedGameObject = null;
                String spritePath = "Textures/terrainTiles/" + weedID.ToString().ToLower();
                try {
                    weedGameObject = Resources.Load<GameObject>(spritePath);
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
                Debug.Assert(weedGameObject != null, "Terrain not loaded: " + spritePath);
                _resources.Add(weedID, weedGameObject);
            }
        }
    }

    public void addChunkID(int id)
    {
        _currentWeed[id] = new HashSet<uint>();
    }

    public void removeChunkID(int id)
    {
        _currentWeed.Remove(id);
    }

    public void produceWeeds()
    {
        //return;
        foreach (int i in _currentWeed.Keys) {

            // --- PRODUCE NEW WEED ---- //
            if (ActionManager.GetInstance().isActionInProgressInAChunk(i)) {
                continue;
            }

            //--GROW WEED --//
            foreach (int tile in _currentWeed[i]) {
                _parent.growWeedOnTile(i, tile);
            }

            if (_currentWeed[i].Count < _tilesPerChunk) {
                float randomNumber = UnityEngine.Random.value;
                float probability = WEED_PROBABILITY_CHUNK;
                if (_parent.hasChunkAppliedHerbicide(i)) {
                    probability *= WEED_HERBICIDE_MODIFIER;
                }
                if (_parent.hasChunkAppliedFangueig(i)) {
                    probability *= WEED_FANGUEJAR_MODIFIER;
                }
				probability *= (1 - ((float)InvestigationManager.GetInstance().getBadWeedDecreaseBonus() / 100.0f));
                if (randomNumber <= probability) {
                    uint randomTile = (uint)((UnityEngine.Random.value) * (float)(_tilesPerChunk));
                    while (_currentWeed[i].Contains(randomTile)) {
                        randomTile = (uint)((UnityEngine.Random.value) * (float)(_tilesPerChunk));
                        foreach (uint tile in _currentWeed[i]) {
                            Debug.Log("tiles::" + tile);
                        }
                    }
                    int randomWeed = (int)((UnityEngine.Random.value) * (float)(TYPES_OF_WEED - 1));
                    TypeWeed weed = TypeWeed.NONE;
                    switch (randomWeed) {
                        case 0:
                            weed = TypeWeed.WILD_RICE_PHASE_1;
                            break;
                        case 1:
                            weed = TypeWeed.CIPERACEA;
                            break;
                    }
                    _parent.updateWeedStateInTile(i, (int)randomTile, weed);
                    _currentWeed[i].Add(randomTile);
                }
            }
            // --- EXPAND WEED ---- //
            if (_currentWeed[i].Count < _tilesPerChunk) {
                float randomNumber = UnityEngine.Random.value;
                float probability = WEED_PROBABILITY_EXPANSION;
                if (_parent.hasChunkAppliedHerbicide(i)) {
                    probability *= WEED_HERBICIDE_MODIFIER;
                }
                if (_parent.hasChunkAppliedFangueig(i)) {
                    probability *= WEED_FANGUEJAR_MODIFIER;
                }
				probability *= (1 - ((float)InvestigationManager.GetInstance().getBadWeedDecreaseBonus() / 100.0f));
                if (randomNumber <= probability) {
                    uint randomTile = (uint)((UnityEngine.Random.value) * (float)(_tilesPerChunk));
                    while (_currentWeed[i].Contains(randomTile)) {
                        randomTile = (uint)((UnityEngine.Random.value) * (float)(_tilesPerChunk));
                    }
                    int randomWeed = (int)((UnityEngine.Random.value) * (float)(TYPES_OF_WEED - 1));
                    TypeWeed weed = TypeWeed.NONE;
                    switch (randomWeed) {
                        case 0:
                            weed = TypeWeed.WILD_RICE_PHASE_1;
                            break;
                        case 1:
                            weed = TypeWeed.CIPERACEA;
                            break;
                    }
                    _parent.updateWeedStateInTile(i, (int)randomTile, weed);
                    _currentWeed[i].Add(randomTile);
                }
            }
        }
    }

    public void save(WeedFactoryData weedFactoryData)
    {
        //weedFactoryData.CurrentWeed.Clear();
        foreach (KeyValuePair<int, HashSet<uint>> weed in _currentWeed) {
            weedFactoryData.CurrentWeed.Add(weed.Key, new List<uint>());
            foreach (uint value in weed.Value) {
                weedFactoryData.CurrentWeed[weed.Key].Add(value);
            }
        }
    }

    public void load(WeedFactoryData weedFactoryData)
    {
        foreach (KeyValuePair<int, List<uint>> weed in weedFactoryData.CurrentWeed) {
            _currentWeed.Add(weed.Key, new HashSet<uint>());
            foreach (uint value in weed.Value) {
                _currentWeed[weed.Key].Add(value);
            }
        }
    }

    public void removeWeed(int chunkID, int localTileID)
    {
        _currentWeed[chunkID].Remove((uint)localTileID);
    }
}

