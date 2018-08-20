using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * PhaseManager - Alex
 * Es un manager que se ocupa de controlar la informacion relacionada con las fases
 * del cultivo del arroz segun el doc:
 * https://docs.google.com/document/d/1MDgYp0W9jbiQO2NNFqtR_qfnS7iTMA53hy44Pux4Dfw/edit * 
 */

public delegate void onPhaseChangeHandler();
public enum TypeFase { PREWORK_I, PREWORK_II, SOWING, GERMINATION, PLANT, HARVEST, POST_WORK, NONE };	

public class Phase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> ActionsInfo { get; set; }
}

public class PhaseManager : MonoBehaviour 
{
	public TypeFase _currentPhase, _lastPhase;

	//Caution some logics depending on the thing that there is only 1 month of difference between phases
	private uint[] _startPhaseMonth = {  0,  2,  3,  4,  5,  8,  9}; //0 -> Enero, etc
	private uint[] _startPhaseDay =   { 16,  1, 16,  9,  1,  1, 16};
	private uint[] _endPhaseMonth =    {  0,  3,  4,  4,  6,  9, 10};
	private uint[] _endPhaseDay =      { 31, 15,  8, 31, 15, 15, 15};
	private TimeManager _timeManager;
	private List<List<int>> _phaseActions;
	private onPhaseChangeHandler _onPhaseChange;
    private List<Phase> m_phasesInfo;

	void Awake()
	{
		_timeManager = GetComponent<TimeManager>();
		_timeManager.init ();
		_timeManager._onTimeStateChange += handleOnTimeStateChange;
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
        }
    }

	void Start ()
    {
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
			_phaseActions = JSONLoader.readPhaseActions();
			m_phasesInfo = JSONLoader.readPhaseInfo();
            _currentPhase = (TypeFase)determinePhase();
			if (_currentPhase != TypeFase.PREWORK_I) {
				_lastPhase = (TypeFase)DeterminePreviousPhase (_currentPhase);
			}
        }
	}

	private TypeFase DeterminePreviousUsefulPhase(TypeFase _currentPhase) 
	{
		TypeFase lastPhase = TypeFase.NONE;
		foreach (TypeFase phase in Enum.GetValues(typeof(TypeFase))) {
			if (phase == _currentPhase) {
				break;
			}
			else {
				lastPhase = phase;
			}
		}
		return lastPhase;
	}

	private TypeFase DeterminePreviousPhase(TypeFase _currentPhase)
	{
		if (_currentPhase != TypeFase.NONE) {
			uint day = _startPhaseDay [(int)_currentPhase];
			uint month = _startPhaseMonth[(int)_currentPhase];
			--day;
			if (day == 0) {
				--month;
				if (month == 0) {
					return TypeFase.NONE;
				} 
				else {
					day = _timeManager.getDaysOfAMonth (month);
				}
			}
			return DeterminePhaseByDate (day, month);
		} 
		else {
			uint currentDay = _timeManager.getCurrentDay ();
			uint currentMonth = _timeManager.getCurrentMonth ();
			int i = 0;
			bool phaseFound = false;
			while (currentMonth >= _startPhaseMonth [i]) {
				if(currentMonth == _startPhaseMonth[i] && currentDay > _endPhaseDay[i]) {
					break;
				}
				else {
					phaseFound = true;
					++i;
				}
			}
			if (phaseFound) {
				//var phases = Enum.GetValues (typeof(TypeFase));
				int j = 0;
				foreach (TypeFase phase in Enum.GetValues (typeof(TypeFase))) {
					if (j == i) {
						return phase;
					}
					++j;
				}
			}
			return TypeFase.NONE;
		}


	}

	private TypeFase DeterminePhaseByDate(uint day, uint month)
	{
		bool phaseFound = false;
		uint phase = (uint) TypeFase.NONE;
		uint i = 0;
		while (!phaseFound && i < _startPhaseDay.Length) {
			if(_startPhaseMonth[i] <= month && _endPhaseMonth[i] >= month) {
				if(month == _startPhaseMonth[i] && month == _endPhaseMonth[i]) {
					if(day >= _startPhaseDay[i] && day <= _endPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else if(month == _startPhaseMonth[i]) {
					if(day >= _startPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else if(month == _endPhaseMonth[i]) {
					if(day <= _endPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else {
					phase = i;
					phaseFound = true;
				}

			}
			++i;
		}
		return (TypeFase)phase;
	}

	public void handleOnTimeStateChange()
	{
		TypeFase newPhase = (TypeFase) determinePhase ();
		if(_currentPhase != newPhase) {
			_lastPhase = _currentPhase;
			_currentPhase = newPhase;
			_onPhaseChange();
		}
	}

    public void save(PhaseManagerData phaseManagerData)
    {
        phaseManagerData.CurrentPhase = _currentPhase;
        phaseManagerData.LastPhase = _lastPhase;
    }
    
    public void load(PhaseManagerData phaseManagerData)
    {
		Debug.Log ("Loading... PhaseManager");
        _timeManager = GetComponent<TimeManager>();
        _timeManager._onTimeStateChange += handleOnTimeStateChange;
		_phaseActions = JSONLoader.readPhaseActions();
		m_phasesInfo = JSONLoader.readPhaseInfo();
        _currentPhase = phaseManagerData.CurrentPhase;
        _lastPhase = phaseManagerData.LastPhase;
    }

    private uint determinePhase()
	{
		bool phaseFound = false;
		uint phase = (uint) TypeFase.NONE;
		uint currentMonth = _timeManager.getCurrentMonth ();
		uint currentDay = _timeManager.getCurrentDay ();
		uint i = 0;
		while (!phaseFound && i < _startPhaseDay.Length) {
			if(_startPhaseMonth[i] <= currentMonth && _endPhaseMonth[i] >= currentMonth) {
				if(currentMonth == _startPhaseMonth[i] && currentMonth == _endPhaseMonth[i]) {
					if(currentDay >= _startPhaseDay[i] && currentDay <= _endPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else if(currentMonth == _startPhaseMonth[i]) {
					if(currentDay >= _startPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else if(currentMonth == _endPhaseMonth[i]) {
					if(currentDay <= _endPhaseDay[i]) {
						phase = i;
						phaseFound = true;
					}
				}
				else {
					phase = i;
					phaseFound = true;
				}

			}
			++i;
		}
		return phase;
	}
	
	
	public void addListenerToPhaseChange(onPhaseChangeHandler function)
	{
		_onPhaseChange += function;
	}

    public void removeListenerToPhaseChange(onPhaseChangeHandler function)
    {
        _onPhaseChange -= function;
    }

    public TypeFase getCurrentPhase()
	{
		return _currentPhase;
	}

    public void setCurrentPhase(TypeFase phase)
    {
        _currentPhase = phase;
    }

    public TypeFase getLastPhase()
	{
		return _lastPhase;
	}
	
	public List<int> getActionsInPhase(TypeFase phase) 
	{
		return _phaseActions[(int) phase];
	}
	
	public  List<int> getActionsInCurrentPhase() 
	{
		return _phaseActions[(int) _currentPhase];
	}
	
	public List<int> getActionsInLastPhase() 
	{
		if(_lastPhase != TypeFase.NONE) {
			return _phaseActions[(int) _lastPhase];
		}
		else {
			return new List<int>();
		}
	}

	public List<int> GetActionsTillLastUsefulPhase() 
	{
		List<int> actions = new List<int> ();	
		TypeFase lastUsefulPhase = DeterminePreviousUsefulPhase (_currentPhase);
		if (lastUsefulPhase != TypeFase.NONE) {
			foreach (TypeFase phase in Enum.GetValues(typeof(TypeFase))) {
				if (phase == _currentPhase) {
					break;
				}
				else {
					List<int> actionsThisPhase = _phaseActions [(int)_lastPhase];
					actions.AddRange (actionsThisPhase);
				}
			}
		}
		return actions;
	}

    public int getRemainingDaysOfCurrentPhase()
    {
        uint remainingDays = 0;
        if (_currentPhase != TypeFase.NONE) {
            uint currentMonth = _timeManager.getCurrentMonth();
            uint currentDay = _timeManager.getCurrentDay();
            if (_endPhaseMonth[(int)_currentPhase] > currentMonth) {
                uint daysOfCurrentMonth = _timeManager.getDaysOfAMonth(currentMonth);
                uint daysOfNextMonth = _endPhaseDay[(int)_currentPhase];
                remainingDays = daysOfCurrentMonth - currentDay + daysOfNextMonth;
            }
            else {
                uint endDay = _endPhaseDay[(int)_currentPhase];
                remainingDays = endDay - currentDay;
            }
        }
		return (int)remainingDays;
    }

    public Phase getCurrentPhaseInfo()
    {
        return m_phasesInfo[(int)_currentPhase];
    }

    public uint getStartDayOfPhase(TypeFase phase)
    {
        if (phase == TypeFase.NONE) return 0;
        return _startPhaseDay[(int)phase];
    }

    public uint getStartMonthOfPhase(TypeFase phase)
    {
        if (phase == TypeFase.NONE) return 0;
        return _startPhaseMonth[(int)phase];
    }
}
