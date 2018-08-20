using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankingPanel : MonoBehaviour {

    public Text[] ranks;
    private int[] rnk;
    private int n;
    private RankingManager rm;

	void Start () {
        rm = RankingManager.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
        rnk = rm.getRanks();
        n = rm.getNRanks();
        for (int i = 0; i < n; i++)
        {
            ranks[i].text = rnk[i] + "  pts";
        }
	}
}
