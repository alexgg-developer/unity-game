using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public enum TERRAIN_TOOLS {/*NULL=0*/ CANAL=1, RICE_CHUNK, DELETE};

public class BuyMenu : Panel
{
    private const string BCKG_SELECTED = "Textures/UI/shop/bgObjetoComprarSelected";
    private const string BCKG_UNSELECTED = "Textures/UI/shop/bgObjetoComprar";
    private const int HORSE_ID = 2;

	private Text descriptionText=null;
    private Text nameText = null;
    private Text priceText = null;
    private Button buyButton=null;
	private Text buyButtonText=null;
	private RectTransform buyLeftPanel = null;
    private RectTransform buyCenterPanel = null;
    private RectTransform buyRightPanel = null;
	private SideMenu sideMenu = null;

    private Button leftButt=null, rightButt=null;

	private int selectedId;

	private const int itemsPerPage = 3;
	private int selectedPage, numOfPages;

	private TAB indexTabSelected;
	public enum TAB { BUILDINGS, OBJECTS, TERRAIN_TOOLS};
    public enum PanelPosition { LEFT, CENTER, RIGHT};
    //private List<BUILDINGS> m_buildingsID = new List<BUILDINGS>();

	private TutorialManager _tutMan;

	void Awake () {
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		init ();
	}

	void OnEnable() {
		_tutMan.eventDone (TutorialManager.EVENTS.MENU_BUY);
		init();
	}

	override public void kill() {
		Debug.Log ("buy panel try to kill");
		if (_tutMan.getState () != TutorialManager.STATES.TutInfoHouse5 &&
			_tutMan.getState () != TutorialManager.STATES.TutPlantell_NO &&
			_tutMan.getState () != TutorialManager.STATES.TutBuildingsB1_NO &&
			_tutMan.getState () != TutorialManager.STATES.TutBuildingsB2_NO &&
			_tutMan.getState () != TutorialManager.STATES.TutBuildingsB3_NO ) {
			gameObject.transform.parent.gameObject.SetActive(false);
		}
	}

	void init() {
		if (descriptionText == null) descriptionText = GameObject.Find ("BuyDescription").GetComponent<Text>(); //transform.find? +++++++
        if (nameText == null) nameText = GameObject.Find("BuyName").GetComponent<Text>();
        if (priceText == null) priceText = GameObject.Find("BuyPrice").GetComponent<Text>();
        if (buyButton == null) buyButton = GameObject.Find ("BuyButton").GetComponent<Button>();
		if (buyButtonText == null) buyButtonText = GameObject.Find ("BuyButtonText").GetComponent<Text>();
		if (buyLeftPanel == null) buyLeftPanel = GameObject.Find ("BuyItemsPanelLeft").GetComponent<RectTransform>();
        if (buyCenterPanel == null) buyCenterPanel = GameObject.Find("BuyItemsPanelCenter").GetComponent<RectTransform>();
        if (buyRightPanel == null) buyRightPanel = GameObject.Find("BuyItemsPanelRight").GetComponent<RectTransform>();

        if (leftButt == null) leftButt = GameObject.Find ("BuyLeftDir").GetComponent<Button>();
		if (rightButt == null) rightButt = GameObject.Find ("BuyRightDir").GetComponent<Button>();

		if (sideMenu == null) sideMenu = GameObject.Find ("SideMenu").GetComponent<SideMenu>();
		
		selectedPage = 0;
		indexTabSelected = TAB.OBJECTS;
		if (_tutMan.getState () == TutorialManager.STATES.TutInfoHouse4 ||
			_tutMan.getState () == TutorialManager.STATES.TutPlantell_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB1_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB2_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB3_NO) {
			indexTabSelected = TAB.BUILDINGS;
		}
		UpdateBuyList(true);
		updateDirButts();
	}

	void Update () {
		/*
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			gameObject.SetActive(false);
		*/

		switch (indexTabSelected) {
		case TAB.BUILDINGS:
			updateBuildingInfo ();
			break;
		case TAB.OBJECTS:
			updateObjectInfo ();
			break;
		case TAB.TERRAIN_TOOLS:
			updateTerrainToolInfo ();
			break;
		}

		if (selectedPage == 0)
			leftButt.interactable = false;
		else
			leftButt.interactable = true;

		if (selectedPage >= numOfPages - 1)
			rightButt.interactable = false;
		else
			rightButt.interactable = true;
	}

	void updateBuildingInfo()
	{
		BuildingsManager bm = BuildingsManager.GetInstance ();
		if (selectedId == 0) {
			buyButton.interactable = false; 

            buyButtonText.text = Dictionary.getString("ALL_BUILDINGS_BOUGHT");
			nameText.text = "";
			priceText.text = " ";
			descriptionText.text = "";
		} else {
			Building selectedBuilding = bm.getBuilding ((BUILDINGS)selectedId);
            nameText.text = selectedBuilding.getInfo();
            priceText.text = " " + Dictionary.getString("COST") + ": " + selectedBuilding.getInitialCost();
            descriptionText.text = selectedBuilding.Description;
			buyButton.interactable = true;
			buyButtonText.text = Dictionary.getString("BUY");

			if (_tutMan.getState () == TutorialManager.STATES.TutInfoHouse5) {
				buyButton.interactable = selectedId == (int) BUILDINGS.CASA;
			}
			if (_tutMan.getState () == TutorialManager.STATES.TutPlantell_NO) {
				buyButton.interactable = selectedId == (int) BUILDINGS.PLANTER;
			}
			if (_tutMan.getState () == TutorialManager.STATES.TutBuildingsB1_NO) {
				buyButton.interactable = selectedId == (int) BUILDINGS.TRILL;
			}
			if (_tutMan.getState () == TutorialManager.STATES.TutBuildingsB2_NO) {
				buyButton.interactable = selectedId == (int) BUILDINGS.ERA;
			}
			if (_tutMan.getState () == TutorialManager.STATES.TutBuildingsB3_NO) {
				buyButton.interactable = selectedId == (int) BUILDINGS.SILO;
			}

            
			
		}
	}

	void updateObjectInfo()
	{
        RiceObject riceObject = RiceObjectsManager.GetInstance().getRiceObject(selectedId);
        nameText.text = riceObject.title;
        priceText.text = Dictionary.getString("COST") + ": " + riceObject.Cost.ToString();
        descriptionText.text = riceObject.info;
        buyButton.interactable = true;
        buyButtonText.text = Dictionary.getString("BUY");
        if(selectedId == HORSE_ID) {
			BuildingsManager bm = BuildingsManager.GetInstance ();
            Building_Stable stable = (Building_Stable) bm.getBuilding(BUILDINGS.ESTABLO);
			int numOfHorses = RiceObjectsManager.GetInstance ().getQuantityOfObject (selectedId);
			if ((!bm.isBuilded(BUILDINGS.ESTABLO) && numOfHorses >= 1) || (stable.getCurrentCapacity() <= numOfHorses)) {
                buyButton.interactable = false;
                buyButtonText.text = Dictionary.getString("NO_SPACE_LEFT_STABLE");
            }
        }
        else if(riceObject.NeedsGarage) {
            //Building_Garage garage = (Building_Garage)BuildingsManager.GetInstance().getBuilding(BUILDINGS.GARAJE);
            //if (garage.getCurrentCapacity() <= RiceObjectsManager.GetInstance().getTotalObjectsThatNeedGarage()) {
            //    buyButton.interactable = false;
            //    buyButtonText.text = Dictionary.getString("NO_SPACE_LEFT_GARAGE");
            //}
		}
		if (_tutMan.getState () == TutorialManager.STATES.TutInfoHouse5 ||
			_tutMan.getState () == TutorialManager.STATES.TutPlantell_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB1_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB2_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB3_NO ) {
			buyButton.interactable = false; 
		}
    }

	void updateTerrainToolInfo() {
		switch ((TERRAIN_TOOLS) selectedId) {
		    case TERRAIN_TOOLS.CANAL:
                nameText.text = Dictionary.getString("WATER_CANALS");
                descriptionText.text = Dictionary.getString("LET_BUILD_WATER_CANALS");
                buyButtonText.text = Dictionary.getString("START_BUILDING");
                priceText.text = Dictionary.getString("COST") + ": " + WorldTerrain.PRICE_Canal + " " +  Dictionary.getString("EACH_UNIT");
                buyButton.interactable = true;
			    break;
		    case TERRAIN_TOOLS.RICE_CHUNK:
			    nameText.text = Dictionary.getString("RICE_FIELD");
                descriptionText.text = Dictionary.getString("LET_BUILD_RICE_FIELDS");
                priceText.text = Dictionary.getString("COST") + ": " + WorldTerrain.PRICE_RiceChunk;
			    buyButton.interactable = true;
			    buyButtonText.text = Dictionary.getString("BUY");
                break;
		    case TERRAIN_TOOLS.DELETE:
			    nameText.text = Dictionary.getString("ERASE_TERRAIN");
                descriptionText.text = Dictionary.getString("LET_ERASE_RICE_FIELDS");
                buyButtonText.text = Dictionary.getString("START_ERASING");
                priceText.text = Dictionary.getString("COST") + ": " + WorldTerrain.PRICE_Clean + " " + Dictionary.getString("EACH_UNIT");
                buyButton.interactable = true;
			    break;
		    default:
			    nameText.text = "NO Terrain Tools";
			    buyButtonText.text = "NO Terrain Tools";
			    buyButton.interactable = false;
			    break;
		}
		if (_tutMan.getState () == TutorialManager.STATES.TutInfoHouse5 ||
			_tutMan.getState () == TutorialManager.STATES.TutPlantell_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB1_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB2_NO ||
			_tutMan.getState () == TutorialManager.STATES.TutBuildingsB3_NO ) {
			buyButton.interactable = false; 
		}
	}

	// Funcion para actualizar la lista de la compra
	void UpdateBuyList(bool changeOfTab){
		// CLEAR
		
        switch (indexTabSelected) {
		case TAB.BUILDINGS:
			updateBuildingList (changeOfTab);
			break;
		case TAB.OBJECTS:
			updateObjectList (changeOfTab);
			break;
		case TAB.TERRAIN_TOOLS:
			updateTerrainToolList ();
			break;
        }

        if (selectedPage == 0)
            leftButt.interactable = false;
        else
            leftButt.interactable = true;

        if (selectedPage >= numOfPages - 1)
            rightButt.interactable = false;
        else
            rightButt.interactable = true;
    }

	void updateBuildingList(bool changeOfTab)
	{
        List<BUILDINGS> buildingsIds = new List<BUILDINGS>();
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
        foreach (BUILDINGS buildingId in Enum.GetValues(typeof(BUILDINGS))) {
			if (buildingId != 0 && !buildingsMan.isBuilded(buildingId)) {
				if (buildingId != BUILDINGS.PLANTA || InvestigationManager.GetInstance().isInvestigated(INVESTIGATIONS_ID.PLANTA)) {
					buildingsIds.Add(buildingId);
				}
			}
        }
        if (changeOfTab) {
            selectedId = buildingsIds.Count > 0 ? (int)buildingsIds[0] : 0;
        }
        // UPDATE DATA
        numOfPages = Convert.ToInt32(Math.Ceiling(buildingsIds.Count / (float)itemsPerPage));
        selectedPage = Math.Min(selectedPage, numOfPages - 1);
        selectedPage = Math.Max(selectedPage, 0);
        showBuildingsSelectedPage(buildingsIds);
        //m_buildingsID = buildingsIds;
    }

    private void showBuildingsSelectedPage(List<BUILDINGS> buildingsID)
    {
		buyLeftPanel.gameObject.SetActive (false);
		buyCenterPanel.gameObject.SetActive (false);
		buyRightPanel.gameObject.SetActive (false);
        BuildingsManager buildingsMan = BuildingsManager.GetInstance();
        //int initialObject = Math.Min(itemsPerPage * selectedPage, buildingsID.Count - itemsPerPage);
		int initialObject = itemsPerPage * selectedPage;
		for (int i = 0; i < itemsPerPage && i < buildingsID.Count-initialObject; ++i) {
            Building buildingToAdd = buildingsMan.getBuilding(buildingsID[initialObject + i]);
            //int objectPrice = buildingToAdd.getInitialCost();
            /*
			string quantityText = "cap";
            if(buildingsMan.isBuilded(buildingsID[initialObject + i])) {
                quantityText = "comprat";
            }
            */
            switch (i) {
                case 0:
					buyLeftPanel.gameObject.SetActive (true);
                    changePanel(buyLeftPanel, (int)buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.LEFT, "");
                    break;
                case 1:
					buyCenterPanel.gameObject.SetActive (true);
                    changePanel(buyCenterPanel, (int)buildingsID[initialObject + i],  buildingToAdd.getImgPath(), PanelPosition.CENTER, "");
                    break;
                case 2:
					buyRightPanel.gameObject.SetActive (true);
                    changePanel(buyRightPanel, (int)buildingsID[initialObject + i], buildingToAdd.getImgPath(), PanelPosition.RIGHT, "");
                    break;
            }
        }
    }


    public void updateObjectList (bool changeOfTab)
	{		
        List<RiceObject> riceObjects = RiceObjectsManager.GetInstance().getRiceObjectsAvailable();
        if (changeOfTab) {
            selectedId = riceObjects[0].id;
        }        
        // UPDATE DATA
        numOfPages = Convert.ToInt32 (Math.Ceiling (riceObjects.Count / (float)itemsPerPage));
		selectedPage = Math.Min(selectedPage, numOfPages-1);
		selectedPage = Math.Max(selectedPage, 0);
        showObjectsSelectedPage(riceObjects);
		updateQuantities (riceObjects);
    }

    public void showObjectsSelectedPage(List<RiceObject> riceObjects)
	{
		buyLeftPanel.gameObject.SetActive (false);
		buyCenterPanel.gameObject.SetActive (false);
		buyRightPanel.gameObject.SetActive (false);
        int initialObject = itemsPerPage * selectedPage;
		for (int i = 0; i < itemsPerPage && i < riceObjects.Count-initialObject; ++i) {
            RiceObject riceObjectToAdd = riceObjects[initialObject + i];
            //int objectPrice = riceObjectToAdd.Cost;
            switch (i) {
                case 0:
					buyLeftPanel.gameObject.SetActive (true);
                    changePanel(buyLeftPanel, riceObjectToAdd.id, riceObjectToAdd.iconImgPath, PanelPosition.LEFT, "");
                    break;
                case 1:
					buyCenterPanel.gameObject.SetActive (true);
				    changePanel(buyCenterPanel, riceObjectToAdd.id, riceObjectToAdd.iconImgPath, PanelPosition.CENTER, "");
                    break;
                case 2:
					buyRightPanel.gameObject.SetActive (true);
				    changePanel(buyRightPanel, riceObjectToAdd.id, riceObjectToAdd.iconImgPath, PanelPosition.RIGHT, "");
                    break;
            }
        }
    }

	private void updateQuantities(List<RiceObject> riceObjects)
	{
		int initialObject = itemsPerPage * selectedPage;
		for (int i = 0; i < itemsPerPage && i < riceObjects.Count - initialObject; ++i) {
			RiceObject riceObjectToModify = riceObjects [initialObject + i];
			string freeObjects = RiceObjectsManager.GetInstance ().howManyFreeOfObject (riceObjectToModify.id).ToString ();
			string quantityObject = RiceObjectsManager.GetInstance ().getQuantityOfObject (riceObjectToModify.id).ToString ();
			string quantityText = String.Format ("{0}/{1}", freeObjects, quantityObject);
			switch (i) {
			case 0:
				changeQuantityOnPanel (buyLeftPanel, quantityText);
				break;
			case 1:
				changeQuantityOnPanel (buyCenterPanel, quantityText);
				break;
			case 2:
				changeQuantityOnPanel (buyRightPanel, quantityText);
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


    private void changePanel(RectTransform panel, int id, string imgPath, PanelPosition panelPosition, string quantityText)
    {
        Image objectImage = panel.transform.FindChild("ObjectImage").GetComponent<Image>();
        if (imgPath != null) {
            objectImage.sprite = Resources.Load<Sprite>(imgPath);
        }
        Transform childPanel = panel.transform.FindChild("Quantities");
        Text quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.text = quantityText;
        panel.GetComponent<Button>().GetComponent<ShopPanelButton>().id = id;
        panel.GetComponent<Button>().GetComponent<ShopPanelButton>().PanelPosition = panelPosition;
        if(id == selectedId) {
            changePanelToSelected(panel);
        }        
        else {
            changePanelToUnselected(panel);
        }
    }

	void updateTerrainToolList() {
		numOfPages = 1;
		selectedPage = 0;
		selectedId = 1;

		buyLeftPanel.gameObject.SetActive (true);
		buyCenterPanel.gameObject.SetActive (true);
		buyRightPanel.gameObject.SetActive (true);

        changePanel(buyLeftPanel, (int)TERRAIN_TOOLS.CANAL, "Textures/Menu/Terrain/addcanal", PanelPosition.LEFT, "");
        changePanel(buyCenterPanel, (int)TERRAIN_TOOLS.RICE_CHUNK, "Textures/Menu/Terrain/addchunk", PanelPosition.CENTER, "");
        changePanel(buyRightPanel, (int)TERRAIN_TOOLS.DELETE, "Textures/Menu/Terrain/deletecbhunks", PanelPosition.RIGHT, "");
    }

	override public void actionClicked (int id) {

        selectedId = id;
    }

    //object as a item, this is for the 3 categories
    public void objectClicked(int id, PanelPosition position)
    {
        if (id != selectedId) {
            selectedId = id;
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
    }

    private void changePanelToSelected(RectTransform panel)
    {
        panel.GetComponent<Image>().sprite = Resources.Load<Sprite>(BCKG_SELECTED);
    }

    private void changePanelToUnselected(RectTransform panel)
    {
        panel.GetComponent<Image>().sprite = Resources.Load<Sprite>(BCKG_UNSELECTED);
    }

    public void ExecuteBuy(){
		switch (indexTabSelected) {
		case TAB.BUILDINGS:
			executeBuildingBuy ();               
			break;
		case TAB.OBJECTS:
			executeObjectBuy ();
			updateObjectList (false);
			break;
		case TAB.TERRAIN_TOOLS:
			executeTerrainToolBuy ();
			break;
		}
	}

	void executeBuildingBuy()
	{
		BuildingsManager buildingMan = BuildingsManager.GetInstance ();
		int buildingPrice = buildingMan.getBuilding ((BUILDINGS)selectedId).getInitialCost ();
		if ((BUILDINGS) selectedId == BUILDINGS.CASA) {
			_tutMan.eventDone(TutorialManager.EVENTS.MENU_BUY_HOUSE);
		}
		if ((BUILDINGS) selectedId == BUILDINGS.PLANTER) {
			_tutMan.eventDone(TutorialManager.EVENTS.MENU_BUY_PLANTELL);
		}
		if ((BUILDINGS) selectedId == BUILDINGS.TRILL) {
			_tutMan.eventDone(TutorialManager.EVENTS.MENU_BUY_TRILL);
		}
		if ((BUILDINGS) selectedId == BUILDINGS.ERA) {
			_tutMan.eventDone(TutorialManager.EVENTS.MENU_BUY_ERA);
		}
		if ((BUILDINGS) selectedId == BUILDINGS.SILO) {
			_tutMan.eventDone(TutorialManager.EVENTS.MENU_BUY_SILO);
		}
		UserDataManager.GetInstance ().gold.espendGold (buildingPrice);
		GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>().startBuilding(TileTerrainType.BUILDING,(BUILDINGS)selectedId);
		UpdateBuyList (false);
		sideMenu.closeMenu ();
		//kill ();
		gameObject.transform.parent.gameObject.SetActive(false);
	}

	void executeObjectBuy()
	{
		RiceObject riceObject = RiceObjectsManager.GetInstance ().getRiceObject (selectedId);
		UserDataManager.GetInstance ().gold.espendGold (riceObject.Cost);
		RiceObjectsManager.GetInstance ().buyRiceObject (selectedId);
	}

	void executeTerrainToolBuy()
	{
		switch ((TERRAIN_TOOLS)selectedId) {
		case TERRAIN_TOOLS.CANAL:
			GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager> ().startBuilding (TileTerrainType.CANAL);
			if (!Tutorial_ConstrCanal.init) {
				TutorialManager tutMan =  GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
				tutMan.startTuto(new Tutorial_ConstrCanal());
			}
			break;
		case TERRAIN_TOOLS.RICE_CHUNK:
			UserDataManager.GetInstance ().gold.espendGold (WorldTerrain.PRICE_RiceChunk);
			GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager> ().startBuilding (TileTerrainType.RICE_TERRAIN);
			break;
		case TERRAIN_TOOLS.DELETE:
			GameObject.FindGameObjectWithTag ("Logic").GetComponent<LogicManager> ().startDeleting ();
			if (!Tutorial_ConstrDelete.init) {
				TutorialManager tutMan =  GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
				tutMan.startTuto(new Tutorial_ConstrDelete());
			}
			break;
		}
		sideMenu.closeMenu ();
		kill ();
	}

	public void SelectPageDir(MenuDirButtBehaviour.DIR dir) {
		if (dir == MenuDirButtBehaviour.DIR.LEFT) {
			--selectedPage;
		} else if (dir == MenuDirButtBehaviour.DIR.RIGHT) {
			++selectedPage;
		}
		UpdateBuyList (false);
		updateDirButts ();
	}

	

	void updateDirButts() {
		leftButt.interactable = selectedPage > 0;
		rightButt.interactable = selectedPage < numOfPages-1;
	}

	/*
    private void setQuantityTextVisible(bool isVisible)
    {
        Transform childPanel = buyLeftPanel.transform.FindChild("Quantities");
        Text quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.gameObject.SetActive(isVisible);

        childPanel = buyCenterPanel.transform.FindChild("Quantities");
        quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.gameObject.SetActive(isVisible);

        childPanel = buyRightPanel.transform.FindChild("Quantities");
        quantities = childPanel.gameObject.GetComponent<Text>();
        quantities.gameObject.SetActive(isVisible);
    }
    */

	public void changeOfTab(TAB index)
	{
		if (index != indexTabSelected) {
			indexTabSelected = index;
            selectedPage = 0;
            UpdateBuyList (true);
            updateDirButts ();
            if (index == TAB.TERRAIN_TOOLS) {
                //setQuantityTextVisible(false);
				if (!Tutorial_ConstructionMenu.init) {
					_tutMan.startTuto(new Tutorial_ConstructionMenu());
				}
            }
            else {
                //setQuantityTextVisible(true);
            }
		}
	}

	public void selectObjectTab()
	{
		indexTabSelected = TAB.OBJECTS;
	}

    public void selectUpgradeTab()
    {
        indexTabSelected = TAB.BUILDINGS;
    }
}
