using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Planta: Building
{
	enum STATE { EMPTY, HALF, FULL };
	
	uint riceUnprepared;
	//uint rate;
		
	public Building_Planta (JSONNode data, BUILDINGS type): base(data, type)
	{
		riceUnprepared = 0;
		//rate = (uint) getCapacity2 ();
    }

	public override void upgrade() {
		base.upgrade ();
		//rate = (uint) getCapacity2 ();
	}

	public void sendRice(uint riceSent) 
	{
		riceUnprepared = Math.Min (riceUnprepared + riceSent, (uint) getCapacity1 ());
	}
	
	override public void newDayCallback()
	{
		uint riceToPrepare = Math.Min(riceUnprepared, (uint) getCapacity2());
		riceUnprepared -= riceToPrepare;
        
        UserDataManager.GetInstance().sellRice(riceToPrepare);   
	}

    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        string measurementUnprepared = "kg.";
        string riceUnpreparedInfo = riceUnprepared.ToString();
        if (riceUnprepared > 1000)
        {
			double riceUnprRounded = Math.Round(riceUnprepared / 1000.0f, 2);
			riceUnpreparedInfo = string.Format("{0:0.00}", riceUnprRounded);
            measurementUnprepared = "t.";
        }

        string measurementCapacity = "kg.";
        string capacityInfo = getCapacity1().ToString();
        if (this.getCapacity1() > 1000)
        {
			double capacity = Math.Round(getCapacity1 () / 1000.0f, 2);
			capacityInfo = string.Format("{0:0.00}", capacity);
            measurementCapacity = "t.";
        }

        m_specificBuildingInfo.Clear();
		m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("LEFT_TO_PROCESS"), ""  + riceUnpreparedInfo + " " + measurementUnprepared));
		m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("MAXIMUM_CAPACITY_IS"), capacityInfo + " " + measurementCapacity));
		m_specificBuildingInfo.Add(new Pair<string, string>("", ""));

        return m_specificBuildingInfo;
    }

    public override string getWorkInfo()
    {
		return getCapacity2 () + " kg.";
    }

    public uint getRiceUnprepared() { return riceUnprepared; }

    public void setRice(uint riceUnPrepared)
    {
        this.riceUnprepared = riceUnPrepared;
    }

    public uint getCurrentFreeCapacity()
    {
        return (uint)getCapacity1() - riceUnprepared;
    }
}

