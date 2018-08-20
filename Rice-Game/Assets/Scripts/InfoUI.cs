using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InfoUI : MonoBehaviour {
	
	private Text coinsText;
    private Text m_coinsRedText;
	private Text dateText;
    private Text riceCounterText;
    private Image m_warningIcon;
    private Image m_coinIcon;
    private TimeManager time;

	void Start () {
		coinsText = GameObject.Find ("Coins").GetComponent<Text>();
        m_coinsRedText = GameObject.Find("CoinsRed").GetComponent<Text>();
        dateText = GameObject.Find ("Date").GetComponent<Text>();
        riceCounterText = GameObject.Find("CounterText").GetComponent<Text>();
        m_coinIcon = GameObject.Find("CoinIcon").GetComponent<Image>();
        m_warningIcon = GameObject.Find("WarningIcon").GetComponent<Image>(); 
        time = GameObject.Find ("Logic").GetComponent<TimeManager>();
		if (!UserDataManager.GetInstance().gold.inRed()) {
            coinsText.gameObject.SetActive(true);
            m_coinIcon.gameObject.SetActive(true);
            m_coinsRedText.gameObject.SetActive(false);
            m_warningIcon.gameObject.SetActive(false);
        }
        else {
            coinsText.gameObject.SetActive(false);
            m_coinIcon.gameObject.SetActive(false);
            m_coinsRedText.gameObject.SetActive(true);
            m_warningIcon.gameObject.SetActive(true);
        }
    }

    void Update () {
		uint day = time.getCurrentDay();
		uint month = time.getCurrentMonth();
		uint year = time.getCurrentYear();
        // Updating UI text
		if (!UserDataManager.GetInstance().gold.inRed()) {
            coinsText.gameObject.SetActive(true);
            m_coinIcon.gameObject.SetActive(true);
            m_coinsRedText.gameObject.SetActive(false);
            m_warningIcon.gameObject.SetActive(false);            
            coinsText.text = UserDataManager.GetInstance().gold.getGold().ToString();
        }
        else {
            coinsText.gameObject.SetActive(false);
            m_coinIcon.gameObject.SetActive(false);
            m_coinsRedText.gameObject.SetActive(true);
            m_warningIcon.gameObject.SetActive(true);
            int gold = Math.Abs(UserDataManager.GetInstance().gold.getGold());
            m_coinsRedText.text = gold.ToString();
        }
        dateText.text = makeDate(day, month+1, year);

        uint totalRiceProduced = UserDataManager.GetInstance().getTotalRiceProduced();
        if (totalRiceProduced >= 10000) {
            double tonesProduced = (float)totalRiceProduced / 1000.0f;
			tonesProduced = Math.Round (tonesProduced, 2);
			riceCounterText.text = string.Format("{0:0.00}", tonesProduced) + " t.";
        }
        else {
            riceCounterText.text = totalRiceProduced.ToString() + " kg";
        }

    }

	string makeDate(uint day, uint month, uint year)
	{
		return day.ToString()+"/"+month.ToString()+"/"+year.ToString();
	}
}
