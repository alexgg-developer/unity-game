using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorkerCounterBehaviour : MonoBehaviour 
{
    private Text m_workerCounterText = null;

	private TutorialManager _tutMan;

	void Awake()
	{

	}

	void Start () 
	{
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
        if (m_workerCounterText == null) {
            GameObject workerCounter = GameObject.Find("WorkerCounter");
            m_workerCounterText = workerCounter.transform.FindChild("WorkerCounterText").GetComponent<Text>();
        }
        WorkerManager.GetInstance().addOnWorkerChangeListener(updateCounter);
        TimeManager timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
        timeManager.addListenerToMonthChange(updateCounter);
        updateCounter();
	}

    private void updateCounter()
    {
        int freeWorkers = WorkerManager.GetInstance().FreeWorkers;
        int workers = WorkerManager.GetInstance().Workers;
        m_workerCounterText.text = freeWorkers.ToString() + "/" + workers.ToString();
    }

    public void openHirePanel()
    {
		if (_tutMan.getState () == TutorialManager.STATES.END) {
			GameObject hirePanel = Resources.Load ("Prefabs/WorkerRecruitmentlLostFocusLayer") as GameObject;
			Instantiate (hirePanel);
		}
    }
    

	
}
