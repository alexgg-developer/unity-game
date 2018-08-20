using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public static class JSONLoader
{

	public static List<List<int>> readPhaseActions() 
	{
		List<List<int>> phaseActions = new List<List<int>> ();

		TextAsset file = Resources.Load("json/phase_actions", typeof(TextAsset)) as TextAsset;
		
		if (file != null) { 
			JSONNode node = JSON.Parse(file.text);
			for(int j = 0; j < node["PHASES"].Count; ++j) {
				JSONNode nodeDeeper = node["PHASES"][j];
				List<int> action = new List<int>();
				for(int i = 0; i < nodeDeeper.Count; ++i) {
					action.Add (nodeDeeper[i].AsInt);
				}
				phaseActions.Add(action);
			}

		}
		else {
			Debug.Log("File json/phase_actions not loaded");
		}
		return phaseActions;
	}

	public static List<Phase> readPhaseInfo()
    {
		List<Phase> phasesInfo = new List<Phase> ();

        TextAsset file = Resources.Load("json/phases", typeof(TextAsset)) as TextAsset;

        if (file != null) {
            JSONNode node = JSON.Parse(file.text);
            for (int j = 0; j < node["PHASES"].Count; ++j) {
                Phase newPhase = new Phase();
                JSONNode nodeDeeper = node["PHASES"][j];
                newPhase.Name = Dictionary.getString(nodeDeeper["NAME"].Value);
                newPhase.Description = Dictionary.getString(nodeDeeper["DESCRIPTION"].Value);
                newPhase.ActionsInfo = new List<string>();
                for (int i = 0; i < nodeDeeper["ACTIONS_INFO"].Count; ++i) {
                    newPhase.ActionsInfo.Add(Dictionary.getString(nodeDeeper["ACTIONS_INFO"][i].Value));
                }
                phasesInfo.Add(newPhase);
            }
        }
		return phasesInfo;
    }

	public static Dictionary<int, ChunkAction> readActions() 
	{
		Dictionary<int, ChunkAction> actions = new Dictionary<int, ChunkAction> ();

		TextAsset file = Resources.Load("json/terrain_actions", typeof(TextAsset)) as TextAsset;
		
		if (file != null) { 
			JSONNode node = JSON.Parse(file.text);
			for(int j = 0; j < node["ACTIONS"].Count; ++j) {
				JSONNode nodeDeeper = node["ACTIONS"][j];
				int id = nodeDeeper ["ID"].AsInt;
				bool isRequired = nodeDeeper["IS_REQUIRED"].Value.Equals("TRUE");
				bool hasPenalization = nodeDeeper["PENALIZATION"].Value.Equals("TRUE");
				string name = Dictionary.getString(nodeDeeper["NAME"].Value);
                string desc = Dictionary.getString(nodeDeeper["DESCRIPTION"].Value);
                int duration = nodeDeeper ["DURATION"].AsInt;
				bool addAsDone = nodeDeeper ["ADD_AS_DONE"].Value.Equals("TRUE");
                bool needCanal = nodeDeeper["NEED_CANAL"].Value.Equals("TRUE");
                int workersNeeded = nodeDeeper["WORKERS_NEEDED"].AsInt;
				int priority = nodeDeeper["PRIORITY"].AsInt;
				ChunkAction chunkAction = new ChunkAction(id, isRequired, hasPenalization, name, desc, duration, addAsDone, needCanal, workersNeeded, priority); //TMP should be info and title instead of name and name
				for(int i = 0; i < nodeDeeper["DEPENDENCIES"].Count; ++i) {
					chunkAction.addDependency(nodeDeeper["DEPENDENCIES"][i]["TYPE"].Value, nodeDeeper["DEPENDENCIES"][i]["VALUE"].Value);					
				}
				for(int i = 0; i < nodeDeeper["OBJECTS_ID"].Count; ++i) {
					chunkAction.addObjectRequired(nodeDeeper["OBJECTS_ID"][i].AsInt);	
				}

				for(int i = 0; i < nodeDeeper["ANIMATION"].Count; ++i) {
					chunkAction.addAnimation(nodeDeeper["ANIMATION"][i].Value);	
				}

				for(int i = 0; i < nodeDeeper["INVESTIGATION_REQUIRED"].Count; ++i) {
					chunkAction.addInvestigationRequired(nodeDeeper["INVESTIGATION_REQUIRED"][i].AsInt);	
				}

				for(int i = 0; i < nodeDeeper["ACTION_PARTNERS"].Count; ++i) {
					chunkAction.addActionPartner(nodeDeeper["ACTION_PARTNERS"][i].AsInt);	
				}

                actions.Add(id, chunkAction);
			}
			
		}
		else {
			Debug.Log("File json/terrain_actions not loaded");
		}
		return actions;
	}

	public static Dictionary<int, RiceObject> readObjects() 
	{
		Dictionary<int, RiceObject> objectList = new Dictionary<int, RiceObject> ();

		TextAsset file = Resources.Load("json/objects", typeof(TextAsset)) as TextAsset;
        try {
            if (file != null) {
                JSONNode node = JSON.Parse(file.text);
                for (int j = 0; j < node["OBJECTS"].Count; ++j) {
                    JSONNode nodeDeeper = node["OBJECTS"][j];
                    int id = nodeDeeper["ID"].AsInt;
                    string name = Dictionary.getString(nodeDeeper["NAME"].Value);
                    string description = Dictionary.getString(nodeDeeper["DESCRIPTION"].Value);
                    string img = nodeDeeper["IMAGE"].Value;
                    int cost = nodeDeeper["COST"].AsInt;
                    bool isDurable = nodeDeeper["IS_DURABLE"].AsBool;
                    bool needsGarage = nodeDeeper["NEED_GARAGE"].AsBool;
                    if (needsGarage) {
                        //riceObjectManager.addObjectToGarageNeeded(id);
                    }
					int order = nodeDeeper["ORDER"].AsInt;
					RiceObject riceObject = new RiceObject(id, name, description, img, cost, isDurable, needsGarage, order);
                    List<int> investigationRequired = new List<int>();
                    for (int i = 0; i < nodeDeeper["INVESTIGATION_REQUIRED"].Count; ++i) {
                        investigationRequired.Add(nodeDeeper["INVESTIGATION_REQUIRED"][i].AsInt);
                    }
                    riceObject.InvestigationRequired = investigationRequired;
					objectList.Add(riceObject.id, riceObject);
                }

            }
            else {
                Debug.Log("File json/terrain_actions not loaded");
            }
        }
        catch(Exception ex) {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);
        }
		return objectList;
	}

	public static void readPlagues(List<Plague> plagueList) 
	{
		TextAsset file = Resources.Load("json/plagues", typeof(TextAsset)) as TextAsset;

		if (file != null) { 
			JSONNode node = JSON.Parse(file.text);
			for(int j = 0; j < node["PLAGUES"].Count; ++j) {
				JSONNode nodeDeeper = node["PLAGUES"][j];
				int id = nodeDeeper ["ID"].AsInt;
				string name = Dictionary.getString(nodeDeeper["NAME"].Value);
				string description = Dictionary.getString(nodeDeeper["DESCRIPTION"].Value);
				string img = nodeDeeper["IMAGE"].Value;
				int monthIn = nodeDeeper ["MONTH_IN"].AsInt;
				int monthOut = nodeDeeper ["MONTH_OUT"].AsInt;
				int baseDmg = nodeDeeper ["BASE_DMG"].AsInt;
				Plague plg = new Plague(id, name, description, img, monthIn, monthOut, baseDmg) ;
			    plagueList.Add(plg);
			}

		}
		else {
			Debug.Log("File json/plagues not loaded");
		}
	}

	public static Dictionary<int, Investigation> getReadInvestigations() 
	{
		Dictionary<int, Investigation> invList = new Dictionary<int, Investigation> ();

		TextAsset file = Resources.Load("json/investigations", typeof(TextAsset)) as TextAsset;

		if (file != null) { 
			JSONNode node = JSON.Parse(file.text);
			for(int j = 0; j < node["INVESTIGATIONS"].Count; ++j) {
				JSONNode nodeDeeper = node["INVESTIGATIONS"][j];
				int id = nodeDeeper ["ID"].AsInt;
				string name = Dictionary.getString(nodeDeeper["NAME"].Value);
				string description = Dictionary.getString(nodeDeeper["DESCRIPTION"].Value);
				string img = nodeDeeper["IMAGE"].Value;
				int year = nodeDeeper ["YEAR"].AsInt;
				int req = nodeDeeper ["REQ"].AsInt;
				int cost = nodeDeeper ["COST"].AsInt;
				Investigation inv = new Investigation(id, name, description, img, year, req, cost) ;
				invList.Add(id, inv);
			}

		}
		else {
			Debug.Log("File json/investigations not loaded");
		}

		return invList;
	}
}

