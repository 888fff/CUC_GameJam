using UnityEngine;

[SerializeField]
public struct WolfInitData
{
    public WolfInitData(int x,int y,int dx,int dy)
    {
        Pos = new Vector2Int(x, y);
        Dir = new Vector2Int(dx, dy);
    }
    public Vector2Int Pos;
    public Vector2Int Dir;
}

public class BoardMap
{
    public string MapPrefabName;
    public int[] Data;
    public Vector2Int SheepPos;
    public Vector2Int DogPos;
    public WolfInitData[] WolfPosAndDir;

    //
    public BoardMap()
    {
        Debug.Log("BoardMap");
        InitData();
    }

    public virtual void InitData()
    {
        MapPrefabName = "";
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
    }
}
