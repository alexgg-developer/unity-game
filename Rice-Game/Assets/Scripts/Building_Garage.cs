using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Garage : Building
{

	public Building_Garage(JSONNode data, BUILDINGS type): base(data, type)
	{

	}

	public override void upgrade()
    {
		base.upgrade ();
	}

    //specific camp 1 -> number of busy machines
    //specific camp 2 -> number total of machines
    //specific camp 3 -> current capacity
    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        List<int> garageNeededObject = RiceObjectsManager.GetInstance().GarageNeededObjects;
        int totalQuantityUsed = 0;
        int totalQuantity = 0;

        foreach(int id in garageNeededObject)
        {
            RiceObject riceObject = RiceObjectsManager.GetInstance().getRiceObject(id);
            totalQuantity += riceObject.quantity;
            totalQuantityUsed += riceObject.quantityUsed;
        }
        m_specificBuildingInfo.Clear(); 
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("MACHINERY_BUSY"), totalQuantityUsed.ToString()));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("MACHINERY_TOTAL"), totalQuantity.ToString()));
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