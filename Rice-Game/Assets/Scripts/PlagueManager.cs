using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public enum TypePlague {CARGOL_POMA,FLAMENCO1,FLAMENCO2,FLAMENCO3,PALOMETA,QUIRONOMIDS,NONE};

public class PlagueInstance
{
    public GameObject instance;
    public RiceTerrainTile _parent;
    public TypePlague type;
    public bool active;
	public int PenalizationDays { get; set; }

    public PlagueInstance(RiceTerrainTile t)
    {
        _parent = t;
        instance = null;
        active = false;
        type = TypePlague.NONE;
		PenalizationDays = 0;
    }

    public void delete()
    {
        type = TypePlague.NONE;
        if (instance != null)
        {
            GameObject.Destroy(instance);
            instance = null;
		}
		PenalizationDays = 0;
    }
}

public class Plague {
	int id;
	string name;
	string desc;
	string imgPath;
	int monthIn;
	int monthOut;
	int baseDmg;
	int spawnChance;
	bool active;
	bool happened;

	public Plague(int aId, string aName, string aDesc, string img, int month1, int month2, int dmg){
		id = aId;
		name = aName;
		desc = aDesc;
		imgPath = img;
		monthIn = month1;
		monthOut = month2;
		baseDmg = dmg;
		active = false;
		happened = false;
		spawnChance = 10;
	}

	public int getID(){return id;}
	public string getName(){return name;}
	public string getDesc(){return desc;}
	public string getImg(){return imgPath;}
	public int getMonthIn(){return monthIn;}
	public int getMonthOut(){return monthOut;}
	public int getBaseDmg(){return baseDmg;}
	public int getSpawnChance(){return spawnChance;}
	public bool isActive(){return active;}
	public void setActive(bool b){active = b; happened = true;}
	public bool alreadyHappened(){return happened;}
	public void reset(){happened = false; active = false;}

}

public class PlagueManager {

	private static PlagueManager instance = null;
	private List<Plague> plagues;

    public static PlagueManager GetInstance()
	{
		if(instance == null)
		{
			instance = new PlagueManager();
		}

		return instance;
	}

    private PlagueManager() 
	{
		plagues = new List<Plague> ();
		JSONLoader.readPlagues (plagues);
	}

	public Plague getPlague(int id){
		return plagues [id];
	}

	public int getPlagueCount(){
		return plagues.Count;
	}

	public void resetPlagues(){
		foreach (Plague p in plagues) {
			p.reset ();
		}
	}

    public void save(PlagueManagerData plagueManagerData)
    {
        for (int i = 0; i < plagues.Count; i++) {
            PlagueData plagueData = new PlagueData();
            plagueData.AlreadyHappened = plagues[i].alreadyHappened();
            plagueData.IsActive = plagues[i].isActive();
            plagueManagerData.PlaguesData.Add(plagueData);
        }
    }


    public void load(PlagueManagerData plagueManagerData)
    {
        for(int i = 0; i < plagueManagerData.PlaguesData.Count; ++i) {
            if (plagueManagerData.PlaguesData[i].IsActive) {
                plagues[i].setActive(true);
            }
            else if (plagueManagerData.PlaguesData[i].AlreadyHappened) {
                plagues[i].setActive(false);
            }
            else {
                plagues[i].reset();
            }
        }
    }
}
