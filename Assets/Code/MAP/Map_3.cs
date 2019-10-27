using UnityEngine;
using System.Collections;

public class Map_3 : BoardMap
{
    public override void InitData()
    {
        MapPrefabName = "Map_3";
        Data = new int[96] {
            0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,1,1,1,0,0,1,1,1,0,0,
            0,0,1,0,1,0,0,1,0,1,0,0,
            0,0,1,0,1,0,0,1,0,1,0,0,
            0,0,1,1,1,0,0,1,1,1,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,9
        };
        SheepPos = new Vector2Int(1,1);
        DogPos = new Vector2Int(0,0);
        WolfPosAndDir = new WolfInitData[2] {
           new WolfInitData(7,6,1,0),
           new WolfInitData(5,4,0,1),
        };
    }
}
