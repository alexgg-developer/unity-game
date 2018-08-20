using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Silo: Building
{
	enum STATE { EMPTY, HALF, FULL };
	
	uint riceUnprepared;
	//uint rate;
	STATE _state;
	Dictionary<STATE, string> _stateImgs;
    bool b_IsProductionStopped;

    public Building_Silo (JSONNode data, BUILDINGS type): base(data, type)
	{
		_stateImgs = new Dictionary<STATE, string> ();		
		foreach (STATE state in Enum.GetValues(typeof(STATE))) {
			_stateImgs[state] = data ["states"] [state.ToString ()];
		}
		_state = STATE.EMPTY;
		riceUnprepared = 0;
		//rate = (uint) getCapacity2 ();
		b_IsProductionStopped = false;
		//Debug.Log("data[states][state.ToString()]:::" + data["states"]["EMPTY"]);
	}

	public override void upgrade() {
		base.upgrade ();
		//rate = (uint) getCapacity2 ();
	}

	public void sendRice(uint riceSent) 
	{
		uint riceRecibed = Math.Min (getCurrentFreeCapacity (), riceSent);
		Debug.Log ("Building_Silo: recibidos "+riceRecibed+" kg de "+riceSent+" Kg");
		riceUnprepared += riceRecibed;
		//Debug.Log ("Building_Silo: recibidos "+riceSent+" Kg");
		//riceUnprepared = Math.Min (riceUnprepared + riceSent, (uint) getCapacity1 ());
	}
	
	override public void newDayCallback()
	{
        if (!b_IsProductionStopped) {
			uint riceToPrepare = Math.Min(riceUnprepared, (uint) getCapacity2 ());
            riceUnprepared -= riceToPrepare;
            UserDataManager.GetInstance().sellRice(riceToPrepare);

            STATE newState;
			float filledRate = riceUnprepared / (float)getCapacity1();
            
            if (filledRate < 0.002) {
                newState = STATE.EMPTY;
            }
            else if (filledRate < 0.8) {
                newState = STATE.HALF;
            }
            else {
                newState = STATE.FULL;
            }

            if (_state != newState) {
                changeToState(newState);
			}
			//Debug.Log ("Building_Silo: filledRate=" + filledRate + " (" + newState + ")");
        }
	}

	private void changeToState(STATE newState) {
		_state = newState;
		_currentImgPath = _stateImgs [newState];
		updateRepresentation ();
	}

    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        string measurementUnprepared = "kg.";
        string riceUnpreparedInfo = riceUnprepared.ToString();
        if (riceUnprepared > 1000)
        {
            riceUnpreparedInfo = (riceUnprepared / 1000).ToString();
            measurementUnprepared = "t.";
        }

        string measurementCapacity = "kg.";
        string capacityInfo = this.getCapacity1().ToString();
        if (this.getCapacity1() > 1000)
        {
            capacityInfo = (this.getCapacity1() / 1000).ToString();
            measurementCapacity = "t.";
        }
        m_specificBuildingInfo.Clear(); 
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("RICE_TO_PACK"), riceUnpreparedInfo + " " + measurementUnprepared));
        m_specificBuildingInfo.Add(new Pair<string, string>("", ""));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("CURRENT_MAX_CAPACITY"), capacityInfo + " " + measurementCapacity));

        return m_specificBuildingInfo;
    }

    public override string getWorkInfo()
    {
		string rateAndMeasurement = (uint) getCapacity2 () + " kg.";
		/*
        if (rate > 1000)
        {
            rateAndMeasurement = (rate / 1000) + " t.";
        }
        */
        return rateAndMeasurement;
    }


    public void stopProduction()
    {
        b_IsProductionStopped = true;
    }

    public uint getAllTheRice()
    {
        return riceUnprepared;
    }

    public void setRice(uint riceUnprepared)
    {
        this.riceUnprepared = riceUnprepared;
	}

	public uint getCurrentFreeCapacity()
	{
		return (uint)getCapacity1() - getAllTheRice();
	}
}

