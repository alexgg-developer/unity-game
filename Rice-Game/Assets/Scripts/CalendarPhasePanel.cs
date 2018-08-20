using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class CalendarPhasePanel : Panel
{

	private Text m_phaseTitleText = null;
    private Text m_currentPhaseDescriptionText = null;
    private List<Text> m_actionsPhase;
    private const int MAX_ACTIONS = 7;

    void Start () {
		init ();
	}

	void OnEnable()
    {
		init();
    }

	void init() {
		m_actionsPhase = new List<Text>();
		if (m_phaseTitleText == null) m_phaseTitleText = GameObject.Find ("PhaseTitleText").GetComponent<Text>();
        if (m_currentPhaseDescriptionText == null) m_currentPhaseDescriptionText = GameObject.Find("CurrentPhaseDescText").GetComponent<Text>();
        if(m_actionsPhase.Count == 0) {
            for(int i = 1; i <= MAX_ACTIONS; ++i) {
                m_actionsPhase.Add(GameObject.Find("ActionText_" + i).GetComponent<Text>());
            }
        }
        updatePhaseTexts();
        GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().addListenerToPhaseChange(updatePhaseTexts);
    }

    private void updatePhaseTexts()
    {
        Phase phaseInfo = GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().getCurrentPhaseInfo();
        m_phaseTitleText.text = phaseInfo.Name;
        m_currentPhaseDescriptionText.text = phaseInfo.Description;
        List<string> actionsPhase = phaseInfo.ActionsInfo;
        int i;
        for(i = 0; i < actionsPhase.Count; ++i) {
            //m_actionsPhase[i].gameObject.SetActive(true);
            m_actionsPhase[i].text = actionsPhase[i];
        }
        for (; i < MAX_ACTIONS; ++i) {
			//m_actionsPhase[i].gameObject.SetActive(false);
			m_actionsPhase[i].text = "";
        }

        m_phaseTitleText.text = phaseInfo.Name;
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }
}
