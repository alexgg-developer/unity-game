using UnityEngine;

public delegate void OnGoldInRedHandler(bool inTheRed);
public delegate void PanelHandler();

public class GoldReserve
{
	private float _gold;
	private float _riceGoldAddThisYear;

    public GoldReserve()
	{
		_gold = 0;
		_riceGoldAddThisYear = 0;
	}

	public GoldReserve(int initialGold)
	{
		_gold = initialGold;
		_riceGoldAddThisYear = 0;
    }

	public int getGold()
	{
		return (int) _gold;
	}

	public void setGold(int newGold)
	{
		_gold = newGold;
	}

	public int getRiceGoldAddThisYear()
	{
		return (int) _riceGoldAddThisYear;
	}

	public void setRiceGoldAddThisYear(int newGold)
	{
		_riceGoldAddThisYear = newGold;
	}

	public void resetRiceGoldAddThisYear()
	{
		setRiceGoldAddThisYear(0);
	}

	public void addGold(float gold)
	{
		_gold += gold;
	}

	public void addRiceGold(float gold)
	{
		addGold (gold);
		_riceGoldAddThisYear += gold;
    }

	public void espendGold(float gold)
    {
        _gold -= gold;
    }

	public bool inRed()
	{
		return _gold < 0;
	}
}
