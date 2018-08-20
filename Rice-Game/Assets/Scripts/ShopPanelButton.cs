using UnityEngine;
using System.Collections;

public class ShopPanelButton : PanelButton
{
    public BuyMenu.PanelPosition PanelPosition { get; set; }
    public new void clicked()
    {
        BuyMenu buyMenu = AttachedPanel.GetComponent<BuyMenu>();
        buyMenu.objectClicked(id, PanelPosition);
    }
}


