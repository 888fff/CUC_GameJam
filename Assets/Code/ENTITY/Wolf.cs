using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Pawn
{
    public WolfState state;
    public bool afraidInit;
    //public bool patrolInit;

    public Vector2Int destination;
    public Vector2Int curPosition;

    void Start()
    {
        //初始化位置;
        curPosition = new Vector2Int(0, 2);
        destination = new Vector2Int(0, 2);
        SetToGrid(curPosition);

        state = WolfState.patrol;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            state = WolfState.afraid;
        }

        NextPosition();

        WalkTo(destination);
    }

    void NextPosition()
    {
        //获取当前的位置;
        curPosition = GetGridPos();

        if (state == WolfState.dead)
            return;

        if(state == WolfState.afraid)
        {
            if(!afraidInit)
            {
                curPosition = GetGridPos();
                Vector2Int targetDir = WalkDir * -1;
                Vector2Int targetPos = Vector2Int.zero;
                int targetX = curPosition.x + targetDir.x;
                int targetY = curPosition.y + targetDir.y;
                //为了更直观,被惊吓则变为蓝色;
                GetComponent<MeshRenderer>().material.color = Color.blue;
                Speed = 0.3f;

                if (targetX<0||targetX>4||targetY<0||targetY>4)
                {
                    //如果越界,则还为当前位置;
                    targetPos = curPosition;
                }
                else
                {
                    int mapIndex = targetY * 5 + targetX;//即将移动位置的总索引值;
                    targetPos = curPosition + targetDir;

                    if (Board.BoardData[mapIndex] == 1)
                    {
                        //当反向一格是障碍物时
                        targetPos = curPosition;
                    }
                }

                destination = targetPos;
                afraidInit = true;
                return;
            }
            else
            {
                if(curPosition==destination)
                {
                    //在受惊吓状态,但已到达目的地,则恢复红色,并开始巡逻;
                    GetComponent<MeshRenderer>().material.color = Color.red;
                    Speed = 1.2f;
                    state = WolfState.patrol;
                    afraidInit = false;
                }
            }
        }

        if(state==WolfState.chase)
        {
            //如果当前位置有猎物;
            if(curPosition==Vector2Int.zero)
            {
                state = WolfState.attack;
                destination = curPosition;
            }
            else
            {
                Vector2Int preyPos = Vector2Int.zero;
                var pawn = DetectionOtherPawn(WalkDir, ref preyPos);
                //如果检测到猎物;
                if(pawn!=null)
                {
                    destination = preyPos;
                }
                else
                {
                    state = WolfState.patrol;
                }
            }
        }
        else
        {
            if(state!=WolfState.afraid)
            {
                Vector2Int preyPos = Vector2Int.zero;
                var pawn = DetectionOtherPawn(WalkDir, ref preyPos);

                if (pawn != null)
                {
                    //如果检测到猎物;
                    destination = preyPos;
                    state = WolfState.chase;
                }
                else
                {
                    //未检测到猎物，则进行巡逻;
                    PatrolControl();
                }
            }
        }
    }

    void PatrolControl()
    {
        Vector2Int targetPos = Vector2Int.zero;

        curPosition = GetGridPos();
        int targetX = curPosition.x+WalkDir.x;
        int targetY = curPosition.y+WalkDir.y;
        int mapIndex = targetY * 5 + targetX;//即将移动位置的总索引值;

        //即将移动位置是否越界,越界则反弹;
        if(targetX<0||targetX>4||targetY<0||targetY>4)
        {
            Vector2Int targetDir = WalkDir * -1;
            targetPos=curPosition+targetDir;
        }
        else
        {
            if(Board.BoardData[mapIndex]==1)
            {
                //检测到障碍物;
                Vector2Int targetDir = WalkDir * -1;
                targetPos = curPosition + targetDir;
            }
            else
            {
                //若没有检测到障碍物;
                targetPos = curPosition + WalkDir;
            }
        }

        destination = targetPos;
    }
}

public enum WolfState
{
    afraid, patrol, chase, attack, dead
}
