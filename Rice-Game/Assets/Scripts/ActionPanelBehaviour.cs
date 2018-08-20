using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class ActionPanelBehaviour : Panel
{
    private Text m_description;
    private Text m_name;
    private Text m_remainingDays;
    private List<Text> m_requirements = new List<Text>();
    private const int MAX_REQUIREMENTS = 8;
    private const string TICK_IMG_PATH = "Textures/UI/common/tick";
    private const string WRONG_IMG_PATH = "Textures/UI/common/wrong";
    private Button m_doButton;
    private Button m_backButton;
    private Button m_nextButton;
    private Image m_calendar;
    private List<MenuAction> m_actions;
    private int m_currentAction;
    private List<int> m_currentObjectList;
    private bool b_areEnoughWorkers;
    private bool b_haveAllObjectsRequired;
	private RiceTerrainTile m_riceTerrain;

    void Awake()
    {
        initBase();
    }

    void Start()
    {
        init();
    }

    void OnEnable()
    {
        init();
    }

    private void init()
    {
        if (m_description == null) m_description = GameObject.Find("ActionDescription").GetComponent<Text>();
        if (m_calendar == null) m_calendar = GameObject.Find("Calendar").GetComponent<Image>();
        if (m_remainingDays == null) m_remainingDays = GameObject.Find("PhaseRemainingDays").GetComponent<Text>();
        if (m_name == null) m_name = GameObject.Find("ActionNameTitle").GetComponent<Text>();
        if (m_requirements.Count == 0) {
            for (int i = 1; i <= MAX_REQUIREMENTS; ++i) {
                m_requirements.Add(GameObject.Find("RequirementInfoPanel_" + i).GetComponent<Text>());
            }
        }
        if (m_doButton == null) m_doButton = GameObject.Find("DoButton").GetComponent<Button>();
        if (m_backButton == null) m_backButton = GameObject.Find("BackButton").GetComponent<Button>();
		if (m_nextButton == null) m_nextButton = GameObject.Find("NextButton").GetComponent<Button>();
    }

    public void setRiceTerrain(RiceTerrainTile riceTerrain)
	{
		m_currentAction = 0;
		m_riceTerrain = riceTerrain;

		GameObject.Find("Logic").GetComponent<TimeManager>().addListerToDayChange(happyNewDay);
		updateActions ();
    }

	private void updateActions()
	{
		if (m_riceTerrain.isDisabled()) {
			kill();
			return;
		}
		List<MenuAction> newActions = (List<MenuAction>)ActionManager.GetInstance().getRiceTerrainActions(m_riceTerrain);
		// si no hay acciones se cierra el panel
		if (newActions.Count == 0) {
			kill();
			return;
		}
		if (m_actions != null) {
			// intentamos mantener la accion anterior (si existe)
			int lastID = m_actions [m_currentAction].id;
			m_currentAction = 0;
			for (int i = 0; i < newActions.Count; ++i) {
				if (newActions [i].id == lastID) {
					m_currentAction = i;
					break;
				}
			}
		}

		m_actions = newActions;
		updateAction();
		updateButtons();
	}

	private void updateAction()
	{
		m_currentObjectList = m_actions[m_currentAction].getListOfObjectsRequired();
		updateObjectRequired();
		updateWorkersRequired();
		updateActionDescription();
		updateCalendar();
	}

	void updateButtons()
	{
		m_backButton.interactable = m_currentAction > 0;
		m_nextButton.interactable = m_currentAction < m_actions.Count - 1;
	}

    public void actionClicked()
    {
        if (!b_haveAllObjectsRequired) {
            openBuyMenu();
        }
        else if (!b_areEnoughWorkers) {
            openHireMenu();
        }
        else {
            const int ID_SEND_HARVEST_TRAD = 17;
            const int ID_SEND_HARVEST_MODERN = 29;
            if (m_actions[m_currentAction].id != ID_SEND_HARVEST_MODERN && m_actions[m_currentAction].id != ID_SEND_HARVEST_TRAD) {
                m_actions[m_currentAction].doMenuAction();
            }
            else {
                int chunk = ((ChunkAction)m_actions[m_currentAction]).chunk;
                uint riceInChunk = WorldTerrain.GetInstance().getRiceChunkProduction(chunk);
                if (!BuildingsManager.GetInstance().isBuilded(BUILDINGS.PLANTA)) {
                    Building_Trill trill = (Building_Trill)BuildingsManager.GetInstance().getBuilding(BUILDINGS.TRILL);
                    uint currentFreeCapacity = trill.getCurrentFreeCapacity();
                    if(currentFreeCapacity >= riceInChunk) {
                        m_actions[m_currentAction].doMenuAction();
                    }
                    else {
                        //uint possibleLost = riceInChunk - currentFreeCapacity;
                        GameObject panelTemplate = Resources.Load("Prefabs/RiceOverflowLostFocusLayer") as GameObject;
                        GameObject panelInstance = Instantiate(panelTemplate);
                        GameObject panel = panelInstance.transform.FindChild("RiceOverflowPanel").gameObject;
                        panel.GetComponent<RiceOverflowPanelBehaviour>().init((MenuAction)m_actions[m_currentAction]);
                    }
                }
                else {
                    Building_Planta planta = (Building_Planta)BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTA);
                    uint currentFreeCapacity = planta.getCurrentFreeCapacity();
                    if (currentFreeCapacity >= riceInChunk) {
                        m_actions[m_currentAction].doMenuAction();
                    }
                    else {
                        //uint possibleLost = riceInChunk - currentFreeCapacity;
                        GameObject panelTemplate = Resources.Load("Prefabs/RiceOverflowLostFocusLayer") as GameObject;                        
                        GameObject panelInstance = Instantiate(panelTemplate);
                        GameObject panel = panelInstance.transform.FindChild("RiceOverflowPanel").gameObject;
                        panel.GetComponent<RiceOverflowPanelBehaviour>().init((MenuAction)m_actions[m_currentAction]);
                    }
                }
            }
            kill();
        }
    }

    private void updateActionDescription()
    {
        m_description.text = m_actions[m_currentAction].info;
        m_name.text = m_actions[m_currentAction].title;
        if (!b_haveAllObjectsRequired) {
            m_doButton.transform.FindChild("DoButtonText").gameObject.GetComponent<Text>().text = Dictionary.getString("SHOP");
        }
        else if (!b_areEnoughWorkers) {
            m_doButton.transform.FindChild("DoButtonText").gameObject.GetComponent<Text>().text = Dictionary.getString("HIRE");
        }
        else {
            m_doButton.transform.FindChild("DoButtonText").gameObject.GetComponent<Text>().text = Dictionary.getString("DO");
        }
        if(((ChunkAction)m_actions[m_currentAction]).addAsDone) {
            m_calendar.gameObject.SetActive(true);
            m_remainingDays.gameObject.SetActive(true);
        }
        else {
            m_calendar.gameObject.SetActive(false);
            m_remainingDays.gameObject.SetActive(false);
        }
    }

	private void updateObjectRequired ()
	{
		b_haveAllObjectsRequired = true;
		int i;
		for (i = 0; i < m_currentObjectList.Count; ++i) {
			m_requirements [i + 1].gameObject.SetActive (true);
			int idObjectSelected = m_currentObjectList [i];
			RiceObject objectSelected = RiceObjectsManager.GetInstance ().getRiceObject (idObjectSelected);
			m_requirements [i + 1].text = objectSelected.title;
			bool isCurrentObjectAvailable = RiceObjectsManager.GetInstance ().isThereAvailable (m_currentObjectList [i]);
			b_haveAllObjectsRequired = b_haveAllObjectsRequired && isCurrentObjectAvailable;
			string img = isCurrentObjectAvailable ? TICK_IMG_PATH : WRONG_IMG_PATH;                
			m_requirements [i + 1].transform.FindChild ("Tick").gameObject.GetComponent<Image> ().sprite =
                    Resources.Load<Sprite> (img);
		}
		for (; i < MAX_REQUIREMENTS - 1; ++i) {
			m_requirements [i + 1].gameObject.SetActive (false);
		}
	}

    private void updateWorkersRequired()
    {
        int freeWorkers = WorkerManager.GetInstance().FreeWorkers;
        int workersNeeded = m_actions[m_currentAction].workersNeeded;
		m_requirements[0].text = workersNeeded.ToString() + "/" + freeWorkers.ToString();
		b_areEnoughWorkers = freeWorkers >= workersNeeded;
        string img = b_areEnoughWorkers ? TICK_IMG_PATH : WRONG_IMG_PATH;
        m_requirements[0].transform.FindChild("Tick").gameObject.GetComponent<Image>().sprite =
            Resources.Load<Sprite>(img);
    }

    public override void kill()
    {
		GameObject.Find("Logic").GetComponent<TimeManager>().removeListenerToDayChange(happyNewDay);
		GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().actionPanelKilled();
		Destroy(gameObject);
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

    public void SelectPageDir(MenuDirButtBehaviour.DIR dir)
    {
        if (dir == MenuDirButtBehaviour.DIR.LEFT) {
            --m_currentAction;
        }
        else if (dir == MenuDirButtBehaviour.DIR.RIGHT) {
            ++m_currentAction;
        }
        
        updateAction();
        updateButtons();
    }

    public void happyNewDay()
	{
		updateActions ();
    }

    private void updateCalendar()
    {
        if (m_remainingDays != null) {
            TypeFase currentPhase = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().getCurrentPhase();
            if (currentPhase != TypeFase.NONE) {
                m_remainingDays.gameObject.SetActive(true);
                m_calendar.gameObject.SetActive(true);
                int remainingDays = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().getRemainingDaysOfCurrentPhase();
                
                m_remainingDays.text = Dictionary.getString("WILL_TAKE") + " " + ((ChunkAction)m_actions[m_currentAction]).duration / 24 + 
                    " " + Dictionary.getString("DAYS") + "." + Dictionary.getString("HAVE_LEFT") + " " + remainingDays.ToString() + " " + Dictionary.getString("DAYS_TO_DO_IT");
            }
            else {
                m_remainingDays.gameObject.SetActive(false);
                m_calendar.gameObject.SetActive(false);
            }
        }
    }

	public void openBuyMenu()
	{
		GameObject parent = GameObject.Find("UICanvas");
		Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
		foreach (Component t in trs) {
			if (t.name.Equals("BuyMenu")) {
				t.gameObject.GetComponent<BuyMenu>().selectObjectTab();
				t.gameObject.SetActive(true);
				this.kill();
			}
			else if (t.tag.Equals("BuyMenuParent")) {
				t.gameObject.SetActive(true);
				//t.gameObject.GetComponent<BuyMenu>().selectObjectTab();
				//this.kill();
			}
		}
	}

    private void openHireMenu()
    {
        GameObject hirePanel = Resources.Load("Prefabs/WorkerRecruitmentlLostFocusLayer") as GameObject;
        Instantiate(hirePanel);
        kill();
    }
}
