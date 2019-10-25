using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardMap
{
    public string MapPrefabName;
    public int[] Data;
    //
    public BoardMap()
    {
        Debug.Log("BoardMap");
        InitData();
    }

    public virtual void InitData()
    {
        MapPrefabName = "";
        Data = new int[25] {
            0,0,0,0,0,
            0,0,0,0,0,
            0,0,0,0,0,
            0,0,0,0,0,
            0,0,0,0,0
        };
    }
}
