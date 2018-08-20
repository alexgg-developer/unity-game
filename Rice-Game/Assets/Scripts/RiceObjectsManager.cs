using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class RiceObject
{
    public int id;
    public string title;
    public string info;
    public string iconImgPath;
    public int quantity;
    public int quantityUsed;
	public int order;
    
    private int cost;
    public int Cost
    {
        get
        {
            return System.Math.Max(cost - (cost * CoopManager.GetInstance().getCurrentExpansionBonus()) / 100, 1);
        }
        private set
        {
            cost = value;
        }
    }
    public bool IsDurable { get; set; }
    public List<int> InvestigationRequired {get; set;}
    public bool NeedsGarage { get; set; }

	public RiceObject(int aID, string aTitle, string aInfo, string aIconImgPath, int aCost, bool isDurable, bool needsGarage, int aOrder) 
	{
		id = aID;
		title = aTitle;
		info = aInfo;
		iconImgPath = aIconImgPath;
		cost = aCost;
		quantity = 0;
		quantityUsed = 0;
        IsDurable = isDurable;
        NeedsGarage = needsGarage;
        InvestigationRequired = new List<int>();
		order = aOrder;
	}
}

public class RiceObjectsManager {
	// SINGLETON
	private static RiceObjectsManager instance = null;
	private Dictionary<int, RiceObject> _object;
    private List<int> m_garageNeededObjects = new List<int>();
    public List<int> GarageNeededObjects
    {
        get
        {
            return m_garageNeededObjects;
        }
        set
        {
            m_garageNeededObjects = value;
        }
    }

    public static RiceObjectsManager GetInstance()
	{
		if(instance == null)
		{
			instance = new RiceObjectsManager();
		}

		return instance;
	}

	private RiceObjectsManager() 
	{
		_object = JSONLoader.readObjects();
	}

	public int getTotalObjects()
	{
		return _object.Count;
	}
		
	public RiceObject getRiceObject(int id)
	{
		return _object [id];
	}

	public List<RiceObject> getRiceObjectsAvailable()
	{
        List<RiceObject> _objectsAvailable = new List<RiceObject>();
		foreach(RiceObject riceObject in _object.Values)
        {
            bool hasBeenInvestigated = InvestigationManager.GetInstance().areInvestigated(riceObject.InvestigationRequired);
            if(hasBeenInvestigated)
            {
                _objectsAvailable.Add(riceObject);
            }

        }
		_objectsAvailable.Sort((x, y) => x.order.CompareTo(y.order));
		return _objectsAvailable;
	}

    public void buyRiceObject(int id) 
	{
		++_object [id].quantity;
	}

	public bool isThereAvailable(int id)
	{
		return ((_object [id].quantity - _object [id].quantityUsed) > 0);
	}

    public int howManyFreeOfObject(int id)
    {
        return (_object[id].quantity - _object[id].quantityUsed);
    }

    public int getQuantityOfObject(int id)
    {
        return _object[id].quantity;
    }

    public string getObjectRiceTitle(int id)
	{
		return _object [id].title;
	}

	public void useObject(int id) 
	{
        if (_object[id].IsDurable) {
            _object[id].quantityUsed++;
        }
        else {
            _object[id].quantity--;
        }
	}

    public void addObjectToGarageNeeded(int id)
    {
        m_garageNeededObjects.Add(id);
    }

    public void returnObject(int id) 
	{
        if (_object[id].IsDurable) {
            _object[id].quantityUsed--;
        }
	}

    public int getTotalObjectsThatNeedGarage()
    {
        int total = 0;
        foreach(int id in m_garageNeededObjects) {
            int count = RiceObjectsManager.GetInstance().getQuantityOfObject(id);
            total += count;
        }

        return total;
    }

    public void load(RiceObjectManagerData riceObjectManagerData)
    {
        for(int i = 0; i < riceObjectManagerData.Objects.Count; ++i) {
			RiceObjectData obj = riceObjectManagerData.Objects [i];
			_object[obj.ID].quantity = obj.Quantity;
			_object[obj.ID].quantityUsed = obj.QuantityUsed;
        }
    }

    public void save(RiceObjectManagerData riceObjectManagerData)
    {
		foreach (RiceObject ro in _object.Values) {
            RiceObjectData rod = new RiceObjectData();
            rod.ID = ro.id;
            rod.Quantity = ro.quantity;
            rod.QuantityUsed = ro.quantityUsed;
            riceObjectManagerData.Objects.Add(rod);
        }
    }
}
