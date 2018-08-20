using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class CoopMenu : Panel
{

	private Text m_itemDescriptionText = null;
    private Text m_currentItemBonusText = null;
    private Text m_nextItemBonusText = null;

    //Portes x produits dels y que necessites per arribar al seguent objectiu
    
    private List<RectTransform> m_coopPanels = new List<RectTransform>();
    private List<Image> m_coopPanelImages = new List<Image>();
    public enum MilestoneType { EXPANSION, ECOLOGY, PRODUCTION, ANUAL_PRODUCTION, NUMBER_MILESTONES };
    
	private MilestoneType m_selectedMilestone;

    private const string IMG_EXPANSION_PATH = "Textures/UI/coop/chunk";
    private const string IMG_ECOLOGY_PATH = "Textures/UI/coop/eco";
    private const string IMG_PRODUCTION_PATH = "Textures/UI/coop/pro";
    private const string IMG_ANUAL_PRODUCTION_PATH = "Textures/UI/coop/time";


    void Start () {
		init ();
	}

	void OnEnable()
    {
		init();
		if (!Tutorial_Cope.init) {
			TutorialManager tutMan  = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
			tutMan.startTuto(new Tutorial_Cope());
		}
    }

	void init() {
		if (m_itemDescriptionText == null) m_itemDescriptionText = GameObject.Find ("ItemTitle").GetComponent<Text>();
        if (m_currentItemBonusText == null) m_currentItemBonusText = GameObject.Find("CurrentItemBonus").GetComponent<Text>();
        if (m_nextItemBonusText == null) m_nextItemBonusText = GameObject.Find("NextItemBonus").GetComponent<Text>();

        if(m_coopPanels.Count == 0) {
            for(int i = 1; i <= (int)MilestoneType.NUMBER_MILESTONES; ++i) {
                m_coopPanels.Add(GameObject.Find("CoopItemPanel_" + i).GetComponent<RectTransform>());
                m_coopPanelImages.Add(m_coopPanels[m_coopPanels.Count - 1].transform.FindChild("ObjectImage").GetComponent<Image>());
            }
        }

		m_selectedMilestone = MilestoneType.EXPANSION;

		updateCoopList();
        updateSelectedMilestone();

		bool ecoGrant = InvestigationManager.GetInstance ().isInvestigated (INVESTIGATIONS_ID.ECO_SUBVENCION);
		m_coopPanels [(int)MilestoneType.ECOLOGY].GetComponent<Button> ().interactable = ecoGrant;
	}

    private void updateCoopList()
    {
        int expansionLevel = Math.Max(CoopManager.GetInstance().getCurrentExpansionLevel(), 1);
        m_coopPanelImages[(int)MilestoneType.EXPANSION].sprite = Resources.Load<Sprite>(IMG_EXPANSION_PATH + expansionLevel);

        int ecologyLevel = Math.Max(CoopManager.GetInstance().getCurrentEcologyLevel(), 1);
        m_coopPanelImages[(int)MilestoneType.ECOLOGY].sprite = Resources.Load<Sprite>(IMG_ECOLOGY_PATH + ecologyLevel);

        int productionLevel = Math.Max(CoopManager.GetInstance().getCurrentTotalProductionLevel(), 1);
        m_coopPanelImages[(int)MilestoneType.PRODUCTION].sprite = Resources.Load<Sprite>(IMG_PRODUCTION_PATH + productionLevel);

        int anualProductionLevel = Math.Max(CoopManager.GetInstance().getCurrentAnualProductionLevel(), 1);
        m_coopPanelImages[(int)MilestoneType.ANUAL_PRODUCTION].sprite = Resources.Load<Sprite>(IMG_ANUAL_PRODUCTION_PATH + anualProductionLevel    );
    }

    private void updateSelectedMilestone()
    {
        string milestoneDescription = "";
        string currentBonus = "";
        string nextBonus = ""; 

		CoopManager coopMan = CoopManager.GetInstance ();
        switch (m_selectedMilestone) {
		case MilestoneType.EXPANSION:
			if (!coopMan.expansionBonusIsInMaxLvl ()) {
				milestoneDescription = Dictionary.getString ("CURRENTLY_YOU_HAVE") + " " + WorldTerrain.GetInstance ().getNumberOfChunks () + " " + Dictionary.getString ("OF") + " "
					+ CoopManager.GetInstance ().getNextExpansionGoal () + " " + Dictionary.getString ("FIELDS_CURRENT_GOAL");
			} else {
				milestoneDescription = Dictionary.getString ("MAX_BONUS");
			}
			currentBonus = Dictionary.getString ("CURRENT_BONUS") + ": " + coopMan.getCurrentExpansionBonus () + Dictionary.getString ("OBJECT_DISCOUNT");
			if (!coopMan.expansionBonusIsInMaxLvl ()) {
				nextBonus = Dictionary.getString ("NEXT_BONUS") + ": " + coopMan.getNextExpansionBonus () + Dictionary.getString ("OBJECT_DISCOUNT");
			}
			break;

		case MilestoneType.ECOLOGY:
			if (!Tutorial_Ecology.init) {
				TutorialManager tutMan  = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
				tutMan.startTuto(new Tutorial_Ecology());
			}

			if (!coopMan.ecologyBonusIsInMaxLvl()) {
				milestoneDescription = Dictionary.getString ("YOU_HAVE") + " " + coopMan.getLastYearEcologyScore ()
					+ " " + Dictionary.getString ("ECOLOGY_MILESTONE_DESC");
			} else {
				milestoneDescription = Dictionary.getString ("MAX_BONUS");
			}
			currentBonus = Dictionary.getString ("CURRENT_BONUS") + ": " + coopMan.getCurrentEcologyBonus () + " " + Dictionary.getString ("COINS_PER_YEAR");
			if (!coopMan.ecologyBonusIsInMaxLvl ()) {
				nextBonus = Dictionary.getString ("NEXT_BONUS") + ": " + coopMan.getNextEcologyBonus () + " " + Dictionary.getString ("COINS_PER_YEAR");
			}
			break;

		case MilestoneType.PRODUCTION:
			if (!coopMan.totalProductionBonusIsInMaxLvl()) {
				milestoneDescription = Dictionary.getString ("PRODUCTION_MILESTONE_DESC_1") + " " + UserDataManager.GetInstance ().getTotalRiceProduced () / 1000.0f
					+ " " + Dictionary.getString ("PRODUCTION_MILESTONE_DESC_2") + " " + coopMan.getNextTotalProductionGoal () + " t.";
			} else {
				milestoneDescription = Dictionary.getString ("MAX_BONUS");
			}
			currentBonus = Dictionary.getString ("CURRENT_BONUS") + ": " + coopMan.getCurrentTotalProductionBonus () + Dictionary.getString ("COINS_AUGMENTED");
			if (!coopMan.totalProductionBonusIsInMaxLvl ()) {    
				nextBonus = Dictionary.getString ("NEXT_BONUS") + ": " + coopMan.getNextTotalProductionBonus () + Dictionary.getString ("COINS_AUGMENTED");
			}
			break;

		case MilestoneType.ANUAL_PRODUCTION:
			if (!coopMan.anualProductionBonusIsInMaxLvl ()) {
				milestoneDescription = Dictionary.getString ("ANUAL_PRODUCTION_MILESTONE_DESC_1") + " " + coopMan.getLastYearProduction () / 1000.0f
				+ " " + Dictionary.getString ("ANUAL_PRODUCTION_MILESTONE_DESC_2") + " " + coopMan.getNextAnualProductionGoal () + " t.";
			} else {
				milestoneDescription = Dictionary.getString ("MAX_BONUS");
			}
			MyUtils.Pair.Pair<int, int> currentAnualProductionBonus = coopMan.getCurrentAnualProductionBonus ();
			currentBonus = Dictionary.getString ("CURRENT_BONUS") + ": " + currentAnualProductionBonus.First + Dictionary.getString ("ANUAL_PRODUCTION_BONUS_1") + " " + currentAnualProductionBonus.Second
				+ Dictionary.getString ("ANUAL_PRODUCTION_BONUS_2");
			MyUtils.Pair.Pair<int, int> nextAnualProductionBonus = coopMan.getNextAnualProductionBonus ();
			if (!coopMan.anualProductionBonusIsInMaxLvl ()) {    
				nextBonus = Dictionary.getString ("NEXT_BONUS") + ": " + nextAnualProductionBonus.First + Dictionary.getString ("ANUAL_PRODUCTION_BONUS_1") + " " + nextAnualProductionBonus.Second
					+ Dictionary.getString ("ANUAL_PRODUCTION_BONUS_2");
			}
			break;
        }

        m_itemDescriptionText.text = milestoneDescription;
        m_currentItemBonusText.text = currentBonus;
        m_nextItemBonusText.text = nextBonus;

        for (int i = 0; i < (int)MilestoneType.NUMBER_MILESTONES; ++i) {
            m_coopPanels[i].GetComponent<Image>().enabled = false;
        }
        m_coopPanels[(int)m_selectedMilestone].GetComponent<Image>().enabled = true;
    }

    void Update () {
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			gameObject.SetActive(false);
	}

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

    public void objectClicked(MilestoneType type)
    {
        m_selectedMilestone = type;
        updateSelectedMilestone();
    }
}
