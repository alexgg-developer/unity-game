using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class BuildingData
{
    public BUILDINGS ID;
    //position
    public uint i, j;
    public int Level;
    //they represent the state of the rice in the building, -1 for unused
    public int SpecialSlot1, SpecialSlot2, SpecialSlot3; 
}

[System.Serializable]
public class BuildingManagerData
{
    public List<BuildingData> BuildingsBuilded;

    public BuildingManagerData()
    {
        BuildingsBuilded = new List<BuildingData>();
    }
}