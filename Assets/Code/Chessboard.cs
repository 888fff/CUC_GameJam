using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
  
*/
public class GridType
{
    public const int WALKABLE = 0;
    public const int UNWALKABLE = 1;
    public const int TRAP = 2;
    public const int TRAP_USED = 3;
    public const int DEST = 9;
}
public class Chessboard : MonoBehaviour
{
    public Transform BasePole;
    public int Col;         // X
    public int Row;         // Z
    public int GridSize = 1;
    public List<int> BoardData; //Data
    public List<Pawn> PawnList;
    //there is a raw data
    public BoardMap MapData;
    //---
    public GameObject MapObject;


    // Start is called before the first frame update
    void Start()
    {
        if (BasePole == null) BasePole = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // LOAD
    public void LoadMap(int level)
    {
        Type t = Type.GetType("Map_" + level);
        MapData = System.Activator.CreateInstance(t) as BoardMap;
        MapObject = Instantiate(Resources.Load(MapData.MapPrefabName)) as GameObject;
        MapObject.transform.parent = BasePole;
        MapObject.transform.localPosition = Vector3.zero;
        BoardData = new List<int>(MapData.Data);
        if (Col * Row != BoardData.Count)
        {
            Debug.LogError("Error Board Num");
        }
        FinishLoadMap();
    }
    public void ResetMap()
    {
        //remove scene gameobject
        if(MapObject != null) Destroy(MapObject);
        //remove all board data
        if(BoardData!= null) BoardData.Clear();
        //remove pawns
        if(PawnList != null)
        {
            for (var i = 0; i < PawnList.Count; ++i)
            {
                Destroy(PawnList[i].gameObject);
            }
            PawnList.Clear();
        }
        //clear mapdata
        MapData = null;
    }
    public void FinishLoadMap()
    {
        //TEST CODE
        /*
        var gp = Vector2Int.zero;
        for (var i = 0;i<Col;++i)
        {
            for (var j = 0;j<Row;++j)
            {
                gp.Set(i, j);
                var gt = GetGridType(gp);
                switch (gt)
                {
                    case GridType.UNWALKABLE:
                        {
                            var trans = MapObject.transform.GetChild((gp.y) * Col + gp.x);
                            if (trans)
                            {
                                trans.GetComponent<MeshRenderer>().material.color = Color.red;
                            }
                        }
                     break;
                }
            }
        }
        */
    }
    //// HELPER Function
    public Vector3 GridToWorld(Vector2Int gp)
    {
        var wp = new Vector3(gp.x * GridSize,0,gp.y * GridSize);
        wp = wp + BasePole.position;
        return wp;
    }
    public Vector2Int WorldToGrid(Vector3 wp)
    {
        wp = wp - BasePole.position;
        var gp = new Vector2Int(Mathf.CeilToInt(wp.x / GridSize), Mathf.CeilToInt(wp.z / GridSize));
        return gp;
    }
    //-------------------------------
    public void RegisterPawn(Pawn pawn)
    {
        PawnList.Add(pawn);
        pawn.Board = this;
    }
    public void UnregisterPawn(Pawn pawn)
    {
        pawn.Board = null;
        PawnList.Remove(pawn);
    }
    //---------------------------------
    public int GetGridType(Vector2Int gp)
    {
        if (!IsGridPosLegal(gp)) return -1;
        return BoardData[gp.y * Col + gp.x];
    }
    // No Bounding check!
    private int gridToIdx(Vector2Int gp)
    {
        return gp.y * Col + gp.x;
    }
    public bool IsGridPosLegal(Vector2Int gp)
    {
        if (gp.x < 0 || gp.y < 0 || gp.x >= Col || gp.y >= Row) return false;
        return true;
    }
    public bool IsOccupation(Vector2Int gp)
    {
        for (var i = PawnList.Count - 1; i >= 0 ; i--)
        {
            if(PawnList[i].GetGridPos() == gp)
            {
                return true;
            }
        }
        return false;
    }


}
