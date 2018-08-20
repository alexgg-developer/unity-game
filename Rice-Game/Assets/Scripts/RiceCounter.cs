using UnityEngine;
using System;

public class RiceCounter
{
    private uint _totalRiceProduced;
    private uint _anualRiceProduced;
	private uint _anualRiceLost;

	public delegate void RiceProductionChanged();
	private event RiceProductionChanged m_productionChangedListener;

	public RiceCounter()
	{
        _totalRiceProduced = 0;
		_anualRiceProduced = 0;
		_anualRiceLost = 0;
        //CoopManager.GetInstance().checkProductionMilestone((float)_totalRiceProduced / 1000.0f);
    }

	public void addRiceProduced(uint newRiceProduced)
	{
		_totalRiceProduced += newRiceProduced;
		_anualRiceProduced += newRiceProduced;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::addRiceProduced");
		Debug.Log ("_totalRiceProduced = " + _totalRiceProduced);
		Debug.Log ("_anualRiceProduced = " + _anualRiceProduced);
	}

	// RICE LOST
    public void addRiceLost(uint riceLost)
    {
		_anualRiceLost += riceLost;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::addRiceLost");
		Debug.Log ("_anualRiceLost = " + _anualRiceLost);
    }

    public uint getRiceLost()
    {
        return _anualRiceLost;
    }

    public void setRiceLost(uint riceLostThisYear)
    {
		_anualRiceLost = riceLostThisYear;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::setRiceLost");
		Debug.Log ("_anualRiceLost = " + _anualRiceLost);
    }

	// RICE PRODUCED THIS YEAR
    public uint getAnualRiceProduced() { return _anualRiceProduced; }

    public void setAnualRiceProduced(uint anualRiceProduced) {
		_anualRiceProduced = anualRiceProduced;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::setAnualRiceProduced");
		Debug.Log ("_anualRiceProduced = " + _anualRiceProduced);
	}

	// RICE PRODUCED TOTAL
	public uint getTotalRiceProduced() {return _totalRiceProduced;}

	public void setTotalRiceProduced(uint newRice) {
		_totalRiceProduced = newRice;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::setTotalRiceProduced");
		Debug.Log ("_totalRiceProduced = " + _totalRiceProduced);
	}

	// RESET YEAR
    public void resetRiceProducedThisYear()
    {
        _anualRiceProduced = 0;
		_anualRiceLost = 0;
		m_productionChangedListener ();
		Debug.Log ("RiceCounter::resetRiceProducedThisYear");
		Debug.Log ("_anualRiceProduced = " + _anualRiceProduced);
		Debug.Log ("_riceLostThisYear = " + _anualRiceLost);
    }

	public void addListenerToRiceProductionChanged(RiceProductionChanged fun)
	{
		m_productionChangedListener += fun;
	}
}

