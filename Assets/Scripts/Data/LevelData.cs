using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string sceneNane;
    public int enterpointID;

    public LevelData(string sceneNane,int enterpointID)
    {
        this.sceneNane = sceneNane;
        this.enterpointID = enterpointID;
    }
}
