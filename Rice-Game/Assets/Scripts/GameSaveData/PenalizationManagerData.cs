using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class PenalizationManagerData
{
    public Dictionary<int, int>  ChunkPenalizationPoints = new Dictionary<int, int>();
    public Dictionary<int, int>  ChunkPenalization = new Dictionary<int, int>();
    public Dictionary<int, bool> ChunkDisabled = new Dictionary<int, bool>();
}