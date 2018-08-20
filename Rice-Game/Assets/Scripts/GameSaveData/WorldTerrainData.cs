using MyUtils.Pair;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class WorldTerrainData
{
    public uint NumTilesX, NumTilesY;
    public TileTerrainType[,] WorldTerrainType;
    public TileTerrainIDs[,] WorldTerrainID;
    public RiceTerrainTileData[,] RiceTerrains;
    public Dictionary<int, List<Pair<uint, uint>>> ChunkTilesPos;
    public Dictionary<uintPair, VegID> Vegetation;
    public int ChunkNextID;
}

[System.Serializable]
public class RiceTerrainTileData
{
    public uint Row, Col;
    public RiceTerrainState TerrainState;
    public RiceState RiceState;
    public uint DaysPlanted;
    public bool IsBurningRostoll;
    public uint DaysBurning;
    public int ChunkID;
    public int LocalTileID;
    //To maintain the record of actions
    public List<int> ActionsDoneInTheYear; //ids of actions done in the year
    public bool HasAlreadyBeenWorked;
    public WeedData WeedData;
    public bool HasAppliedHerbicide;
    public bool HasAppliedFangueig;
    public bool HasWaterSource;
    public PlageData PlagueData;
}

[System.Serializable]
public class PlageData
{
    public TypePlague Type;
    public bool Active;

    public PlageData(TypePlague type, bool active)
    {
        Type = type;
        Active = active;
    }
}

[System.Serializable]
public class WeedData
{
    public TypeWeed Type;
    public int Day;
    public int PenalizationDay;

    public WeedData(TypeWeed type, int day, int penalizationDay)
    {
        Type = type;
        Day = day;
        PenalizationDay = penalizationDay;
    }
}