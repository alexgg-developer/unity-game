using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
using MyUtils.Pair;

public class Building_Planter: Building
{
	enum STATE {EMPTY, GROWING, GROWN};

	bool _sembrat;
	const uint DIAS_HASTA_CRECER = 10;	
	STATE _state;
	//uint _maxPlantes;
	Dictionary<STATE, string> _stateImgs;
	private List<GameObject> _object;

    uint _diesSembrat;
    public uint DiesSembrat
    {
        get
        {
            return _diesSembrat;
        }
        set
        {
            _diesSembrat = value;
        }
    }
    uint _plantes;
    public uint Plantes
    {
        get
        {
            return _plantes;
        }
        set
        {
            _plantes = value;
        }

    }

	private TutorialManager _tutMan;
    public Building_Planter  (JSONNode data, BUILDINGS type): base(data, type)
	{
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		_stateImgs = new Dictionary<STATE, string> ();		
		foreach (STATE state in Enum.GetValues(typeof(STATE))) {
			_stateImgs[state] = data["states"][state.ToString()];
		}
		_sembrat = false;
		_diesSembrat = 0;
		changeToState(STATE.EMPTY, false);
		_plantes = 0;
		//_maxPlantes = (uint) (getCapacity1 () * WorldTerrain.GetInstance().getTotalTilesInAChunk()) + InvestigationManager.GetInstance().getPlanterCapacityBonus();
	}

	public uint getMaxPlantes() {
		uint basicCapacity = (uint) (getCapacity1 () * WorldTerrain.GetInstance ().getTotalTilesInAChunk ());
		uint bonusCapacity = basicCapacity * InvestigationManager.GetInstance ().getPlanterCapacityBonus () / 100;
		return basicCapacity + bonusCapacity;
	}

	public void sembrar() 
	{
		_sembrat = true;
		_diesSembrat = 0;
		_plantes = getMaxPlantes();
		_tutMan.eventDone (TutorialManager.EVENTS.PLANTELL_SEMBRAT);
	}

    public void sembrar(uint diesSembrat, uint plantes)
    {
        _sembrat = true;
        _diesSembrat = diesSembrat;
		_plantes = plantes;
		_tutMan.eventDone (TutorialManager.EVENTS.PLANTELL_SEMBRAT);
    }

    public bool estaSembrat()
	{
		return _sembrat;
	}

	public bool estaCreixent()
	{
		return estaSembrat () && !estaCrescut ();
	}

	// la germinacion ha terminado y esta listo para recoger
	public bool estaCrescut()
	{
		return _diesSembrat >= DIAS_HASTA_CRECER && _sembrat;
	}

	public bool hasPlants()
	{
		return estaCrescut() && _plantes >= 0;
    }

    public bool hasPlantsForAChunk()
    {        
		return estaCrescut() && _plantes >= WorldTerrain.GetInstance().getTotalTilesInAChunk();
    }

    public void agafarPlanta()
	{
		Debug.Assert (hasPlantsForAChunk());
		_plantes -= (uint)WorldTerrain.GetInstance().getTotalTilesInAChunk();
		if (_plantes <= 0) {
			_sembrat = false;
			//changeRepresentation
		}
	}

	public bool esEpocaDeSembrar()
	{
		return GameObject.FindGameObjectWithTag ("Logic").GetComponent<PhaseManager> ().getCurrentPhase () == TypeFase.SOWING;
	}

	public override void upgrade() {
		base.upgrade ();
		//_maxPlantes = (uint) (getCapacity1 () * WorldTerrain.GetInstance().getTotalTilesInAChunk());
	}

	override public void newDayCallback()
	{
		++_diesSembrat;

		STATE newState;
		float growingRate = _diesSembrat / (float) DIAS_HASTA_CRECER;
		//Debug.Log ("Plantell Growing Rate: " + growingRate);
		if (!_sembrat) {
			newState = STATE.EMPTY;
		} else if (growingRate < 0.2f) {
			newState = STATE.EMPTY;
		} else if (!estaCrescut()) {
			newState = STATE.GROWING;
		} else {
			newState = STATE.GROWN;
		}

		if (_state != newState) {
			changeToState(newState);
		}
	}

	private void changeToState(STATE newState, bool updateRepr = true)
    {
		_state = newState;
		_currentImgPath = _stateImgs [newState];
		if (updateRepr)
			updateRepresentation ();
	}

    public override List<Pair<string, string>> getSpecificBuildingInfo()
    {
        m_specificBuildingInfo.Clear();
		if (estaCrescut ()) {
			m_specificBuildingInfo.Add (new Pair<string, string> (Dictionary.getString ("PLANTER_PLANTS_AVAILABLE"), _plantes.ToString ()));
		} else if (estaCreixent ()) {
			m_specificBuildingInfo.Add (new Pair<string, string> (Dictionary.getString ("PLANTER_GROWING"), _diesSembrat * 100/ (float) DIAS_HASTA_CRECER + " %"));
		} else if (esEpocaDeSembrar ()) {
			m_specificBuildingInfo.Add (new Pair<string, string> (Dictionary.getString ("PLANTER_NOT_SOWN"), ""));
		} else {
			m_specificBuildingInfo.Add (new Pair<string, string> (Dictionary.getString ("PLANTER_NOT_PHASE"), ""));
		}

        m_specificBuildingInfo.Add(new Pair<string, string>("", ""));
		m_specificBuildingInfo.Add(new Pair<string, string>(Dictionary.getString("CURRENT_MAX_CAPACITY"), (getMaxPlantes()).ToString()));

        return m_specificBuildingInfo;
    }    

    public void reset()
    {
        _sembrat = false;
        _plantes = 0;
        _diesSembrat = 0;
        changeToState(STATE.EMPTY);
    }

}

