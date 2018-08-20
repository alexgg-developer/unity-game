using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class InvestigationUnlockedPanel : Panel
{
	private Text m_investigationText = null;

    override protected void initBase()
    {
        this.transform.parent.gameObject.GetComponent<RectTransform>().SetParent(GameObject.Find("UICanvas").GetComponent<RectTransform>(), false);
    }

    void Awake ()
    {
		initBase ();	
		Init ();
	}

	private void Init()
	{
		if (m_investigationText == null) m_investigationText = GameObject.Find("InvestigationUnlockedText").GetComponent<Text>();
	}
    
    public new void kill()
    {
        Destroy(this.transform.parent.gameObject);
    }

    public override void actionClicked(int id)
    {
        throw new NotImplementedException();
    }

	public void ChangeText(string text)
	{
		m_investigationText.text = text;
	}
}