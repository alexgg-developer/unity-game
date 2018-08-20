using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class CoopManagerData
{
    //in order: production, ecology, expansion, anualproduction
    public List<int> MilestoneLevels = new List<int>();
	public uint LastYearProduction;
	public int LastYearEcologyScore;
}