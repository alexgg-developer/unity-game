using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Stable: Building
{

	public Building_Stable(JSONNode data, BUILDINGS type): base(data, type)
	{

	}

	public override void upgrade()
    {
		base.upgrade ();
	}

    //specific camp 1 -> number of busy horses
    //specific camp 2 -> number total of horses
    //specific camp 3 -> current capacity
    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        const int HORSE_ID = 2;
        RiceObject horse = RiceObjectsManager.GetInstance().getRiceObject(HORSE_ID);
        m_specificBuildingInfo.Clear(); 
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("BUSY_HORSES"), horse.quantityUsed.ToString()));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("TOTAL_HORSES"), horse.quantity.ToString()));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("CURRENT_MAX_CAPACITY"), this.getCapacity1().ToString()));

        return m_specificBuildingInfo;
    }

    public void openBuyMenu()
    {
        GameObject parent = GameObject.Find("UICanvas");
        Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
        foreach (Component t in trs)
        {
            if (t.name.Equals("BuyMenu"))
            {
                t.gameObject.GetComponent<BuyMenu>().selectObjectTab();
                t.gameObject.SetActive(true);
            }
            else if (t.tag.Equals("BuyMenuParent"))
            {
                t.gameObject.SetActive(true);
            }
        }

    }
}