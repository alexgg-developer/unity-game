using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public enum INVESTIGATIONS_ID {
	PLANTA = 5,
	ECOLOGY = 9,
	COOP = 16,
	ECO_SUBVENCION = 17,
	QUANTITY = 26
};

public class Investigation {
	int id;
	string name;
	string desc;
	string imgPath;
	int year;
	int req;
	int cost;
	bool purchased;
	bool investigating;

	public Investigation(int aId, string aName, string aDesc, string img, int aYear, int aReq, int aCost){
		id = aId;
		name = aName;
		desc = aDesc;
		imgPath = img;
		year = aYear;
		req = aReq;
		cost = aCost;
		purchased = false;
		investigating = false;
	}

	public void printInfo() {
		Debug.Log ("Investigation {");
		Debug.Log ("    id="+id);
		Debug.Log ("    name="+name);
		Debug.Log ("    desc="+desc);
		Debug.Log ("    imgPath="+imgPath);
		Debug.Log ("    year="+year);
		Debug.Log ("    req="+req);
		Debug.Log ("    cost="+cost);
		Debug.Log ("}");
	}

	public int getID(){return id;}
	public string getName(){return name;}
	public string getDesc(){return desc;}
	public string getImg(){return imgPath;}
	public int getYear(){return year;}
	public int getReq(){return req;}
	public int getCost(){return cost;}
	public bool isPurchased(){return purchased;}
	public bool isInvestigating(){return investigating;}
	public void setPurchased(bool b){purchased = b;}
	public void setInvestigating(bool b){investigating = b;}

}

public class InvestigationManager {

	private static InvestigationManager instance = null;
    public static InvestigationManager GetInstance() {
		if(instance == null) {
			instance = new InvestigationManager();
		}

        return instance;
	}


	private Dictionary<int, Investigation> m_invest;

	private InvestigationManager() 
	{
		m_invest = JSONLoader.getReadInvestigations();

		TimeManager tm = GameObject.Find("Logic").GetComponent<TimeManager>();
        tm.addListenerToYearChange(newYear);
    }

	public Investigation getInvestigation(int id) {
		Debug.Assert (m_invest.ContainsKey (id), "Investigation NotFound " + id);
		return m_invest [id];
	}

	public List<Investigation> getAvailableInvestigations() {
		List<Investigation> res = new List<Investigation> ();
		foreach (Investigation i in m_invest.Values) {
			if (i.getReq() == -1 || m_invest[i.getReq()].isPurchased()){ 
				TimeManager tm = GameObject.Find("Logic").GetComponent<TimeManager>();
				if (!i.isPurchased() && !i.isInvestigating() && tm.getCurrentYear() >= i.getYear ()) {
					res.Add (i);
					//Debug.Log ("InvAvailable: " + i.getID ());
				}
			}
		}
		return res;
	}

	public List<Investigation> getProgressInvestigations() {
		List<Investigation> res = new List<Investigation> ();
		foreach (Investigation i in m_invest.Values) {
			if (i.isInvestigating()){
				res.Add (i);
			}
		}
		return res;
	}

	public List<Investigation> getCompletedInvestigations() {
		List<Investigation> res = new List<Investigation> ();
		foreach (Investigation i in m_invest.Values) {
			if (i.isPurchased()){
				res.Add (i);
			}
		}
		return res;
	}

	public void startInvestigation(int id) {
		m_invest [id].setInvestigating (true);
		///newYear (); // ++++++++++++++++++++++
	}

	public bool isInvestigated(INVESTIGATIONS_ID id) {
		return isInvestigated ((int)id);
	}

	public bool isInvestigated(int id) {
		Debug.Assert (m_invest.ContainsKey (id), "Investigation="+id+" NOT Exist");
		return m_invest [id].isPurchased ();
	}

	public bool areInvestigated(List<int> investigation) {
		for (int i = 0; i < investigation.Count; ++i) {
			if (!isInvestigated (investigation [i]))
				return false;
		}
		return true;
	}

	public void newYear() {
		// Se dan por investigadas las que se estaban investigando
		foreach (Investigation i in m_invest.Values) {
			if (i.isInvestigating()) {
				m_invest[i.getID()].setInvestigating (false);
				m_invest[i.getID()].setPurchased (true);
				doInvestigatedEffect(i.getID());
			}
		}

		// Se crean carteles para nuevas invs disponibles
		List<Investigation> availableInvs = getAvailableInvestigations ();
		TimeManager tm = GameObject.Find("Logic").GetComponent<TimeManager>();
		for (int i = availableInvs.Count - 1; i >= 0; --i) {
			Investigation currentInv = availableInvs [i];
			if (currentInv.getYear () == tm.getCurrentYear ()) {
				GameObject panelTemplate = Resources.Load("Prefabs/InvestigationUnlockedFocusLayer") as GameObject;
				GameObject panelInstance = GameObject.Instantiate(panelTemplate);
				GameObject panel = panelInstance.transform.FindChild("InvestigationUnlockedPanel").gameObject;
				string text = Dictionary.getString ("INVESTIGATION_UNLOCKED") + " " + currentInv.getName ();
				panel.GetComponent<InvestigationUnlockedPanel> ().ChangeText (text);
			}
		}
	}

	private void doInvestigatedEffect(int id) {
		if (id == (int) INVESTIGATIONS_ID.COOP) {
			GameObject.FindWithTag ("CoopLock").GetComponent<CoopLockBehaviour> ().unlock ();
		}
	}

	public uint getBadWeedDecreaseBonus() {
		uint res = 0;
		if (m_invest [0].isPurchased ())
			res += 10;
		if (m_invest [6].isPurchased ())
			res += 10;
		return res;
	}

	public uint getRicePerChunkBonus() {
		uint res = 0;
		if (m_invest [1].isPurchased ())
			res += 10;
		if (m_invest [3].isPurchased ())
			res += 10;
		return res;
	}

	public uint getPlanterCapacityBonus() {
		uint res = 0;
		if (m_invest [2].isPurchased ())
			res += 25;
		return res;
	}

	public uint getGoldBonusPerRiceSold() {
		uint res = 0;
		if (m_invest [0].isPurchased ())
			res += 5;
		if (m_invest [6].isPurchased ())
			res += 5;
		if (m_invest [7].isPurchased ())
			res += 5;
		if (m_invest [8].isPurchased ())
			res += 20;
		return res;
	}

	public uint getPlagueDecreaseBonus() {
		uint res = 0;
		if (m_invest [25].isPurchased ())
			res += 25;
		if (m_invest [20].isPurchased ())
			res += 25;
		return res;
	}

	public void InvestigateWithoutWaiting(int id)
	{
		if(m_invest.ContainsKey(id)) {
			m_invest [id].setPurchased (true);
		}
	}

    public void load(InvestigationManagerData investigationManagerData)
    {
        foreach(InvestigationData invData in investigationManagerData.InvestigationData) {
            m_invest[invData.ID].setInvestigating(invData.Investigating);
            m_invest[invData.ID].setPurchased(invData.Purchased);
            if(invData.Purchased) {
				doInvestigatedEffect(invData.ID);
            }
        }
    }

    public void save(InvestigationManagerData investigationManagerData)
    {
		foreach (Investigation i in m_invest.Values) {
            InvestigationData invData = new InvestigationData();
            invData.ID = i.getID();
            invData.Investigating = i.isInvestigating();
            invData.Purchased = i.isPurchased();
            investigationManagerData.InvestigationData.Add(invData);
        }
    }
}

