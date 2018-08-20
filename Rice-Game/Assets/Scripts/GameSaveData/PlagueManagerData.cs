using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class PlagueManagerData
{
    public List<PlagueData> PlaguesData = new List<PlagueData>();
}

[System.Serializable]
public class PlagueData
{
    public bool IsActive;
    public bool AlreadyHappened;
}