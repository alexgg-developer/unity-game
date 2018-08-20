using MyUtils.Pair;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class CanalManagerData
{
    public CanalTileData[,] CanalsMatrix;
    public int[,] RiceChunkMatrix;
    public uint MapWidth, MapHeight;
    public List<uintPair> WaterSources;
    public Dictionary<int, bool> RiceChunkHasWater;

    public CanalManagerData() { }

    public CanalManagerData(CanalTile[,] canalsMatrix, int[,] riceChunkMatrix, uint mapWidth, uint mapHeight, List<uintPair> waterSources)
    {
        CanalsMatrix = new CanalTileData[mapWidth, mapHeight];
        for(int i = 0; i < mapWidth; ++i) {
            for(int j = 0; j < mapHeight; ++j) {
                CanalsMatrix[i, j] = new CanalTileData(canalsMatrix[i,j].getRow(), canalsMatrix[i, j].getCol(), canalsMatrix[i, j].isConfirmed());
            }
        }
        RiceChunkMatrix = riceChunkMatrix;
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        WaterSources = waterSources;
    }
}

[System.Serializable]
public class CanalTileData
{
    public uint Row, Col;
    public bool Confirmed;
    public CanalTileData(uint row, uint col, bool confirmed)
    {
        Row = row;
        Col = col;
        Confirmed = confirmed;
    }
}