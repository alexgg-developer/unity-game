using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class InvestigationMenu : Panel {
	
	public Sprite img;
	public GameObject buttonTemplate;

	private const string BCKG_SELECTED = "Textures/UI/shop/bgObjetoComprarSelected";
	private const string BCKG_UNSELECTED = "Textures/UI/shop/bgObjetoComprar";
	
	private List<Button> buttonList;
	
	private Text descriptionText = null;
	private Text nameText = null;
	private Text priceText = null;
	private Button investigationButton = null;
	private Text investigationButtonText = null;
	private RectTransform investigationLeftPanel = null;
	private RectTransform investigationCenterPanel = null;
	private RectTransform investigationRightPanel = null;
	
	private Button leftButt=null, rightButt=null;

	private int selectedId;

	private const int itemsPerPage = 3;
	private int selectedPage, numOfPages;

	private TAB indexTabSelected = TAB.AVAILABLE;
	public enum TAB { AVAILABLE, PROGRESS, COMPLETED};
	public enum PanelPosition { LEFT, CENTER, RIGHT};

	private TutorialManager _tutMan;

	void Start () {
		init ();
		
		//investigationButton.onClick.AddListener (() => ExecuteInvestigation ());
        GameObject.Find("Logic").GetComponent<TimeManager>().addListenerToYearChange(newYear);

    }

	void OnEnable() {
		init ();
		if(!Tutorial_ResearchMenu.init) {
			_tutMan.startTuto(new Tutorial_ResearchMenu());
        }
	}

	void init() {
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		if (descriptionText == null) descriptionText = GameObject.Find ("InvestigationDescription").GetComponent<Text>();
		if (nameText == null) nameText = GameObject.Find("InvestigationName").GetComponent<Text>();
		if (priceText == null) priceText = GameObject.Find("InvestigationPrice").GetComponent<Text>();
		if (investigationButton == null) investigationButton = GameObject.Find ("InvestigationButton").GetComponent<Button>();
		if (investigationButtonText == null) investigationButtonText = GameObject.Find ("InvestigationButtonText").GetComponent<Text>();
		if (investigationLeftPanel == null) investigationLeftPanel = GameObject.Find ("InvestigationItemsPanelLeft").GetComponent<RectTransform>();
		if (investigationCenterPanel == null) investigationCenterPanel = GameObject.Find("InvestigationItemsPanelCenter").GetComponent<RectTransform>();
		if (investigationRightPanel == null) investigationRightPanel = GameObject.Find("InvestigationItemsPanelRight").GetComponent<RectTransform>();

		if (leftButt == null) leftButt = GameObject.Find ("InvestigationLeftDir").GetComponent<Button>();
		if (rightButt == null) rightButt = GameObject.Find ("InvestigationRightDir").GetComponent<Button>();
		
		selectedPage = 0;
		selectedId = -1;
		indexTabSelected = TAB.AVAILABLE;

		UpdateInvestigationList(true);
		updateDirButts();
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			gameObject.SetActive(false);

		switch (indexTabSelected) {
		case TAB.AVAILABLE:
			updateAvailableInfo ();
			break;
		case TAB.PROGRESS:
			updateProgressInfo ();
			break;
		case TAB.COMPLETED:
			updateCompletedInfo ();
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

	void updateAvailableInfo(){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getAvailableInvestigations ();
		if (investigations.Count == 0) {
			investigationButton.interactable = false;

            investigationButtonText.text = Dictionary.getString("ALL_RESEARCHED");
			nameText.text = "";
			priceText.text = "";
			descriptionText.text = "";
		} else {
			Investigation selectedInvest = im.getInvestigation (selectedId);
			nameText.text = selectedInvest.getName ();
			priceText.text = " " + Dictionary.getString("COST") + " " + selectedInvest.getCost ();
			descriptionText.text = selectedInvest.getDesc();
			investigationButton.interactable = true;
			investigationButtonText.text = Dictionary.getString("RESEARCH");
			//selectedInvest.printInfo ();
		}
	}

	void updateProgressInfo(){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getProgressInvestigations ();
		if (investigations.Count == 0) {
			investigationButton.interactable = false;

            investigationButtonText.text = Dictionary.getString("NOTHING_UNDER_RESEARCH");
			nameText.text = "";
			priceText.text = "";
			descriptionText.text = "";
		} else {
			Investigation selectedInvest = im.getInvestigation (selectedId);
			nameText.text = selectedInvest.getName ();
			priceText.text = " " + Dictionary.getString("COST") + " " + selectedInvest.getCost ();
			descriptionText.text = selectedInvest.getDesc();
			investigationButton.interactable = false;
            investigationButtonText.text = Dictionary.getString("RESEARCH_UNDERWAY");

        }
	}

	void updateCompletedInfo(){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getCompletedInvestigations ();
		if (investigations.Count == 0) {
			investigationButton.interactable = false;
            investigationButtonText.text = Dictionary.getString("NO_RESEARCH_COMPLETED");

            nameText.text = "";
			priceText.text = "";
			descriptionText.text = "";
		} else {
			Investigation selectedInvest = im.getInvestigation (selectedId);
			nameText.text = selectedInvest.getName ();
			priceText.text = " " + Dictionary.getString("COST") + " " + selectedInvest.getCost ();
			descriptionText.text = selectedInvest.getDesc();
			investigationButton.interactable = false;
            investigationButtonText.text = Dictionary.getString("RESEARCH_COMPLETED");

        }
	}

	// Funcion para actualizar la lista de la compra
	void UpdateInvestigationList(bool changeOfTab){

		switch (indexTabSelected) {
		case TAB.AVAILABLE:
			updateAvailableList (changeOfTab);
			break;
		case TAB.PROGRESS:
			updateProgressList (changeOfTab);
			break;
		case TAB.COMPLETED:
			updateCompletedList (changeOfTab);
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

	void updateAvailableList(bool changeOfTab){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getAvailableInvestigations ();

		if (changeOfTab && investigations.Count > 0) {
			selectedId = investigations[0].getID();
		}

		showInvestigationsList (investigations);
	}

	void updateProgressList(bool changeOfTab){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getProgressInvestigations ();

		if (changeOfTab && investigations.Count > 0) {
			selectedId = investigations[0].getID();
		}
		else if (investigations.Count == 0)
			selectedId = -1;

		showInvestigationsList (investigations);
	}

	void updateCompletedList(bool changeOfTab){
		InvestigationManager im = InvestigationManager.GetInstance ();
		List<Investigation> investigations = im.getCompletedInvestigations ();

		if (changeOfTab && investigations.Count > 0) {
			selectedId = investigations [0].getID();
		} 
		else if (investigations.Count == 0)
			selectedId = -1;

		showInvestigationsList (investigations);
	}

	void showInvestigationsList(List<Investigation> investigations){
		numOfPages = Convert.ToInt32 (Math.Ceiling (investigations.Count / (float)itemsPerPage));
		selectedPage = Math.Min(selectedPage, numOfPages-1);
		selectedPage = Math.Max(selectedPage, 0);

		int initialInvestigation = itemsPerPage * selectedPage;
		for (int i = 0; i < itemsPerPage; ++i) {
			if (initialInvestigation + i < investigations.Count) {
				Investigation investigationToAdd = investigations [initialInvestigation + i];
				switch (i) {
				case 0:
					changePanel (investigationLeftPanel, investigations [initialInvestigation + i].getID(), investigationToAdd.getImg (), PanelPosition.LEFT);
					break;
				case 1:
					changePanel (investigationCenterPanel, investigations [initialInvestigation + i].getID(), investigationToAdd.getImg (), PanelPosition.CENTER);
					break;
				case 2:
					changePanel (investigationRightPanel, investigations [initialInvestigation + i].getID(), investigationToAdd.getImg (), PanelPosition.RIGHT);
					break;
				}
			} 
			else {
				switch (i) {
				case 0:
					changePanel (investigationLeftPanel, -1, "", PanelPosition.LEFT);
					break;
				case 1:
					changePanel (investigationCenterPanel, -1, "", PanelPosition.CENTER);
					break;
				case 2:
					changePanel (investigationRightPanel, -1, "", PanelPosition.RIGHT);
					break;
				}
			}
		}
	}

	private void changePanel(RectTransform panel, int id, string imgPath, PanelPosition panelPosition)
	{
		if (id == -1) {
			panel.gameObject.SetActive (false);
			return;
		}
		else panel.gameObject.SetActive (true);
		Image objectImage = panel.transform.FindChild("ObjectImage").GetComponent<Image>();
		if (imgPath != null) {
			objectImage.sprite = Resources.Load<Sprite>(imgPath);
		}
		panel.GetComponent<Button>().GetComponent<InvestigationPanelButton>().id = id;
		panel.GetComponent<Button>().GetComponent<InvestigationPanelButton>().PanelPosition = panelPosition;
		if(id == selectedId) {
			changePanelToSelected(panel);
		}        
		else {
			changePanelToUnselected(panel);
		}
	}

	override public void actionClicked (int id) {
		selectedId = id;
		Debug.Log ("SelectedId=" + selectedId); // esta meirdo no hace nada
	}

	public void investigationClicked(int id, PanelPosition position)
	{
		if (id != selectedId) {
			selectedId = id;
			//Debug.Log ("SelectedId=" + selectedId);
			switch (position) {
			case PanelPosition.LEFT:
				changePanelToSelected(investigationLeftPanel);
				changePanelToUnselected(investigationCenterPanel);
				changePanelToUnselected(investigationRightPanel);
				break;
			case PanelPosition.CENTER:
				changePanelToUnselected(investigationLeftPanel);
				changePanelToSelected(investigationCenterPanel);
				changePanelToUnselected(investigationRightPanel);
				break;
			case PanelPosition.RIGHT:
				changePanelToUnselected(investigationLeftPanel);
				changePanelToUnselected(investigationCenterPanel);
				changePanelToSelected(investigationRightPanel);
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

	public void ExecuteInvestigation(){
		InvestigationManager im = InvestigationManager.GetInstance ();
		int investigationPrice = im.getInvestigation (selectedId).getCost ();
		UserDataManager.GetInstance ().gold.espendGold (investigationPrice);
		im.startInvestigation (selectedId);
		UpdateInvestigationList (true);
		updateAvailableInfo ();
		//UpdateinvestigationList (); //--
		//updatePageInfo (); //--
	}

	public void SelectPageDir(MenuDirButtBehaviour.DIR dir) {
		if (dir == MenuDirButtBehaviour.DIR.LEFT) {
			--selectedPage;
		} else if (dir == MenuDirButtBehaviour.DIR.RIGHT) {
			++selectedPage;
		}
		UpdateInvestigationList (false);
		updateDirButts ();
	}

	void updateDirButts() {
		leftButt.interactable = selectedPage > 0;
		rightButt.interactable = selectedPage < numOfPages-1;
	}

	public void changeOfTab(TAB index)
	{
		if (index != indexTabSelected) {
			indexTabSelected = index;
			UpdateInvestigationList (true);
			updateDirButts ();
		}
	}

	public void newYear(){
		UpdateInvestigationList (false);
	}

}

