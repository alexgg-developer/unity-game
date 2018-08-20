using UnityEngine;
using System.Collections;
using System;

public class UserDataManager
{
	private static UserDataManager instance = null;
	
	
	public static UserDataManager GetInstance()
	{
		if(instance == null)
		{
			instance = new UserDataManager();
		}
		
		return instance;
	}

	private UserDataManager() {
		gold = new GoldReserve (0);
		rice = new RiceCounter ();
	}
	
	public GoldReserve gold;
	public RiceCounter rice;


	public bool init()
	{
		
		return true;
	}

	public void addGold(float _gold)
	{
		gold.addGold(_gold);
	}

	public void addRiceProduced(uint _rice)
	{
		rice.addRiceProduced(_rice);
	}

    public uint getRiceProducedThisYear()
    {
        return rice.getAnualRiceProduced();
    }

    public void resetYearlyData()
    {
        rice.resetRiceProducedThisYear();
		gold.resetRiceGoldAddThisYear ();
    }
    
    public uint getTotalRiceProduced()
    {
		return rice.getTotalRiceProduced();
    }

    public void sellRice(uint riceToSell)
    {
        //rice.addRiceSold(riceToSell);
		float coopBonus = CoopManager.GetInstance().getCurrentTotalProductionBonus() / 100.0f;
		float invBonus = InvestigationManager.GetInstance().getGoldBonusPerRiceSold() / 100.0f;
		gold.addRiceGold(riceToSell * (LogicManager.PRICE_PER_RICE + LogicManager.PRICE_PER_RICE*coopBonus + LogicManager.PRICE_PER_RICE*invBonus));
    }

    public void addRiceLost(uint riceLost)
    {
        rice.addRiceLost(riceLost);
    }

    public uint getRiceLostThisYear()
    {
        return rice.getRiceLost();
    }

    public void load(UserDataManagerData userDataManagerData)
    {
		rice.setRiceLost(userDataManagerData.AnualRiceLost);
		rice.setAnualRiceProduced(userDataManagerData.AnualRiceProduced);
		rice.setTotalRiceProduced(userDataManagerData.TotalRiceProduced);
        gold.setGold(userDataManagerData.Gold);
		gold.setRiceGoldAddThisYear (userDataManagerData.GoldAddThisYear);
    }

    public void save(UserDataManagerData userDataManagerData)
	{
		userDataManagerData.AnualRiceLost = rice.getRiceLost();
        userDataManagerData.AnualRiceProduced = rice.getAnualRiceProduced();
		userDataManagerData.TotalRiceProduced = rice.getTotalRiceProduced();
		userDataManagerData.Gold = gold.getGold();
		userDataManagerData.GoldAddThisYear = gold.getRiceGoldAddThisYear();
    }
}