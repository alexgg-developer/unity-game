using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * Month go from 0 to 11
 * Days go from 1 to 31
 */
public class Calendar
{    
	public uint _currentYear { get; set; }
	public uint _currentMonth { get; set; }
	public uint _currentDay { get; set; }
    //public string[] _monthName = { "Gener", "Febrer", "Març", "Abril", "Maig", "Juny", "Juliol", "Agost", "Setembre", "Octubre", "Novembre", "Desembre" };
    //public List<String> _monthName;
	public static readonly uint[] _monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
	public static readonly uint START_YEAR = 1912;
	private bool _yearChanged;
    private bool _monthChanged;

    public Calendar() 
	{
        /*_monthName = new List<string>() {
            Dictionary.getString("JANUARY"), Dictionary.getString("FEBRAURY"), Dictionary.getString("MARCH"),
            Dictionary.getString("APRIL"),Dictionary.getString("MAY"),Dictionary.getString("JUNE"),
            Dictionary.getString("JULY"), Dictionary.getString("AUGUST"), Dictionary.getString("SEPTEMBER"),
            Dictionary.getString("OCTOBER"), Dictionary.getString("NOVEMBER"), Dictionary.getString("DECEMBER")};*/
		_currentYear = START_YEAR;
		//_currentYear = 1970;
		//_currentMonth = (uint)UnityEngine.Random.Range (0, 12);
		//Original to start at PREWORK_III
		_currentMonth = 0;
		_currentDay = 16;
		_yearChanged = false;
		_monthChanged = false;
		//---PREWORK_I---
		//_currentMonth = 0;
		//_currentDay = 1;
		//---SOWING---
		//_currentMonth = 4;
		//_currentDay = 1;
	}	

	public void nextDay() 
	{		
		_currentDay = _currentDay + 1;;
		if (_currentDay == (_monthDays[_currentMonth] + 1)) {
			_currentDay = 1;
			this.nextMonth();
		}
		//Debug.Log ("Current day:::" + _currentDay);
	}

	public void nextMonth() 
	{		
		_currentMonth = _currentMonth + 1;
		if (_currentMonth == 12) {
			_currentMonth = 0;
			this.nextYear ();
		}
        _monthChanged = true;
	}

	public void nextYear()
	{		
		_currentYear++;
		if(isLeapYear(_currentYear)) {
			_monthDays[1]++;
		}
		else if(isLeapYear(_currentYear-1)) {
			_monthDays[1]--;
		}
		_yearChanged = true;
	}

	public bool isLeapYear(uint year) 
	{
		return (year % 4 == 0) && ((year % 100 != 0) || (year % 400 != 0));
	}

	public uint getFinalDayOfMonth(uint month)
	{
		return _monthDays [month];
	}

	public bool hasYearChanged()
	{
		bool yearChanged = _yearChanged;
		_yearChanged = false;

		return yearChanged;
	}

    public bool hasMonthChanged()
    {
        bool monthChanged = _monthChanged;
        _monthChanged = false;

        return monthChanged;
    }

    public uint getDaysOfAMonth(uint month)
    {
		return Calendar._monthDays[month];
    }

	//day of reference has to be greater that the day to check
	public uint CalculateDistance(uint dayToCheck, uint monthToCheck, uint referenceDay, uint referenceMonth) 
	{		
		uint distance = 0;
		if (monthToCheck == referenceMonth) {
			distance = referenceDay - dayToCheck;
		}
		else {
			distance += referenceDay;
			uint i = referenceMonth - 1;
			while (i > monthToCheck) {
				distance += _monthDays [i];
				--i;
			}
			distance += _monthDays [monthToCheck] - dayToCheck;
		}
		return distance;
	}
}