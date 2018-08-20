using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MyUtils.Pair;

public class Milestone<BonusType, CheckType>
{
    List<BonusType> m_bonus;
    List<CheckType> m_check;
    int m_level;
    const int MAX_LEVEL = 5;

    public int Level
    {
        get
        {
            return m_level;
        }
        set
        {
            m_level = value;
        }
    }

    public Milestone(List<BonusType> bonus, List<CheckType> check)
    {
        m_bonus = bonus;
        m_check = check;
		m_level = 0;
    }

    public BonusType getCurrentBonus()
    {
		int level = InvestigationManager.GetInstance().isInvestigated(INVESTIGATIONS_ID.COOP) ? m_level : 0;
        return m_bonus[level];
    }

    public BonusType getNextBonus()
    {
        return m_bonus[Math.Min(m_level + 1, MAX_LEVEL)];
    }

    public CheckType getNextCheck()
    {
        return m_check[m_level];
    }

    public void checkValue(CheckType value)
    {
		m_level = 0;
		while (m_level < MAX_LEVEL && Comparer<CheckType>.Default.Compare(m_check[m_level], value) <= 0) {
            ++m_level;
        }        
    }

	public bool isInMaxLevel()
	{
		return m_level == MAX_LEVEL;
	}
}

public class CoopManager
{
    #region members
	private static CoopManager instance = null;
	private Milestone<int, float> m_totalProduction;
	private Milestone<int, int> m_ecology, m_expansion;
    private Milestone<Pair<int, int>, float> m_anualProduction;
    //private int m_ecologyScore;
    private int m_ecologyPlageControlPoint;
    private int m_ecologyWeedControlPoint;
    //private int m_ecologyStubbleManagementPoint;
    private int m_ecologyFertilizerPoint;

	private uint m_lastYearProduction;
	private int m_lastYearEcologyScore;

    #endregion

    public static CoopManager GetInstance()
	{
		if(instance == null)
		{
			instance = new CoopManager();
		}
		
		return instance;
	}
	
	private CoopManager()
    {
        //check per ton of rice
        //Milestone -> bonus / check
		m_totalProduction = new Milestone<int, float>(new List<int>() { 0, 10, 15, 20, 23, 25 }, new List<float>() { 5.0f, 20.0f, 80.0f, 320.0f, 1280.0f });
        m_ecology    = new Milestone<int, int>(new List<int>() { 0, 1000, 1500, 2000, 2500, 3000 }, new List<int>() { 1, 2, 3, 4, 5 });
		m_expansion  = new Milestone<int, int>(new List<int>() { 0, 10, 15, 20, 25, 30 }, new List<int>() { 5, 10, 15, 20, 25 });
		//m_expansion  = new Milestone<int, int>(new List<int>() { 0, 10, 15, 20, 25, 30 }, new List<int>() { 1, 2, 3, 4, 6 }); //TEST

        List<Pair<int, int>> m_anualProductionBonus = new List<Pair<int, int>>();
        m_anualProductionBonus.Add(new Pair<int, int>(0, 0));
        m_anualProductionBonus.Add(new Pair<int, int>(10, 10));
        m_anualProductionBonus.Add(new Pair<int, int>(20, 20));
        m_anualProductionBonus.Add(new Pair<int, int>(30, 30));
        m_anualProductionBonus.Add(new Pair<int, int>(40, 40));
        m_anualProductionBonus.Add(new Pair<int, int>(50, 50));
		m_anualProduction = new Milestone<Pair<int, int>, float>(m_anualProductionBonus, new List<float>() { 0.5f, 1.0f, 1.5f, 2.0f, 2.5f });
		//m_anualProduction = new Milestone<Pair<int, int>, float>(m_anualProductionBonus, new List<float>() { 0.05f, 0.1f, 0.15f, 0.20f, 0.25f }); //TEST

		m_ecologyPlageControlPoint = 1;
		m_ecologyWeedControlPoint = 1;
		//m_ecologyStubbleManagementPoint = 1;
		m_ecologyFertilizerPoint = 1;

		m_lastYearProduction = 0;
		m_lastYearEcologyScore = 0;

        WorldTerrain.GetInstance().addListenerToChunkChange(OnChunkChange);
		UserDataManager.GetInstance ().rice.addListenerToRiceProductionChanged (OnRiceProductionChange);
    }

	// == GET BONUSES ==

    public int getCurrentTotalProductionBonus()
    {
        return m_totalProduction.getCurrentBonus();
    }

    public int getCurrentEcologyBonus()
    {
        return m_ecology.getCurrentBonus();
    }

    public Pair<int, int> getCurrentAnualProductionBonus()
    {
        return m_anualProduction.getCurrentBonus();
    }

    public int getCurrentExpansionBonus()
    {
        return m_expansion.getCurrentBonus();
    }

	// == CHECK MILESTONES ==

	private void checkTotalProductionMilestone()
    {
		m_totalProduction.checkValue( (float) UserDataManager.GetInstance ().getTotalRiceProduced () / 1000.0f);
    }
    
	private void checkEcologyMilestone()
    {
		int ecologyScore = getEcologyScore ();
		m_ecology.checkValue(ecologyScore);
		m_lastYearEcologyScore = ecologyScore;
		m_ecologyPlageControlPoint = 1;
		m_ecologyWeedControlPoint = 1;
		//m_ecologyStubbleManagementPoint = 1;
		m_ecologyFertilizerPoint = 1;
    }

	private void checkExpansionMilestone(int chunks)
    {
        m_expansion.checkValue(chunks);
    }

	private void checkAnualProductionMilestone()
	{
		uint riceProducedThisYear = UserDataManager.GetInstance().getRiceProducedThisYear();
		//int anualProductionLevelOld = m_anualProduction.Level;
		m_anualProduction.checkValue((float)riceProducedThisYear / 1000.0f);
		//int anualProductionLevelNew = m_anualProduction.Level;
		//m_anualProduction.Level = anualProductionLevelNew >= anualProductionLevelOld ? anualProductionLevelNew : anualProductionLevelOld;
		m_lastYearProduction = riceProducedThisYear;
	}

    public void happyNewYear()
    {
        checkEcologyMilestone();
		//checkTotalProductionMilestone ();
		checkAnualProductionMilestone ();

		if (InvestigationManager.GetInstance ().isInvestigated (INVESTIGATIONS_ID.ECO_SUBVENCION)) {
			int grant = getCurrentEcologyBonus ();
			UserDataManager.GetInstance ().addGold ((uint)grant);
		}
    }

    public void loseEcologyPlageControlPoint()
    {
        m_ecologyPlageControlPoint = 0;
    }

    public void loseEcologyWeedControlPoint()
    {
        m_ecologyWeedControlPoint = 0;
    }

	/*
    public void loseEcologyStubbleManagemenPoint()
    {
        m_ecologyStubbleManagementPoint = 0;
    }
    */

    public void loseEcologyFertilizerPoint()
    {
        m_ecologyFertilizerPoint = 0;
    }

    public int getCurrentEcologyLevel()
    {
        return m_ecology.Level;
    }

    public int getCurrentTotalProductionLevel()
    {
        return m_totalProduction.Level;
    }

    public int getCurrentAnualProductionLevel()
    {
        return m_anualProduction.Level;
    }

    public int getCurrentExpansionLevel()
    {
        return m_expansion.Level;
    }

    public int getNextEcologyBonus()
    {
        return m_ecology.getNextBonus();
    }

    public int getNextTotalProductionBonus()
    {
        return m_totalProduction.getNextBonus();
    }

    public Pair<int,int> getNextAnualProductionBonus()
    {
        return m_anualProduction.getNextBonus();
    }

    public int getNextExpansionBonus()
    {
        return m_expansion.getNextBonus();
    }

    public float getNextTotalProductionGoal()
    {
        return m_totalProduction.getNextCheck();
    }

    public int getNextExpansionGoal()
    {
        return m_expansion.getNextCheck();
    }

    public float getNextAnualProductionGoal()
    {
        return m_anualProduction.getNextCheck();
    }

	// MAX LVLS?
	public bool expansionBonusIsInMaxLvl()
	{
		return m_expansion.isInMaxLevel ();
	}

	public bool ecologyBonusIsInMaxLvl()
	{
		return m_ecology.isInMaxLevel ();
	}

	public bool totalProductionBonusIsInMaxLvl()
	{
		return m_totalProduction.isInMaxLevel ();
	}

	public bool anualProductionBonusIsInMaxLvl()
	{
		return m_anualProduction.isInMaxLevel ();
	} 
    
	//
	public uint getLastYearProduction()
	{
		return m_lastYearProduction;
	}

	public int getLastYearEcologyScore()
	{
		return m_lastYearEcologyScore;
	}

    private int getEcologyScore()
	{
		int investigationPoint = InvestigationManager.GetInstance().isInvestigated(INVESTIGATIONS_ID.ECOLOGY) ? 1 : 0;
		int plagueControlPoint = m_ecologyPlageControlPoint;
		int weedControlPoint = m_ecologyWeedControlPoint;
		//ecologyScore += m_ecologyStubbleManagementPoint;
		int fertilizerPoint = m_ecologyFertilizerPoint;
		const int INUNDAR_CAMPOS_ACTION_ID = 2;
		int inundarPoint = WorldTerrain.GetInstance ().todosLosChunksInundados() ? 1 : 0;

		int ecologyScore = investigationPoint + plagueControlPoint + weedControlPoint + fertilizerPoint + inundarPoint;

		Debug.Log ("EcologiScore=" + ecologyScore);
		Debug.Log ("..investigationPoint=" + investigationPoint);
		Debug.Log ("..plagueControlPoint=" + plagueControlPoint);
		Debug.Log ("..weedControlPoint=" + weedControlPoint);
		Debug.Log ("..fertilizerPoint=" + fertilizerPoint);
		Debug.Log ("..inundarPoint=" + inundarPoint);
		return ecologyScore;
    }

	public void OnRiceProductionChange()
	{
		checkTotalProductionMilestone ();
	}

    public void OnChunkChange()
    {
        int numberChunks = WorldTerrain.GetInstance().getNumberOfChunks();
        checkExpansionMilestone(numberChunks);
    }
    
    //in order: production, ecology, expansion, anualproduction
    public void save(CoopManagerData coopManagerData)
    {
        coopManagerData.MilestoneLevels.Add(m_totalProduction.Level);
        coopManagerData.MilestoneLevels.Add(m_ecology.Level);
        coopManagerData.MilestoneLevels.Add(m_expansion.Level);
        coopManagerData.MilestoneLevels.Add(m_anualProduction.Level);

		coopManagerData.LastYearProduction = m_lastYearProduction;
		coopManagerData.LastYearEcologyScore = m_lastYearEcologyScore;
    }

    public void load(CoopManagerData coopManagerData)
    {
        m_totalProduction.Level = coopManagerData.MilestoneLevels[0];
        m_ecology.Level = coopManagerData.MilestoneLevels[1];
        m_expansion.Level = coopManagerData.MilestoneLevels[2];
        m_anualProduction.Level = coopManagerData.MilestoneLevels[3];

		m_lastYearProduction = coopManagerData.LastYearProduction;
		m_lastYearEcologyScore = coopManagerData.LastYearEcologyScore;
    }
}
