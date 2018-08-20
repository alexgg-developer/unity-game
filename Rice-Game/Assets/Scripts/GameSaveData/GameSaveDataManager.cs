using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class GameSaveDataManager
{
    private static GameSaveData m_saveData;
    private static ActionManager m_am;
    private static BuildingsManager m_bm;
    private static TimeManager m_tm;
    private static CoopManager m_cm;
    private static UserDataManager m_um;
    private static InvestigationManager m_im;
    private static LogicManager m_lm;
    private static PenalizationManager m_pm;
    private static PhaseManager m_phasem;
    private static PlagueManager m_plaguem;
    private static RankingManager m_rm;
    private static RiceObjectsManager m_rom;
    private static WorkerManager m_workerm;
    private static WorldTerrain m_worldm;
	private static TutorialManager m_tutMan;

    public static void init()
    {
        m_am = ActionManager.GetInstance();
        m_bm = BuildingsManager.GetInstance();
        m_tm = GameObject.Find("Logic").GetComponent<TimeManager>();
        m_cm = CoopManager.GetInstance();
        m_um = UserDataManager.GetInstance();
        m_im = InvestigationManager.GetInstance();
        m_lm = GameObject.Find("Logic").GetComponent<LogicManager>();
        m_pm = PenalizationManager.GetInstance();
        m_phasem = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>();
        m_plaguem = PlagueManager.GetInstance();
        m_rm = RankingManager.GetInstance();
        m_rom = RiceObjectsManager.GetInstance();
        m_workerm = WorkerManager.GetInstance();
        m_worldm = WorldTerrain.GetInstance();
		m_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
    }

    public static void Save() 
	{
        //init();
        //Debug.Log ("Savingg");
        m_saveData = new GameSaveData();

        m_am.save(m_saveData.ActionManagerData);
        m_bm.save(m_saveData.BuildingManagerData);
        m_tm.save(m_saveData.TimeManagerData);
        m_cm.save(m_saveData.CoopManagerData);
        m_um.save(m_saveData.UserDataManagerData);
        m_im.save(m_saveData.InvestigationManagerData);
        m_lm.save(m_saveData.LogicManagerData);
		m_worldm.canalManager.save(m_saveData.CanalManagerData);
        m_pm.save(m_saveData.PenalizationManagerData);
        m_phasem.save(m_saveData.PhaseManagerData);
        m_plaguem.save(m_saveData.PlagueManagerData);
        m_rm.save(m_saveData.RankingManagerData);
        m_rom.save(m_saveData.RiceObjectManagerData);
		m_worldm.WeedFactory.save(m_saveData.WeedFactoryData);
        m_workerm.save(m_saveData.WorkerManagerData);
        m_worldm.save(m_saveData.WorldTerrainData);
		m_tutMan.save(m_saveData.tutorialManagerData);


        BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file, m_saveData);
		file.Close();
		PlayerPrefs.SetInt("LoadData", 1);
	}

	public static void Load() 
	{
		if (File.Exists (Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			m_saveData = (GameSaveData)bf.Deserialize (file);

			Debug.Assert(m_saveData != null);
			try {
				init();
				m_tutMan.load(m_saveData.tutorialManagerData);
				//GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ().eventDone(TutorialManager.EVENTS.EXIT);
				m_tm.load(m_saveData.TimeManagerData);
				m_phasem.load(m_saveData.PhaseManagerData);
				m_lm.load(m_saveData.LogicManagerData);
				m_bm.load(m_saveData.BuildingManagerData);   
				m_cm.load(m_saveData.CoopManagerData);
				m_um.load(m_saveData.UserDataManagerData);
				m_im.load(m_saveData.InvestigationManagerData);
				m_pm.load(m_saveData.PenalizationManagerData); //tener en cuenta los callbacks de world terrain   
				m_plaguem.load(m_saveData.PlagueManagerData);
				m_rm.load(m_saveData.RankingManagerData);
				m_rom.load(m_saveData.RiceObjectManagerData);
				m_workerm.load(m_saveData.WorkerManagerData);
				m_worldm.load(m_saveData.WorldTerrainData);
				m_worldm.canalManager.load(m_saveData.CanalManagerData);
				m_worldm.WeedFactory.load(m_saveData.WeedFactoryData);
				m_am.load(m_saveData.ActionManagerData);
			}
			catch (Exception ex) {
				Debug.LogError(ex.Message);
				Debug.LogError(ex.StackTrace);
			}

			file.Close ();
		} else {
			Debug.Log ("ERROR Loading : file not found");
		}
	}

	public static void printData()
	{
		Debug.Log ("PRINT DATA START");

		//foreach (RiceTerrainData terrainData in savedGames) {		
			//Debug.Log ("NumTiles:::" + savedData.numTilesX);
		//}
		Debug.Log ("PRINT DATA END");
	}
}

