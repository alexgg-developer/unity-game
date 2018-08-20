using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class WorkerManager {
    // SINGLETON
    private static WorkerManager instance = null;
    public static WorkerManager GetInstance()
    {
        if (instance == null)
        {
            instance = new WorkerManager();
        }

        return instance;
    }
    private int _workers;
    public int Workers {
        get
        {
            return _workers;
        }
        set
        {
            _workers = value;
            if (m_onWorkerChange != null) {
                m_onWorkerChange();
            }
        }
    }
    private int _busyWorkers;
    public int BusyWorkers
    {
        get
        {
            return _busyWorkers;
        }
        set
        {
            _busyWorkers = value;
            if(m_onWorkerChange != null) {
                m_onWorkerChange();
            }
        }
    }

    public const uint WORKER_COST = 4;
    public delegate void onWorkerChange();
    private event onWorkerChange m_onWorkerChange = null;

    private WorkerManager()
    {
        Workers = 1;
        BusyWorkers = 0;
        TimeManager timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
        timeManager.addListenerToMonthChange(payWorkers);
    }
    
    public bool areWorkersAvailable(int numWorkers)
    {
        return ((_workers - _busyWorkers) >= numWorkers);
    }

    public int FreeWorkers {
        get
        {
            return (_workers - _busyWorkers);
        }
    }

    public uint getWorkerCost()
    {
        return WORKER_COST;
    }

    public void addOnWorkerChangeListener(onWorkerChange fun)
    {
        m_onWorkerChange += fun;
    }

    public void removeOnWorkerChangeListener(onWorkerChange fun)
    {
        m_onWorkerChange -= fun;
    }

    public int calculateMonthlyMoney()
    {
        return (Workers - 1) * (int)getWorkerCost();
    }

    public void hireWorker()
    {
        Workers = Workers + 1;
    }

    public void payWorkers()
    {
        UserDataManager.GetInstance().gold.espendGold(calculateMonthlyMoney());
    }

    public void fireWorker()
    {
        Workers = Workers - 1;
		//pagar el finiquito
		TimeManager tm = GameObject.Find("Logic").GetComponent<TimeManager>();
		UserDataManager.GetInstance().gold.espendGold(WORKER_COST * tm.getCurrentDay () / (float) tm.getDaysOfAMonth(tm.getCurrentMonth()));
    }

    public void save(WorkerManagerData workerManagerData)
    {
        workerManagerData.BusyWorkers = BusyWorkers;
        workerManagerData.Workers = Workers;
    }

    public void load(WorkerManagerData workerManagerData)
    {
        BusyWorkers = workerManagerData.BusyWorkers;
        Workers = workerManagerData.Workers;
    }
}
