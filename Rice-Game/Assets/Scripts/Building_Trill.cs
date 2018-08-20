using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Trill : Building
{
    enum STATE { EMPTY, HALF, FULL };

    uint riceUnprepared;
    uint ricePrepared;
    //uint rate;
    Dictionary<STATE, string> _stateImgs;
    STATE _state;
    bool b_IsProductionStopped;

    public Building_Trill(JSONNode data, BUILDINGS type) : base(data, type)
    {
        _stateImgs = new Dictionary<STATE, string>();
        foreach (STATE state in Enum.GetValues(typeof(STATE))) {
            _stateImgs[state] = data["states"][state.ToString()];
        }
        _state = STATE.EMPTY;
        //rate = (uint)getCapacity2();

        riceUnprepared = 0;
        ricePrepared = 0;
		b_IsProductionStopped = false;
    }

    public override void upgrade() {
        base.upgrade();
        //rate = (uint)getCapacity2();
    }


    public void sendRice(uint riceSent)
    {
		riceUnprepared += Math.Min (getCurrentFreeCapacity (), riceSent);
    }

    override public void newDayCallback()
    {
        if (!b_IsProductionStopped) {
			uint riceToPrepare = Math.Min(riceUnprepared, (uint) getCapacity2 ());
            //riceToPrepare = Math.Min(riceToPrepare, (uint)getCapacity1() - ricePrepared);


            riceUnprepared -= riceToPrepare;
            ricePrepared += riceToPrepare;

            STATE newState;
			float filledRate = ricePrepared / (float)getCapacity1();

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
			//Debug.Log ("Building_Trill: filledRate=" + filledRate + " (" + newState + ")");
        }
    }

    public uint getRicePrepared() { return ricePrepared; }

    public uint getRiceUnprepared() { return riceUnprepared; }

	public void takeRicePrepared(uint riceToTake)
	{
		ricePrepared -= riceToTake;
		Debug.Assert (ricePrepared >= 0);
	}

	/*
    public uint getAndEraseRicePrepared()
    {
        uint riceToReturn = ricePrepared;
        ricePrepared = 0;


        return riceToReturn;
    }
    */

    private void changeToState(STATE newState) {
        _state = newState;
        _currentImgPath = _stateImgs[newState];
        updateRepresentation();
    }

    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        string measurementUnprepared = "kg.";
        string riceUnpreparedInfo = riceUnprepared.ToString();
        if (riceUnprepared > 1000) {
            riceUnpreparedInfo = (riceUnprepared / 1000).ToString();
            measurementUnprepared = "t.";
        }

        string measurementPrepared = "kg.";
        string ricePreparedInfo = ricePrepared.ToString();
        if (ricePrepared > 1000) {
            ricePreparedInfo = (ricePrepared / 1000).ToString();
            measurementPrepared = "t.";
        }

        string measurementCapacity = "kg.";
        string capacityInfo = this.getCapacity1().ToString();
        if (this.getCapacity1() > 1000) {
            capacityInfo = (this.getCapacity1() / 1000).ToString();
            measurementCapacity = "t.";
        }
        m_specificBuildingInfo.Clear();
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("RICE_TO_SEPARATE"), riceUnpreparedInfo + " " + measurementUnprepared));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("RICE_READY_TO_DRY"), ricePreparedInfo + " " + measurementPrepared));
        m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("CURRENT_MAX_CAPACITY"), capacityInfo + " " + measurementCapacity));

        return m_specificBuildingInfo;
    }

    public override string getWorkInfo()
    {
		string rateAndMeasurement = (uint) getCapacity2 () + " kg.";
		/*
        if (rate > 1000) {
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
        return (ricePrepared + riceUnprepared);
    }

    public void setRice(uint riceUnprepared, uint ricePrepared)
    {
        this.riceUnprepared = riceUnprepared;
        this.ricePrepared = ricePrepared;
    }

    public uint getCurrentFreeCapacity()
    {
		return (uint)getCapacity1() - getAllTheRice();
    }
}
