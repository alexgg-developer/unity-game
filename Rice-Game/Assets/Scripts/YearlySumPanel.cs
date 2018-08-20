using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class YearlySumPanel : Panel
{
	override protected void initBase()
	{
		this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
	}
    void Start()
	{
		initBase ();
        init();
    }

    void init()
    {
		Text m_riceProductionNumberText = GameObject.Find("RiceProductionNumberText").GetComponent<Text>();
		Text m_riceSoldNumberText = GameObject.Find("RiceSoldNumberText").GetComponent<Text>();
		Text m_riceLostNumberText = GameObject.Find("RiceLostNumberText").GetComponent<Text>();
		Text m_grantEcologyNumberText = GameObject.Find("EcologyGrantNumberText").GetComponent<Text>();

		UserDataManager userData = UserDataManager.GetInstance ();
		uint anualProduction = userData.getRiceProducedThisYear();
		m_riceProductionNumberText.text = anualProduction.ToString ();

		WorldTerrain wt = WorldTerrain.GetInstance ();
		InvestigationManager invMan = InvestigationManager.GetInstance ();
		//uint riceOfChunksDisabled = (uint) wt.getNumberOfChunks() * RiceTerrainTile.MAX_RICE_UNITS * WorldTerrain.RICE_CHUNK_W * WorldTerrain.RICE_CHUNK_H;
		//uint riceLostThisYear = userData.getRiceLostThisYear() + riceOfChunksDisabled;
		uint riceLostThisYear = (uint) (wt.getNumberOfChunks() * RiceTerrainTile.MAX_RICE_UNITS * WorldTerrain.RICE_CHUNK_W * WorldTerrain.RICE_CHUNK_H * (1 + invMan.getRicePerChunkBonus()*0.01f) - anualProduction);
		m_riceLostNumberText.text = riceLostThisYear.ToString();

		int anualRiceSold = userData.gold.getRiceGoldAddThisYear();
		m_riceSoldNumberText.text = anualRiceSold.ToString ();

		bool ecoGrant = InvestigationManager.GetInstance ().isInvestigated (INVESTIGATIONS_ID.ECO_SUBVENCION);
		if (ecoGrant) {
			int ecoGrantBonus = CoopManager.GetInstance ().getCurrentEcologyBonus ();
			m_grantEcologyNumberText.text = ecoGrantBonus.ToString ();
		} else {
			m_grantEcologyNumberText.text = "";
			GameObject.Find("EcologyGrantText").GetComponent<Text>().enabled = false;
			GameObject.Find ("EcologyGrantNumberUnits").GetComponent<Image> ().enabled = false;
			GameObject.Find ("EcologyGrantButtonHelp").SetActive (false);
		}

		RankingManager.GetInstance ().addScore ((int)anualProduction);

		CoopManager.GetInstance().happyNewYear();

        UserDataManager.GetInstance().resetYearlyData();

		/*
		const float TON = 1000000.0f;
		if (anualRiceSold < TON) {
			m_riceProductionNumberText.text = anualRiceSold.ToString ();
		} else {
			m_riceProductionNumberText.text = (anualRiceSold / TON).ToString ("0.00");
			m_riceProductionNumberUnitsText = "T";
		}
		*/
		GameObject.Find ("Logic").GetComponent<TimeManager> ().pauseTime ();
    }

    public new void kill()
    {
		GameObject.Find("Logic").GetComponent<TimeManager>().changeMode(TimeManager.SPEED_MODE.NORMAL);
        Destroy(this.transform.parent.gameObject);
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

	public void EcoGrantHelpClicked()
	{
		TutorialManager tutMan  = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
		tutMan.startTuto(new Tutorial_Ecology());
	}
}