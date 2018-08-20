using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using MyUtils.Pair;

public class PlantaPanelBehaviour : Panel
{
    private Text m_buildingRiceStored = null;
    private Text m_buldingWork = null;

    private bool b_init = false;

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
        if (m_buildingRiceStored == null) m_buildingRiceStored = GameObject.Find("RiceProductionText").GetComponent<Text>();
        if (m_buldingWork == null) m_buldingWork = GameObject.Find("DailyRiceProductionText").GetComponent<Text>();
        updateTexts();
        GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>().addListerToDayChange(happyNewDay);
        b_init = true;
    }

    private void updateTexts()
    {
        Building building = BuildingsManager.GetInstance().getBuilding(BUILDINGS.PLANTA);
        List<Pair<string, string>> specificInfo = building.getSpecificBuildingInfo();
        m_buildingRiceStored.text = specificInfo[0].First + specificInfo[0].Second;
        m_buldingWork.text = building.getWorkInfo() + " " + "per dia";
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

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }
    
    public void happyNewDay()
    {
        if (b_init)
        {
            updateTexts();
        }
    }
    
}
