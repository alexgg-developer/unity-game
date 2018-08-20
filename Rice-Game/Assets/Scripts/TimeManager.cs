using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public delegate void OnTimeStateChangeHandler();
public delegate void onYearChangeHandler();
public delegate void onDayChangeHandler();

public class TimeManager : MonoBehaviour 
{
	//public float _timePerDay = 1.0f; //sec
	private float _timePerDay; //sec
	private float _timePerHour;//sec
	private float _currentTimeMultiplier;
	public event OnTimeStateChangeHandler _onTimeStateChange = null;
    private event OnTimeStateChangeHandler _onTimeMultiplierChange = null;
    private event onYearChangeHandler _onYearChange = null;
    private event onDayChangeHandler _onDayChange = null;
    private event OnTimeStateChangeHandler _onHourChange = null;
    private event OnTimeStateChangeHandler _onMonthChange = null;
    private float _elapsedTime;
	public Calendar calendar;
	public enum SPEED_MODE { PAUSE, NORMAL, FAST, SUPAFAST };
	private SPEED_MODE _currentMode;
	private float[] _timeMultiplier;
	private float _currentHour;

	public GameObject Pause, Forward, FastForward, SuperFastForward;

	private TutorialManager _tutMan;
	void Awake()
	{
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0) {
            //init();
        }
    }

    public void init()
	{
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
        calendar = new Calendar();
		_currentMode = SPEED_MODE.NORMAL;
        _timeMultiplier = new float[4] { 0.0f, 1.0f, 2.0f, 3.0f };
        _currentTimeMultiplier = _timeMultiplier[(uint)_currentMode];
		_timePerDay = 1.0f;
        _currentHour = 0;
        _timePerHour = _timePerDay / 24.0f;
		_elapsedTime = 0.0f;
		updateUI ();
    }

	void Update ()
	{
		if(!isOnPause()) {
			_elapsedTime += (Time.deltaTime * _currentTimeMultiplier);
			//Debug.Log ("Time: " + _elapsedTime + "/" + _timePerHour);
			while (_elapsedTime >= _timePerHour) {
				++_currentHour;
				_elapsedTime -= _timePerHour;
				_onHourChange ();
				if (_currentHour >= 24) {
					//Debug.Log ("DING DING DING NEW DAY");
					_currentHour = 0;
					calendar.nextDay ();
					_onTimeStateChange ();
					_onDayChange ();
                    if (calendar.hasMonthChanged()) {
                        if (_onMonthChange != null) { 
                            _onMonthChange();
                        }
                    }
					if (calendar.hasYearChanged ()) {
						_onYearChange ();
					}
				}
			}
		}
	}

	private void onTimeMultiplierChange() 
	{
		_currentTimeMultiplier = _timeMultiplier[(uint)_currentMode];
		updateUI ();
 		_onTimeMultiplierChange ();
	}

	private void updateUI () {
		switch (_currentMode) {
		case SPEED_MODE.PAUSE:
			Pause.SetActive (true);
			Forward.SetActive (true);
			FastForward.SetActive (false);
			SuperFastForward.SetActive (false);
			break;
		case SPEED_MODE.NORMAL:
			Pause.SetActive (false);
			Forward.SetActive (true);
			FastForward.SetActive (false);
			SuperFastForward.SetActive (false);
			break;
		case SPEED_MODE.FAST:
			Pause.SetActive (false);
			Forward.SetActive (false);
			FastForward.SetActive (true);
			SuperFastForward.SetActive (false);
			break;
		case SPEED_MODE.SUPAFAST:
			Pause.SetActive (false);
			Forward.SetActive (false);
			FastForward.SetActive (false);
			SuperFastForward.SetActive (true);
			break;
		}
	}

	public SPEED_MODE getCurrentMode() {
		return _currentMode;
	}

	public uint getCurrentDay()
	{
		return calendar._currentDay;
	}

	public uint getCurrentMonth()
	{
		return calendar._currentMonth;
	}
		
	public uint getCurrentYear()
	{
		return calendar._currentYear;
	}

	public uint getFinalDayOfMonth(uint month)
	{
		return calendar.getFinalDayOfMonth (month);
	}

	public void startTime(bool force=false)
	{
		if (force || _tutMan.getState () == TutorialManager.STATES.END) {
			Debug.Log ("Time Started");
			if (isOnPause ()) {
				_currentMode = SPEED_MODE.NORMAL;
				onTimeMultiplierChange ();
			}
		}
	}

	public void pauseTime(bool force=false)
	{
		if (force || _tutMan.getState () == TutorialManager.STATES.END) {
			Debug.Log ("Time Paused");
			if (!isOnPause ()) {
				_currentMode = SPEED_MODE.PAUSE;
				onTimeMultiplierChange ();
			}
		}
	}

	public void switchPauseTime(bool force=false)
	{
		if (isOnPause ()) {
			startTime (force);
		} else {
			pauseTime (force);
		}
    }

    public void nextMode()
	{
		if (_tutMan.getState () == TutorialManager.STATES.END) {
			if (_currentMode == SPEED_MODE.PAUSE) {
				startTime ();
			} else if (_currentMode == SPEED_MODE.SUPAFAST) {
				changeMode(SPEED_MODE.NORMAL);
			} else {
				changeMode(++_currentMode);
			}
		}
	}

	public void changeMode(SPEED_MODE mode)
	{
		if (mode == SPEED_MODE.PAUSE && !isOnPause()) {
			switchPauseTime();
		}
		else  {
			if (isOnPause()) {
				switchPauseTime();
			}
			_currentMode = mode;
			this.onTimeMultiplierChange();
		}
	}

	public void resetMode()
	{
		changeMode(SPEED_MODE.NORMAL);
	}

	public void addListenerToYearChange(onYearChangeHandler function)
	{
		_onYearChange += function;
	}

    public void addListenerToMonthChange(OnTimeStateChangeHandler function)
    {
        _onMonthChange += function;
    }

    public void removeListenerToMonthChange(OnTimeStateChangeHandler function)
    {
        _onMonthChange -= function;
    }

    public void addListenerToMultiplierChange(OnTimeStateChangeHandler function)
	{
		_onTimeMultiplierChange += function;
	}

    public void addListerToDayChange(onDayChangeHandler function)
    {
        _onDayChange += function;
    }

    public void removeListenerToDayChange(onDayChangeHandler function)
    {
        _onDayChange -= function;
    }

    public void addListenerToHourChange(OnTimeStateChangeHandler function)
	{
		_onHourChange += function;
	}

    public void removeListenerToHourChange(OnTimeStateChangeHandler function)
    {
        _onHourChange -= function;
    }

    public string getDate()
	{
		string date = calendar._currentDay.ToString() + "/" + calendar._currentMonth.ToString() + "/" + calendar._currentYear.ToString();

		return date;
	}

	public float getCurrentMultiplier()
	{
		return _currentTimeMultiplier;
	}

	public float getTimePerHour()
	{
		return _timePerHour;
	}

    public uint getDaysOfAMonth(uint month)
    {
        return calendar.getDaysOfAMonth(month);
    }

    public bool isOnPause()
    {
		return _currentMode == SPEED_MODE.PAUSE;
    }

    public void save(TimeManagerData timeManagerData)
    {
        timeManagerData.CurrentDay = calendar._currentDay;
        timeManagerData.CurrentMonth = calendar._currentMonth;
        timeManagerData.CurrentYear = calendar._currentYear;
        timeManagerData.CurrentHour = _currentHour;
    }

    public void load(TimeManagerData timeManagerData)
	{
		Debug.Log ("Loading... TimeManager");
        init();
        _currentHour = timeManagerData.CurrentHour;
        calendar._currentDay = timeManagerData.CurrentDay;
        calendar._currentMonth = timeManagerData.CurrentMonth;
        calendar._currentYear = timeManagerData.CurrentYear;
    }

    public void setDate(uint day, uint month, uint year)
    {
        calendar._currentDay = day;
        calendar._currentMonth = month;
        calendar._currentYear = year;
    }
}
