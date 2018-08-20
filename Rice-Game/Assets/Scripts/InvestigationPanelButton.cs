using UnityEngine;
using System.Collections;

public class InvestigationPanelButton : PanelButton
{
	public InvestigationMenu.PanelPosition PanelPosition { get; set; }
	public new void clicked()
	{
		InvestigationMenu investigationMenu = AttachedPanel.GetComponent<InvestigationMenu>();
		investigationMenu.investigationClicked(id, PanelPosition);
	}
}


