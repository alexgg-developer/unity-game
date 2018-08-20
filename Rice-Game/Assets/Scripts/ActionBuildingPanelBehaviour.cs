using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using MyUtils.Pair;

public class ActionBuildingPanelBehaviour : Panel
{
    private Text m_description;
    private Text m_name;
    private Text m_buldingWork;
    private Text m_info1, m_info2, m_info3;
    private Text m_quantity1, m_quantity2, m_quantity3;
    private Button m_doButton;
    //private Button m_backButton = null;
    //private Button m_nextButton = null;
    private Image m_clock;

    private List<MenuAction> m_actions;
    private int m_currentAction;
    private List<int> m_currentObjectList;
    //private bool b_isTerrain = false;
    //private bool b_areEnoughWorkers = true;
    //private bool b_haveAllObjectsRequired = true;
    private BUILDINGS m_buildingID;
    private bool b_init;

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
        if (m_description == null) m_description = GameObject.Find("ActionDescription").GetComponent<Text>();
        if (m_clock == null) m_clock = GameObject.Find("Clock").GetComponent<Image>();
        if (m_buldingWork == null) m_buldingWork = GameObject.Find("BuildingWorkText").GetComponent<Text>();
        if (m_name == null) m_name = GameObject.Find("BuildingNameTitle").GetComponent<Text>();
        if (m_info1 == null) m_info1 = GameObject.Find("InfoText_1").GetComponent<Text>();
        if (m_info2 == null) m_info2 = GameObject.Find("InfoText_2").GetComponent<Text>();
        if (m_info3 == null) m_info3 = GameObject.Find("InfoText_3").GetComponent<Text>();
        if (m_quantity1 == null) m_quantity1 = GameObject.Find("QuantityText_1").GetComponent<Text>();
        if (m_quantity2 == null) m_quantity2 = GameObject.Find("QuantityText_2").GetComponent<Text>();
        if (m_quantity3 == null) m_quantity3 = GameObject.Find("QuantityText_3").GetComponent<Text>();
        if (m_doButton == null) m_doButton = GameObject.Find("DoButton").GetComponent<Button>();

		m_currentAction = 0;
		m_buildingID = 0;
		b_init = false;

        GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>().addListerToDayChange(happyNewDay);
    }

    public void setActionsForBuilding(BUILDINGS buildingID)
    {
        List<MenuAction> actions = ActionManager.GetInstance().getBuildingActions(buildingID);
        m_buildingID = buildingID;
        m_actions = actions;
        updateAction();
        updateTexts();
        b_init = true;
    }

    private void updateTexts()
    {
        Building building = BuildingsManager.GetInstance().getBuilding(m_buildingID);
        List<Pair<string, string>> specificInfo = building.getSpecificBuildingInfo();
        m_description.text = building.Description;
        m_name.text = building.getInfo();
        if (specificInfo[0].First != "")
        {
            m_info1.gameObject.SetActive(true);
            m_info1.text = specificInfo[0].First;
        }
        else
        {
            m_info1.gameObject.SetActive(false);
        }

        if (specificInfo[1].First != "")
        {
            m_info2.gameObject.SetActive(true);
            m_info2.text = specificInfo[1].First;
        }
        else
        {
            m_info2.gameObject.SetActive(false);
        }

        if (specificInfo[2].First != "")
        {
            m_info3.gameObject.SetActive(true);
            m_info3.text = specificInfo[2].First;
        }
        else
        {
            m_info3.gameObject.SetActive(false);
        }

        if (specificInfo[0].Second != "")
        {
            m_quantity1.gameObject.SetActive(true);
            m_quantity1.text = specificInfo[0].Second;
        }
        else
        {
            m_quantity1.gameObject.SetActive(false);
        }

        if (specificInfo[1].Second != "")
        {
            m_quantity2.gameObject.SetActive(true);
            m_quantity2.text = specificInfo[1].Second;
        }
        else
        {
            m_quantity2.gameObject.SetActive(false);
        }

        if (specificInfo[2].Second != "")
        {
            m_quantity3.gameObject.SetActive(true);
            m_quantity3.text = specificInfo[2].Second;
        }
        else
        {
            m_quantity3.gameObject.SetActive(false);
        }
    }
    private void updateActionDescription()
    {
        Building building = BuildingsManager.GetInstance().getBuilding(m_buildingID);
        if (building.HasActions) {
            m_clock.gameObject.SetActive(true);
            m_buldingWork.gameObject.SetActive(true);
        }
        else {
            m_clock.gameObject.SetActive(false);
            m_buldingWork.gameObject.SetActive(false);
        }
        if (m_actions.Count > 0)
        {
            m_doButton.transform.FindChild("DoButtonText").gameObject.GetComponent<Text>().text = m_actions[m_currentAction].title;
            m_doButton.interactable = m_actions[m_currentAction].enabled;
        }
        else
        {
            m_doButton.gameObject.SetActive(false);
        }

    }
        
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 || (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())) {
            kill();
        }
    }

    public new void kill()
    {
        GameObject.Find("Logic").GetComponent<TimeManager>().removeListenerToDayChange(happyNewDay);
        GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().actionPanelKilled();
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void actionClicked()
    {
        m_actions[m_currentAction].doMenuAction();
        kill();
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
    }
    

    private void updateAction()
    {
        updateActionDescription();
        updateBuildingWork();
    }

    public void happyNewDay()
    {
        if (b_init)
        {
            setActionsForBuilding(m_buildingID);
        }
    }

    private void updateBuildingWork()
    {
        Building building = BuildingsManager.GetInstance().getBuilding(m_buildingID);
        string workInfo = building.getWorkInfo();
        m_buldingWork.text = workInfo + Dictionary.getString("PROCESSED_PER_DAY");
    }

    public void openUpgradeWindow()
    {
        GameObject parent = GameObject.Find("UICanvas");
        Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
        foreach (Component t in trs)
        {
            if (t.tag.Equals("UpgradeMenuParent"))
            {
                t.gameObject.SetActive(true);
            }
            else if (t.tag.Equals("UpgradeMenu")) {
                t.GetComponent<UpgradePanelBehaviour>().selectBuilding(m_buildingID);
            }
        }
        this.kill();
    }
}
