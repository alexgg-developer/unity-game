using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class InvestigationData
{
    public int ID;
    public bool Purchased;
    public bool Investigating;
}

[System.Serializable]
public class InvestigationManagerData
{
    public List<InvestigationData> InvestigationData = new List<InvestigationData>();
}