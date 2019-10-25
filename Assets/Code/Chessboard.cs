using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
  
*/
public class Chessboard : MonoBehaviour
{
    public Transform BasePole;
    public int Col;         // X
    public int Row;         // Z
    public int GridSize = 1;
    public List<int> BoardData; //Data
    public List<Pawn> PawnList;
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
        var mapData = System.Activator.CreateInstance(t) as BoardMap;
        MapObject = Instantiate(Resources.Load(mapData.MapPrefabName)) as GameObject;
        MapObject.transform.parent = BasePole;
        MapObject.transform.localPosition = Vector3.zero;
        BoardData = new List<int>(mapData.Data);
        if (Col * Row != BoardData.Count)
        {
            Debug.LogError("Error Board Num");
        }
        FinishLoadMap();
    }
    public void FinishLoadMap()
    {
        //TEST CODE
        var gp = Vector2Int.zero;
        for (var i = 0;i<Col;++i)
        {
            for (var j = 0;j<Row;++j)
            {
                gp.Set(i, j);
                var gt = GetGridType(gp);
                switch (gt)
                {
                    case 1:
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
        var gp = new Vector2Int(Mathf.FloorToInt(wp.x / GridSize), Mathf.FloorToInt(wp.z / GridSize));
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
        if (gp.x < 0 || gp.y < 0 || gp.x >= Col || gp.y >= Row) return -1;
        return BoardData[gp.y * Col + gp.x];
    }
    // No Bounding check!
    private int gridToIdx(Vector2Int gp)
    {
        return gp.y * Col + gp.x;
    }


}
