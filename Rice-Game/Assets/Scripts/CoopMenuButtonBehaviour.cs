using UnityEngine;
using System.Collections;

public class CoopMenuButtonBehaviour : MonoBehaviour
{
    public GameObject AttachedPanel;
    public CoopMenu.MilestoneType id;
    public void clicked()
    {
        CoopMenu coopMenu = AttachedPanel.GetComponent<CoopMenu>();
        coopMenu.objectClicked(id);
    }
}


