using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class ActionInProgressData
{
    public int ID;
    public int HoursElapsed;
    public int ChunkNumber;
    public float AnimationRemainingDistance;

    public ActionInProgressData(int id, int hoursElapsed, int chunkNumber, float animationRemainingDistance)
    {
        ID = id;
        HoursElapsed = hoursElapsed;
        ChunkNumber = chunkNumber;
        AnimationRemainingDistance = animationRemainingDistance;
    }
}

[System.Serializable]
public class ActionManagerData
{
    public List<ActionInProgressData> ActionsInProgress { get; set; }
    public ActionManagerData()
    {
        ActionsInProgress = new List<ActionInProgressData>();
    }
}
