using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class GameSaveData
{
    public ActionManagerData ActionManagerData;
    public BuildingManagerData BuildingManagerData;
    public TimeManagerData TimeManagerData;
    public CoopManagerData CoopManagerData;
    public UserDataManagerData UserDataManagerData;
    public InvestigationManagerData InvestigationManagerData;
    public LogicManagerData LogicManagerData;
    public CanalManagerData CanalManagerData;
    public PenalizationManagerData PenalizationManagerData;
    public PhaseManagerData PhaseManagerData;
    public PlagueManagerData PlagueManagerData;
    public RankingManagerData RankingManagerData;
    public RiceObjectManagerData RiceObjectManagerData;
    public WeedFactoryData WeedFactoryData;
    public WorkerManagerData WorkerManagerData;
    public WorldTerrainData WorldTerrainData;
	public TutorialManagerData tutorialManagerData;

    public GameSaveData()
    {
        ActionManagerData = new ActionManagerData();
        BuildingManagerData = new BuildingManagerData();
        TimeManagerData = new TimeManagerData();
        CoopManagerData = new CoopManagerData();
        UserDataManagerData = new UserDataManagerData();
        InvestigationManagerData = new InvestigationManagerData();
        LogicManagerData = new LogicManagerData();
        CanalManagerData = new CanalManagerData();
        PenalizationManagerData = new PenalizationManagerData();
        PhaseManagerData = new PhaseManagerData();
        PlagueManagerData = new PlagueManagerData();
        RankingManagerData = new RankingManagerData();
        RiceObjectManagerData = new RiceObjectManagerData();
        WeedFactoryData = new WeedFactoryData();
        WorkerManagerData = new WorkerManagerData();
        WorldTerrainData = new WorldTerrainData();
		tutorialManagerData = new TutorialManagerData ();
}

    public void test()
    {
        ActionManagerData = new ActionManagerData();
        for (int i = 0; i < 10; ++i) {
            ActionManagerData.ActionsInProgress.Add(new ActionInProgressData(i, i*3, i*5, 0.5f));
        }
    }

}
