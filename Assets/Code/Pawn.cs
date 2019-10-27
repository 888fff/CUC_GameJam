using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public int SightLength = 1;
    public float Speed = 1; // m per second
    public Chessboard Board;
    public Vector2Int DestGridPos;
    public Vector2Int WalkDir;
    public bool Walking = false;
    TweenCallback TCB;
    //
    public static Vector2Int[] DIR_ARRAY = {
        Vector2Int.left,Vector2Int.right,Vector2Int.up,Vector2Int.down
    };
    // Start is called before the first frame update
    void Start()
    {
        TCB += EndWalk;
        DestGridPos = new Vector2Int(-1, -1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy()
    {
        Board.UnregisterPawn(this);
    }
    // POS HELP
    public Vector2Int GetGridPos() //maybe not exact!
    {
        return Board.WorldToGrid(transform.position);
    }
    public bool IsWalking()
    {
        return Walking;
    }
    //
    public bool WalkStep(Vector2Int dir)
    {
        if (Walking) return false;
        var gp = GetGridPos();
        var dest = gp + dir;
        if (!Board.IsGridPosLegal(dest))
        {
            return false;
        }
        WalkTo(dest);
        return true;
    }
    public void WalkTo(Vector2Int gridPos)
    {
        if (Walking || Speed == 0) return;
        var wp = Board.GridToWorld(gridPos);
        var dur = Vector3.Distance(wp,transform.position) / Speed;
        WalkDir = gridPos - GetGridPos();
        DestGridPos = gridPos;
        transform.DOMove(wp, dur).OnComplete(()=>_EndWalk());
        _StartWalk();
    }
    private void _StartWalk()
    {
        Walking = true;
        //
        StartWalk();
    }
    private void _EndWalk()
    {
        //WalkDir = Vector2Int.zero;
        Walking = false;
        //
        EndWalk();
    }
    public virtual void StartWalk()
    {
        //Debug.Log("Pawn StartWalk");
        Walking = true;
    }
    public virtual void EndWalk()
    {
        //Debug.Log("Pawn EndWalk");
    }
    public void SetToGrid(Vector2Int gp)
    {
        transform.position = Board.GridToWorld(gp);
    }
    //SIGHT
    /*
      dir must be a (1,0) (0,1) (-1,0) (0,-1)
    */
    public Vector2Int GetRandomAroundPos()
    {
        var gp = GetGridPos();
        var size = Pawn.DIR_ARRAY.Length;
        var rnd_idx = Random.Range(0, size);
        var rnd_pos = Pawn.DIR_ARRAY[rnd_idx];
        while (Board.GetGridType(rnd_pos)!=0)
        {
            rnd_idx--;
            if (rnd_idx<0)
            {
                rnd_idx = Pawn.DIR_ARRAY.Length;
            }
            rnd_pos = Pawn.DIR_ARRAY[rnd_idx];
            size--;
            if (size < 0)
            {
                Debug.LogError("Fuck! Infinite Loop!");
                break;
            }
        }
        return rnd_pos;
    }
    public Pawn DetectionOtherPawn(Vector2Int dir, ref Vector2Int destGridPos,ref Vector2Int moveDir)
    {
        //Pawn retVal=null;
        var gp = GetGridPos();
        //search for dir line except self
        for (var i = 1; i<= SightLength;++i)
        {
            var dp = gp + dir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y <0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;//Type为1表示为障碍物;
                else
                {
                    for (var j = 0;j<Board.PawnList.Count;++j)
                    {
                        if(Board.PawnList[j].GetGridPos() == dp)
                        {
                            /*Dog dog = Board.PawnList[j].transform.gameObject.GetComponent<Dog>();
                            Sheep sheep= Board.PawnList[j].transform.gameObject.GetComponent<Sheep>();
                            if(dog!=null||sheep!=null)
                            {
                                //destGridPos.Set(dp.x,dp.y);
                                moveDir = dir;
                                //return Board.PawnList[j];
                                retVal = Board.PawnList[j];
                            }*/
                            destGridPos.Set(dp.x,dp.y);
                            moveDir = dir;
                            return Board.PawnList[j];
                        }
                    }
                }
            }
        }
        //left and right cross sight line
        var cdir = new Vector2Int(dir.y,dir.x);
        for (var i = - 1; i >= -SightLength ;--i)
        {
            var dp = gp + cdir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            /*Dog dog = Board.PawnList[j].transform.gameObject.GetComponent<Dog>();
                            Sheep sheep = Board.PawnList[j].transform.gameObject.GetComponent<Sheep>();
                            if (dog != null || sheep != null)
                            {
                                //destGridPos.Set(dp.x,dp.y);
                                moveDir = cdir;
                                //return Board.PawnList[j];
                                retVal = Board.PawnList[j];
                            }*/
                            destGridPos.Set(dp.x, dp.y);
                            moveDir = cdir * -1;
                            return Board.PawnList[j];
                        }
                    }
                }
            }
        }
        for (var i = 1; i <= SightLength; ++i)
        {
            var dp = gp + cdir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            /*Dog dog = Board.PawnList[j].transform.gameObject.GetComponent<Dog>();
                            Sheep sheep = Board.PawnList[j].transform.gameObject.GetComponent<Sheep>();
                            if (dog != null || sheep != null)
                            {
                                //destGridPos.Set(dp.x,dp.y);
                                moveDir = cdir;
                                //return Board.PawnList[j];
                                //retVal = Board.PawnList[j];
                            }*/
                            destGridPos.Set(dp.x, dp.y);
                            moveDir = cdir;
                            return Board.PawnList[j];
                        }
                    }
                }
            }
        }
        //return retVal;
        return null;
    }

    public Pawn DetectFood(Vector2Int dir, ref Vector2Int destGridPos,ref Vector2Int moveDir)
    {
        Pawn retVal=null;
        Vector2Int foodPos = Vector2Int.zero;

        var gp = GetGridPos();

        //search for dir line except self
        for (var i = 1; i <= SightLength; ++i)
        {
            var dp = gp + dir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            Food food = Board.PawnList[j].transform.gameObject.GetComponent<Food>();
                            if(food!=null)
                            {
                                if (retVal == null)
                                {
                                    retVal = Board.PawnList[j];
                                    foodPos = new Vector2Int(dp.x, dp.y);
                                    moveDir = dir;
                                }
                                else
                                {
                                    if(food.foodType==FoodType.fruit)
                                    {
                                        retVal= Board.PawnList[j];
                                        foodPos = new Vector2Int(dp.x, dp.y);
                                        moveDir = dir;
                                    }
                                    else
                                    {
                                        //如果是草;
                                        //Do Nothing!
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //search the behind
        for (var i = -1; i >= -SightLength; --i)
        {
            var dp = gp + dir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            Food food = Board.PawnList[j].transform.gameObject.GetComponent<Food>();
                            if (food != null)
                            {
                                if (retVal == null)
                                {
                                    retVal = Board.PawnList[j];
                                    foodPos = new Vector2Int(dp.x, dp.y);
                                    moveDir = dir* -1;
                                }
                                else
                                {
                                    if (food.foodType == FoodType.fruit)
                                    {
                                        retVal = Board.PawnList[j];
                                        foodPos = new Vector2Int(dp.x, dp.y);
                                        moveDir = dir * -1;
                                    }
                                    else
                                    {
                                        //如果是草;
                                        //Do Nothing!
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //left and right cross sight line
        var cdir = new Vector2Int(dir.y, dir.x);
        for (var i = -1; i >= -SightLength; --i)
        {
            var dp = gp + cdir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            Food food = Board.PawnList[j].transform.gameObject.GetComponent<Food>();
                            if (food != null)
                            {
                                if (retVal == null)
                                {
                                    retVal = Board.PawnList[j];
                                    foodPos = new Vector2Int(dp.x, dp.y);
                                    moveDir = cdir * -1;
                                }
                                else
                                {
                                    if (food.foodType == FoodType.fruit)
                                    {
                                        retVal = Board.PawnList[j];
                                        foodPos = new Vector2Int(dp.x, dp.y);
                                        moveDir = cdir * -1;
                                    }
                                    else
                                    {
                                        //如果是草;
                                        //Do Nothing!
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for (var i = 1; i <= SightLength; ++i)
        {
            var dp = gp + cdir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            Food food = Board.PawnList[j].transform.gameObject.GetComponent<Food>();
                            if (food != null)
                            {
                                if (retVal == null)
                                {
                                    retVal = Board.PawnList[j];
                                    foodPos = new Vector2Int(dp.x, dp.y);
                                    moveDir = cdir;
                                }
                                else
                                {
                                    if (food.foodType == FoodType.fruit)
                                    {
                                        retVal = Board.PawnList[j];
                                        foodPos = new Vector2Int(dp.x, dp.y);
                                        moveDir = cdir;
                                    }
                                    else
                                    {
                                        //如果是草;
                                        //Do Nothing!
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if(retVal!=null)
            destGridPos.Set(foodPos.x, foodPos.y);
        return retVal;
    }

    public bool DetectWolf(Vector2Int dir,int barkRange)
    {
        bool retVal = false;

        var gp = GetGridPos();
        //search for dir line except self
        for (var i = 1; i <= barkRange; ++i)
        {
            var dp = gp + dir * i;
            if (dp.x < 0 || dp.x >= Board.Col || dp.y < 0 || dp.y >= Board.Row) break;
            else
            {
                if (Board.GetGridType(dp) != 0) break;
                else
                {
                    for (var j = 0; j < Board.PawnList.Count; ++j)
                    {
                        if (Board.PawnList[j].GetGridPos() == dp)
                        {
                            Wolf wolf = Board.PawnList[j].transform.gameObject.GetComponent<Wolf>();
                            if(wolf!=null)
                            {
                                wolf.state = WolfState.afraid;
                                retVal = true;
                            }
                        }
                    }
                }
            }
        }

        return retVal;
    }
}
