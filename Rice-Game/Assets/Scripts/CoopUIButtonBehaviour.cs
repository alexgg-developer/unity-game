using UnityEngine;
using System.Collections;

public class CoopUIButtonBehaviour : MonoBehaviour
{
    public GameObject CoopMenu;

    public void clicked()
    {
		if(InvestigationManager.GetInstance().isInvestigated(INVESTIGATIONS_ID.COOP)) {
            CoopMenu.SetActive(true);
        }
    }
}
