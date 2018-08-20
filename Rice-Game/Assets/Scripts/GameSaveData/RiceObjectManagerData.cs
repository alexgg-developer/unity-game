using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class RiceObjectManagerData
{
    public List<RiceObjectData> Objects;
    public RiceObjectManagerData()
    {
        Objects = new List<RiceObjectData>();
    }
}

[System.Serializable]
public class RiceObjectData
{
    public int ID;
    public int Quantity;
    public int QuantityUsed;
}