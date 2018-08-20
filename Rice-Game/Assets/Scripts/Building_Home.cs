using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Home: Building
{

	public Building_Home(JSONNode data, BUILDINGS type): base(data, type)
	{

	}

	public override void upgrade()
    {
		base.upgrade ();
	}

    //specific camp 1 -> number of busy workers
    //specific camp 2 -> number total of workers
    //specific camp 3 -> current capacity
    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        m_specificBuildingInfo.Clear();
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("BUSY_WORKERS"), WorkerManager.GetInstance().BusyWorkers.ToString()));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("WORKERS_TOTAL"), WorkerManager.GetInstance().Workers.ToString()));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("CURRENT_MAX_CAPACITY"), this.getCapacity1().ToString()));

        return m_specificBuildingInfo;
    }

    public void openHirePanel()
    {
        GameObject hirePanel = Resources.Load("Prefabs/WorkerRecruitmentlLostFocusLayer") as GameObject;
        GameObject.Instantiate(hirePanel);
    }
}