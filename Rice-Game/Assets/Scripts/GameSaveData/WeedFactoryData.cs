using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class WeedFactoryData
{
    public Dictionary<int, List<uint>> CurrentWeed;

    public WeedFactoryData()
    {
        CurrentWeed = new Dictionary<int, List<uint>>();
    }
}