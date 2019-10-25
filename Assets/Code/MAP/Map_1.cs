using UnityEngine;
using System.Collections;

public class Map_1 : BoardMap
{
    public override void InitData()
    {
        MapPrefabName = "Map_1";
        Data = new int[25] {
            0,0,0,0,0,
            0,1,1,0,0,
            0,0,0,0,0,
            0,0,0,0,0,
            0,0,0,0,0
        };
    }
}
