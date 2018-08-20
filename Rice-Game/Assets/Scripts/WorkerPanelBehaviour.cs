using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class WorkerPanelBehaviour : Panel
{
    private Text m_freeWorkers = null;
    private Text m_busyWorkers = null;
    private Text m_space = null;
    private Text m_hireButtonText = null;
    private Text m_monthlyMoney = null;
    private Button m_hireButton = null;
    private Button m_fireButton = null;
    private Text m_fireButtonText = null;

    override protected void initBase()
    {
        this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
    }

    void Awake()
    {
        initBase();
    }
    
    void OnEnable()
    {
        init();
    }

    private void init()
    {
        if (m_freeWorkers == null) m_freeWorkers = GameObject.Find("FreeWorkerText").GetComponent<Text>();
        if (m_busyWorkers == null) m_busyWorkers = GameObject.Find("BusyWorkersText").GetComponent<Text>();
        if (m_space == null) m_space = GameObject.Find("SpaceText").GetComponent<Text>();
        if (m_hireButtonText == null) {
            m_hireButton = GameObject.Find("BuyButton").GetComponent<Button>();
            m_hireButtonText = m_hireButton.transform.FindChild("BuyButtonText").GetComponent<Text>();
        }
        if (m_fireButton == null)
        {
            m_fireButton = GameObject.Find("FireButton").GetComponent<Button>();
            m_fireButtonText = m_fireButton.transform.FindChild("FireButtonText").GetComponent<Text>();
        }
        if (m_monthlyMoney == null) {
            Image monthlyMoneyImage = GameObject.Find("MonthMoneyImage").GetComponent<Image>();
            m_monthlyMoney = monthlyMoneyImage.transform.FindChild("MonthMoneyText").GetComponent<Text>();
        }
        WorkerManager.GetInstance().addOnWorkerChangeListener(updatePanel);
        TimeManager timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
        timeManager.addListenerToMonthChange(updatePanel);
        updatePanel();
    }

    private void updatePanel()
    {
        
        if (m_freeWorkers != null) {
            int freeWorkers = WorkerManager.GetInstance().FreeWorkers;
            m_freeWorkers.text = freeWorkers.ToString() + " " + Dictionary.getString("FREE");
            int busyWorkers = WorkerManager.GetInstance().BusyWorkers;
            m_busyWorkers.text = busyWorkers.ToString() + " " + Dictionary.getString("BUSY_PLURAL");
            BuildingsManager bm = BuildingsManager.GetInstance ();
			int totalSpace = (bm.isBuilded(BUILDINGS.CASA)) ? bm.getBuilding(BUILDINGS.CASA).getCurrentCapacity() : 1;
            m_space.text = (freeWorkers + busyWorkers).ToString() + "/" + totalSpace;
            int monthlyMoney = WorkerManager.GetInstance().calculateMonthlyMoney();
            m_monthlyMoney.text = monthlyMoney.ToString();
            uint workerCost = WorkerManager.GetInstance().getWorkerCost();
            if (WorkerManager.GetInstance().Workers < totalSpace) {
                m_hireButtonText.text = Dictionary.getString("HIRE") + " +" + workerCost.ToString();
                m_hireButton.interactable = true;
            }
            else {
                m_hireButtonText.text = Dictionary.getString("NO_SPACE_LEFT_HOME");
                m_hireButton.interactable = false;
            }

            if (WorkerManager.GetInstance().FreeWorkers > 0 && WorkerManager.GetInstance().Workers > 1)
            {
                m_fireButtonText.text = Dictionary.getString("FIRE_WORKER");
                m_fireButton.interactable = true;
            }
            else
            {
                if(WorkerManager.GetInstance().Workers == 1) {
                    m_fireButtonText.text = Dictionary.getString("YOU_ARE_ONLY_LEFT");
                }
                else {
                    m_fireButtonText.text = Dictionary.getString("BUSY_WORKERS");
                }
                m_fireButton.interactable = false;
            }
        }
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

    public void hireClicked()
    {
        WorkerManager.GetInstance().hireWorker();
    }

    public void fireClicked()
    {
        WorkerManager.GetInstance().fireWorker();
    }

    public new void kill()
    {
        WorkerManager.GetInstance().removeOnWorkerChangeListener(updatePanel);
        TimeManager timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
        timeManager.removeListenerToMonthChange(updatePanel);
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
