using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class UpgradePanelBehaviour : Panel
{
    private const string BCKG_SELECTED = "Textures/UI/shop/bgObjetoComprarSelected";
    private const string BCKG_UNSELECTED = "Textures/UI/shop/bgObjetoComprar";

	private Text descriptionText=null;
    private Text nameText = null;
    private Text priceText = null;
    private Button buyButton=null;
	private Text buyButtonText=null;
	private RectTransform buyLeftPanel = null;
    private RectTransform buyCenterPanel = null;
    private RectTransform buyRightPanel = null;

    private Button leftButt=null, rightButt=null;
    
	private const int itemsPerPage = 3;
	private int selectedPage, numOfPages;

    public enum PanelPosition { LEFT, CENTER, RIGHT};

    private BUILDINGS m_selectedID;
    private List<BUILDINGS> m_buildingsID;

    void Start () {
		init ();
	}

	void OnEnable() {
		init();
	}

	void init() {
		if (descriptionText == null) descriptionText = GameObject.Find ("BuyDescription").GetComponent<Text>();
        if (nameText == null) nameText = GameObject.Find("BuyName").GetComponent<Text>();
        if (priceText == null) priceText = GameObject.Find("BuyPrice").GetComponent<Text>();
        if (buyButton == null) buyButton = GameObject.Find ("BuyButton").GetComponent<Button>();
		if (buyButtonText == null) buyButtonText = GameObject.Find ("BuyButtonText").GetComponent<Text>();
		if (buyLeftPanel == null) buyLeftPanel = GameObject.Find ("BuyItemsPanelLeft").GetComponent<RectTransform>();
        if (buyCenterPanel == null) buyCenterPanel = GameObject.Find("BuyItemsPanelCenter").GetComponent<RectTransform>();
        if (buyRightPanel == null) buyRightPanel = GameObject.Find("BuyItemsPanelRight").GetComponent<RectTransform>();

        if (leftButt == null) leftButt = GameObject.Find ("BuyLeftDir").GetComponent<Button>();
		if (rightButt == null) rightButt = GameObject.Find ("BuyRightDir").GetComponent<Button>();
		
		selectedPage = 0;
		m_selectedID = 0;

		buyButtonText.text = Dictionary.getString ("P_UPGR_UPGRADE");
		updateUpgradeList();
        updateBuildingInfo();
        updateDirButts();
	}

	void Update () {
        updateDirButts();
	}

	void updateBuildingInfo()
	{
		BuildingsManager bm = BuildingsManager.GetInstance ();
		if (m_selectedID == 0) {
			buyButton.interactable = false;
			//buyButtonText.text = "Tots els edificis millorats";
			descriptionText.text = "";
		} else {
			Building selectedBuilding = bm.getBuilding (m_selectedID);
            nameText.text = selectedBuilding.getInfo();
            if (selectedBuilding.hasNextLevel()) {
				priceText.text = " " + Dictionary.getString("P_UPGR_COST") + selectedBuilding.getNextLevelCost();
            }
            else {
				priceText.text = Dictionary.getString("P_UPGR_NO_MORE");
            }
			descriptionText.text = Dictionary.getString("P_UPGR_CAPACITY_NOW") + selectedBuilding.getCapacity1();
			if (selectedBuilding.getCapacity2 () > 0) descriptionText.text += " | " + selectedBuilding.getCapacity2 ();
            if (selectedBuilding.hasNextLevel()) {
				descriptionText.text += "\n" +Dictionary.getString("P_UPGR_CAPACITY_NEXT") + selectedBuilding.getNextLevelCapacity1();
				if (selectedBuilding.getNextLevelCapacity2 () > 0) descriptionText.text += " | " + selectedBuilding.getNextLevelCapacity2 ();
                buyButton.interactable = true;
                //buyButtonText.text = "Millorar";
                
            }
            else {
                buyButton.interactable = false;
            }
		}
    }

	
	// Funcion para actualizar la lista de la compra
	void updateUpgradeList(){
        // CLEAR
        List<BUILDINGS> buildingsIds = new List<BUILDINGS>();
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
        foreach (BUILDINGS buildingId in Enum.GetValues(typeof(BUILDINGS))) {
            if (buildingId != 0 && buildingsMan.isBuilded(buildingId)) {
				if (buildingId == BUILDINGS.ERA || buildingId == BUILDINGS.SILO || buildingId == BUILDINGS.TRILL) {
					if (buildingsMan.isBuilded (BUILDINGS.PLANTA)) {
						continue;
					}
				}
                buildingsIds.Add(buildingId);
            }
        }
        // UPDATE DATA
        if (m_selectedID == 0 && buildingsIds.Count > 0) {
            m_selectedID = buildingsIds[0];
        }
        numOfPages = Convert.ToInt32(Math.Ceiling(buildingsIds.Count / (float)itemsPerPage));
        selectedPage = Math.Min(selectedPage, numOfPages - 1);
        selectedPage = Math.Max(selectedPage, 0);
        showBuildingsSelectedPage(buildingsIds);
        m_buildingsID = buildingsIds;

        updateDirButts();
    }
    
    private void showBuildingsSelectedPage(List<BUILDINGS> buildingsID)
    {
		buyLeftPanel.gameObject.SetActive (false);
		buyCenterPanel.gameObject.SetActive (false);
		buyRightPanel.gameObject.SetActive (false);
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
		int initialObject = itemsPerPage * selectedPage;
		for (int i = 0; i < itemsPerPage && i < buildingsID.Count-initialObject; ++i) {
            Building buildingToAdd = buildingsMan.getBuilding(buildingsID[initialObject + i]);
            //int objectPrice = buildingToAdd.getInitialCost();
			string quantityText = Dictionary.getString("P_UPGR_LEVEL") + (buildingToAdd.getCurrentLevel() + 1);

            switch (i) {
                case 0:
					buyLeftPanel.gameObject.SetActive (true);
                    changePanel(buyLeftPanel, buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.LEFT, quantityText);
                    break;
                case 1:
					buyCenterPanel.gameObject.SetActive (true);
                    changePanel(buyCenterPanel, buildingsID[initialObject + i],  buildingToAdd.getImgPath(), PanelPosition.CENTER, quantityText);
                    break;
                case 2:
					buyRightPanel.gameObject.SetActive (true);
                    changePanel(buyRightPanel, buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.RIGHT, quantityText);
                    break;
            }
        }
    }  

	/*
    public void updateQuantities()  // +++++++++++++++++++++++++
    {
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
        int initialObject = Math.Min(itemsPerPage * selectedPage, m_buildingsID.Count - itemsPerPage);
        for (int i = 0; i < itemsPerPage; ++i) {
            Building buildingToModify = buildingsMan.getBuilding(m_buildingsID[initialObject + i]);
            string quantityText = "level: " + (buildingToModify.getCurrentLevel() + 1);
            switch (i) {
                case 0:
                    changeQuantityOnPanel(buyLeftPanel, quantityText);
                    break;
                case 1:
                    changeQuantityOnPanel(buyCenterPanel, quantityText);
                    break;
                case 2:
                    changeQuantityOnPanel(buyRightPanel, quantityText);
                    break;
            }
        }
    }

    private void changeQuantityOnPanel(RectTransform panel, string quantityText)
    {
        Transform childPanel = panel.transform.FindChild("Quantities");
        Text quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.text = quantityText;
    }
    */

    private void changePanel(RectTransform panel, BUILDINGS id, string imgPath, PanelPosition panelPosition, string quantityText)
    {
        Image objectImage = panel.transform.FindChild("ObjectImage").GetComponent<Image>();
        if (imgPath != null) {
            objectImage.sprite = Resources.Load<Sprite>(imgPath);
        }
         Transform childPanel = panel.transform.FindChild("Quantities");
        Text quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.text = quantityText;
        panel.GetComponent<Button>().GetComponent<UpgradePanelButton>().id = (int)id;
        panel.GetComponent<Button>().GetComponent<UpgradePanelButton>().PanelPosition = panelPosition;
        if(id == m_selectedID) {
            changePanelToSelected(panel);
        }        
        else {
            changePanelToUnselected(panel);
        }
    }
    
	override public void actionClicked (int id)
    {
        throw new NotImplementedException();
    }

    //object as a item, this is for the 3 categories
    public void objectClicked(int id, PanelPosition position)
    {
        if ((BUILDINGS)id != m_selectedID) {
            m_selectedID = (BUILDINGS)id;
            switch (position) {
                case PanelPosition.LEFT:
                    changePanelToSelected(buyLeftPanel);
                    changePanelToUnselected(buyCenterPanel);
                    changePanelToUnselected(buyRightPanel);
                    break;
                case PanelPosition.CENTER:
                    changePanelToUnselected(buyLeftPanel);
                    changePanelToSelected(buyCenterPanel);
                    changePanelToUnselected(buyRightPanel);
                    break;
                case PanelPosition.RIGHT:
                    changePanelToUnselected(buyLeftPanel);
                    changePanelToUnselected(buyCenterPanel);
                    changePanelToSelected(buyRightPanel);
                    break;
            }
        }
        updateBuildingInfo();
    }

    private void changePanelToSelected(RectTransform panel)
    {
        panel.GetComponent<Image>().sprite = Resources.Load<Sprite>(BCKG_SELECTED);
    }

    private void changePanelToUnselected(RectTransform panel)
    {
        panel.GetComponent<Image>().sprite = Resources.Load<Sprite>(BCKG_UNSELECTED);
    }
    
    public void ExecuteBuy()
    {        
        BuildingsManager buildingMan = BuildingsManager.GetInstance();
        int upgradePrice = buildingMan.getBuilding(m_selectedID).getNextLevelCost();
        UserDataManager.GetInstance().gold.espendGold(upgradePrice);
        buildingMan.upgrade(m_selectedID);
		updateUpgradeList();
        //updateQuantities();
		updateBuildingInfo();
    }    

	public void SelectPageDir(MenuDirButtBehaviour.DIR dir) {
		if (dir == MenuDirButtBehaviour.DIR.LEFT) {
			--selectedPage;
		} else if (dir == MenuDirButtBehaviour.DIR.RIGHT) {
			++selectedPage;
		}
		updateUpgradeList ();
		updateDirButts ();
	}	

	void updateDirButts() {
		leftButt.interactable = selectedPage > 0;
		rightButt.interactable = selectedPage < numOfPages-1;
	}

    public void selectBuilding(BUILDINGS buildingID)
    {
        if (buildingID != m_selectedID) {
            m_selectedID = buildingID;
            int index = m_buildingsID.IndexOf(buildingID);
            selectedPage = index / itemsPerPage;
            int positionSelected = index % itemsPerPage;
            buyLeftPanel.gameObject.SetActive(false);
            buyCenterPanel.gameObject.SetActive(false);
            buyRightPanel.gameObject.SetActive(false);
            BuildingsManager buildingsMan = BuildingsManager.GetInstance();
            int initialObject = itemsPerPage * selectedPage;
            for (int i = 0; i < itemsPerPage && i < m_buildingsID.Count - initialObject; ++i) {
                Building buildingToAdd = buildingsMan.getBuilding(m_buildingsID[initialObject + i]);
                //int objectPrice = buildingToAdd.getInitialCost();
				string quantityText = Dictionary.getString("P_UPGR_LEVEL") + (buildingToAdd.getCurrentLevel() + 1);

                switch (i) {
                    case 0:
                        buyLeftPanel.gameObject.SetActive(true);
                        changePanel(buyLeftPanel, m_buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.LEFT, quantityText);
                        break;
                    case 1:
                        buyCenterPanel.gameObject.SetActive(true);
                        changePanel(buyCenterPanel, m_buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.CENTER, quantityText);
                        break;
                    case 2:
                        buyRightPanel.gameObject.SetActive(true);
                        changePanel(buyRightPanel, m_buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.RIGHT, quantityText);
                        break;
                }
            }
            switch (positionSelected) {
                case 0:
                    changePanelToSelected(buyLeftPanel);
                    changePanelToUnselected(buyCenterPanel);
                    changePanelToUnselected(buyRightPanel);
                    break;
                case 1:
                    changePanelToUnselected(buyLeftPanel);
                    changePanelToSelected(buyCenterPanel);
                    changePanelToUnselected(buyRightPanel);
                    break;
                case 2:
                    changePanelToUnselected(buyLeftPanel);
                    changePanelToUnselected(buyCenterPanel);
                    changePanelToSelected(buyRightPanel);
                    break;
            }
        }
        updateBuildingInfo();
    }
}
