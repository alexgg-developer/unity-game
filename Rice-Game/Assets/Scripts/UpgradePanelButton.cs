using UnityEngine;
using System.Collections;

public class UpgradePanelButton : PanelButton
{
    public UpgradePanelBehaviour.PanelPosition PanelPosition { get; set; }
    public new void clicked()
    {
        UpgradePanelBehaviour upgradePanel = AttachedPanel.GetComponent<UpgradePanelBehaviour>();
        upgradePanel.objectClicked(id, PanelPosition);
    }
}


