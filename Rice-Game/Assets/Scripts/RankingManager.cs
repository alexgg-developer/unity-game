using UnityEngine;
using System.Collections;
using System;

public class RankingManager {

    private static RankingManager instance = null;
    private int[] ranks;
    private int nRanks;

    public static RankingManager GetInstance()
    {
        if (instance == null)
        {
            instance = new RankingManager();
        }

        return instance;
    }

    private RankingManager()
    {
        ranks = new int[7];
        if (PlayerPrefs.HasKey("ranksInit"))
            loadRanks();
        else
        {
            ranks[5] = 10;
            ranks[4] = 100;
            ranks[3] = 500;
            ranks[2] = 1000;
            ranks[1] = 5000;
            ranks[0] = 10000;
            nRanks = 6;
            setRanks();
            PlayerPrefs.SetInt("ranksInit", 1);
        }
    }

    public void loadRanks()
    {
        nRanks = 6;
        for (int i = 0; i < 6; i++)
        {
            ranks[i] = PlayerPrefs.GetInt("rank" + i);
        }
    }

    public void setRanks()
    {
        for (int i = 0; i < nRanks; i++)
        {
            PlayerPrefs.SetInt("rank" + i, ranks[i]);
        }
    }

    public int getNRanks()
    {
        return nRanks;
    }

    public int[] getRanks()
    {
        return ranks;
    }

    public void addScore(int score)
    {
        for (int i = nRanks; i > 0; i--)
        {
            if (score > ranks[i] - 1) ranks[i] = ranks[i - 1];
            else
            {
				ranks[i] = score;
                setRanks();
                return;
            }
        }
		ranks[0] = score;
        setRanks();
    }

    public void resetScores()
    {
        for (int i = 0; i < 6; i++)
        {
            if (PlayerPrefs.HasKey("rank" + i))
                PlayerPrefs.DeleteKey("rank" + i);
        }
        ranks[5] = 10;
        ranks[4] = 100;
        ranks[3] = 500;
        ranks[2] = 1000;
        ranks[1] = 5000;
        ranks[0] = 10000;
        setRanks();
    }

    public void save(RankingManagerData rankingManagerData)
    {
        rankingManagerData.Ranks = ranks;
    }

    public void load(RankingManagerData rankingManagerData)
    {
        ranks = rankingManagerData.Ranks;
    }
}
