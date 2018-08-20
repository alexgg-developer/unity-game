using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PenalizationManager
{
	private static PenalizationManager instance = null;
    public static PenalizationManager GetInstance()
	{
		if(instance == null)
		{
			instance = new PenalizationManager();
		}
		
		return instance;
	}

    private Dictionary<int, int> m_chunkPenalizationPoints = new Dictionary<int, int>();
    private Dictionary<int, int> m_chunkPenalization = new Dictionary<int, int>();
    private Dictionary<int, bool> m_chunkDisabled = new Dictionary<int, bool>();
    private const float PERCENTAGE_LOST_PER_POINT = 2.5f; //percentage lost of the rice of a chunk per penalization point
    public const int DAYS_UNTIL_PENALIZATION_POINT = 3; //days until a penalization point is added, used in plagues and weeds
    private const int POINTS_UNTIL_PENALIZATION = 15;
    private PhaseManager m_phaseManager;

	private PenalizationManager()
    {
        WorldTerrain.GetInstance().addListenerToChunkAdded(addChunk);
        WorldTerrain.GetInstance().addListenerToChunkRemoved(removeChunk);
        m_phaseManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>();
        m_phaseManager.addListenerToPhaseChange(checkActionsLastPhase);
        TimeManager timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
        timeManager.addListenerToYearChange(reset);
    }

    public void reset()
    {
        List<int> keys = new List<int>(m_chunkPenalization.Keys);
        foreach (int key in keys) {
            m_chunkPenalization[key] = 0;
            m_chunkPenalizationPoints[key] = 0;
            m_chunkDisabled[key] = false;
            //WorldTerrain.GetInstance().enableChunk(key);
        }
        //GameObject.Find("Logic").GetComponent<TimeManager>().changeMode(TimeManager.SPEED_MODE.NORMAL);
    }

    public float getPercentageForChunk(int chunkID)
    {
        return m_chunkPenalization[chunkID] * PERCENTAGE_LOST_PER_POINT;
    }	

	public bool init()
	{	
		return true;
	}	

    public void addChunk(int chunkID)
    {
        m_chunkPenalization.Add(chunkID, 0);
        m_chunkPenalizationPoints.Add(chunkID, 0);
        m_chunkDisabled.Add(chunkID, false);
    }

    public void removeChunk(int chunkID)
    {
        if(m_chunkPenalization.ContainsKey(chunkID)) {
            m_chunkPenalization.Remove(chunkID);
            m_chunkPenalizationPoints.Remove(chunkID);
            m_chunkDisabled.Remove(chunkID);
        }
    }

    public void addPenalization(int chunkID)
    {
        if (m_chunkPenalization.ContainsKey(chunkID)) {
            m_chunkPenalization[chunkID]++;
        }
    }

    public void disableChunk(int chunkID)
    {
        if (m_chunkDisabled.ContainsKey(chunkID)) {
            ActionManager.GetInstance().stopActionChunk(chunkID);
            AnimationManager.GetInstance().removeChunkAnimation(chunkID);
            m_chunkDisabled[chunkID] = true;
            WorldTerrain.GetInstance().disableChunk(chunkID);
            
        }
    }

    public bool isChunkDisabled(int chunkID)
    {
        if (m_chunkDisabled.ContainsKey(chunkID)) {
            return m_chunkDisabled[chunkID];
        }
        return false;
    }
    
	public void checkActionsTillCurrentPhase()
	{
		List<int> actionsTillLastPhase = m_phaseManager.GetActionsTillLastUsefulPhase ();
		CheckListOfActions (actionsTillLastPhase);
	}
    public void checkActionsLastPhase()
    {
        List<int> lastPhaseActions = m_phaseManager.getActionsInLastPhase();
		CheckListOfActions (lastPhaseActions);
    }

	private void CheckListOfActions(List<int> actionsToCheck)
	{
		List<int> actionsToCheckPartners = new List<int>();
		List<int> keys = new List<int>(m_chunkPenalization.Keys);
		foreach (int chunk in keys) {
			List<int> actionsDoneInChunk = WorldTerrain.GetInstance().getActionsDoneInAChunk(chunk);
			if(ActionManager.GetInstance().isActionInProgressInAChunk(chunk)) {
				int id = ActionManager.GetInstance().getActionInProgressInAChunk(chunk);
				if(!actionsDoneInChunk.Contains(id)) {
					actionsDoneInChunk.Add(id);
				}
			}
			for (int i = 0; i < actionsToCheck.Count; ++i) {
				bool found = false;
				int j = 0;
				while (!found && j < actionsDoneInChunk.Count) {
					found = actionsDoneInChunk[j] == actionsToCheck[i];
					++j;
				}
				if (!found) {
					if (ActionManager.GetInstance().isAnActionRequired(actionsToCheck[i])) {
						disableChunk(chunk);
					}
					else if (ActionManager.GetInstance().hasAnActionPenalization(actionsToCheck[i])) {
						int k = 0;
						bool hasPartner = false;
						while(!hasPartner && k < actionsToCheckPartners.Count) {
							List<int> partners = ActionManager.GetInstance().getActionPartnersOfAnAction(actionsToCheckPartners[k]);
							hasPartner = partners.Contains(actionsToCheck[i]);
							++k;
						}
						if(!hasPartner) {
							Debug.Log ("PenalizationAdd at Chunk=" + chunk + " por Action=" + actionsToCheck [i]);
							addPenalization(chunk);
							actionsToCheckPartners.Add(actionsToCheck[i]);
						}

					}
				}
			}
			actionsToCheckPartners.Clear();
			if(WorldTerrain.GetInstance().areAllChunksDisabled()) {
				GameObject.Find("Logic").GetComponent<TimeManager>().changeMode(TimeManager.SPEED_MODE.SUPAFAST);
			}
		}
	}

    public void save(PenalizationManagerData penalizationManagerData)
    {
        penalizationManagerData.ChunkDisabled = m_chunkDisabled;
        penalizationManagerData.ChunkPenalization = m_chunkPenalization;
        penalizationManagerData.ChunkPenalizationPoints = m_chunkPenalizationPoints;
    }

    public void load(PenalizationManagerData penalizationManagerData)
    {
        m_chunkDisabled = penalizationManagerData.ChunkDisabled;
        m_chunkPenalization = penalizationManagerData.ChunkPenalization;
        m_chunkPenalizationPoints = penalizationManagerData.ChunkPenalizationPoints;
    }

    public void addPenalizationPoints(int chunk, int penalizationPoints)
    {
        m_chunkPenalizationPoints[chunk] += penalizationPoints;
        int penalizations = m_chunkPenalizationPoints[chunk] / POINTS_UNTIL_PENALIZATION;
        if (penalizations > 0) {
            m_chunkPenalizationPoints[chunk] -= (penalizations * POINTS_UNTIL_PENALIZATION);
            m_chunkPenalization[chunk] += penalizations;
        }
    }
}