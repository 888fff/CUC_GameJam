using UnityEngine;
using System.Collections;

public class Map_2 : BoardMap
{
    public override void InitData()
    {
        MapPrefabName = "Map_2";
        Data = new int[25] {
            0,0,0,0,0,
            0,0,0,1,1,
            0,0,0,0,0,
            1,1,0,0,0,
            0,0,0,0,0
        };
        SheepPos = new Vector2Int(1,1);
        DogPos = new Vector2Int(0,0);
        WolfPosAndDir = new WolfInitData[2] {
           new WolfInitData(2,2,1,0),
           new WolfInitData(3,3,0,1),
        };
    }
}
