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
    public SkinnedMeshRenderer SMR;
    public Animator anim;
    public bool isDead;
    void Start()
    {
        //初始化位置;
        //curPosition = new Vector2Int(3, 4);
        //destination = new Vector2Int(3, 4);
        //SetToGrid(curPosition);

        state = WolfState.patrol;

        anim = GetComponent<Animator>();
        isDead = false;

        SMR = transform.Find("Wolf").GetComponent<SkinnedMeshRenderer>();

    }

    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.M))
        {
            state = WolfState.afraid;
        }*/

        if (isDead)
        {
            anim.SetTrigger("die");
        }

        ControlRotation(WalkDir);

        MoveControl();
    }

    void MoveControl()
    {
        //获取当前的位置;
        curPosition = GetGridPos();

        if (state == WolfState.dead||state==WolfState.attack)
            return;

        if(state==WolfState.chase)
        {
            curPosition = GetGridPos();
            Vector2Int preyPos = Vector2Int.zero;
            Vector2Int moveDir = Vector2Int.zero;
            Pawn pawn = DetectionOtherPawn(WalkDir, ref preyPos, ref moveDir);
            if (pawn != null)
            {               
                Dog dog = pawn.transform.gameObject.GetComponent<Dog>();
                Sheep sheep = pawn.transform.gameObject.GetComponent<Sheep>();
                if(dog!=null||sheep!=null)
                {
                    //追逐过程中未丢失目标;
                    destination = preyPos;
                    WalkStep(moveDir);
                }
            }
            else
            {
                //追逐过程中丢失目标,则进入巡逻状态;
                state = WolfState.patrol;
            }
        }
        else
        {
            //处于巡逻状态时;
            if(state!=WolfState.afraid)
            {
                curPosition = GetGridPos();
                Vector2Int preyPos = Vector2Int.zero;
                Vector2Int moveDir = Vector2Int.zero;
                Pawn pawn = DetectionOtherPawn(WalkDir, ref preyPos,ref moveDir);

                if (pawn != null)
                {
                    Dog dog = pawn.transform.gameObject.GetComponent<Dog>();
                    Sheep sheep = pawn.transform.gameObject.GetComponent<Sheep>();
                    if(dog!=null||sheep!=null)
                    {
                        //如果检测到猎物;
                        destination = preyPos;
                        state = WolfState.chase;
                        //WalkTo(destination);
                        WalkStep(moveDir);
                    }
                    else
                    {
                        PatrolControl();
                    }
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
        int mapIndex = targetY * Board.Col + targetX;//5 + targetX;//即将移动位置的总索引值;

        //即将移动位置是否越界,越界则反弹;
        if(targetX<0||targetX>= Board.Col || targetY<0||targetY>= Board.Row)//!!!!此处需修改边界值;
        {
            Vector2Int targetDir = WalkDir * -1;
            targetPos=curPosition+targetDir;
            WalkStep(targetDir);
        }
        else
        {
            if(Board.BoardData[mapIndex]==1)
            {
                //检测到障碍物;
                Vector2Int targetDir = WalkDir * -1;
                targetPos = curPosition + targetDir;
                WalkStep(targetDir);
            }
            else
            {
                //若没有检测到障碍物;
                targetPos = curPosition + WalkDir;
                WalkStep(WalkDir);
            }
        }

        destination = targetPos;
    }

    public override void EndWalk()
    {
        curPosition = GetGridPos();

        if (state == WolfState.dead||state==WolfState.attack)
            return;

        if (state == WolfState.afraid)
        {
            //Walking = false;
            SMR.material.color = Color.blue;
            if (!afraidInit)
            {
                curPosition = GetGridPos();
                Vector2Int targetDir = WalkDir * -1;
                Vector2Int targetPos = Vector2Int.zero;
                int targetX = curPosition.x + targetDir.x;
                int targetY = curPosition.y + targetDir.y;
                //为了更直观,被惊吓则变为蓝色;
                //GetComponent<MeshRenderer>().material.color = Color.blue;
                Speed = 0.3f;

                if (targetX < 0 || targetX >= Board.Col || targetY < 0 || targetY >= Board.Row)//!!!!此处需修改边界值;
                {
                    //如果越界,则还为当前位置;
                    targetPos = curPosition;
                    targetDir = Vector2Int.zero;

                    //检测倒退1格;
                    /*Vector2Int targetDir_2 = WalkDir * -1;
                    Vector2Int targetPos_2 = Vector2Int.zero;
                    int targetX_2 = curPosition.x + targetDir.x;
                    int targetY_2 = curPosition.y + targetDir.y;

                    if(targetX_2 < 0 || targetX_2 >= Board.Col || targetY_2 < 0 || targetY_2 >= Board.Row)
                    {
                        targetPos = curPosition;
                        targetDir = Vector2Int.zero;
                    }
                    else
                    {
                        int mapIndex = targetY_2 * 5 + targetX_2;//即将移动位置的总索引值;
                        targetPos = curPosition + targetDir_2;
                    }*/
                }
                else
                {
                    int mapIndex = targetY * Board.Col + targetX;//5 + targetX;//即将移动位置的总索引值;
                    targetPos = curPosition + targetDir;


                    if (Board.BoardData[mapIndex] == 1)
                    {
                        //当反向一格是障碍物时
                        targetPos = curPosition;
                        targetDir = Vector2Int.zero;
                    }
                }

                WalkStep(targetDir);
                //WalkTo(targetPos);
                destination = targetPos;
                afraidInit = true;
                return;
            }
            else
            {
                if (curPosition == destination)
                {
                    //在受惊吓状态,但已到达目的地,则恢复红色,并开始巡逻;
                    //GetComponent<MeshRenderer>().material.color = Color.red;
                    Speed = 1.2f;
                    state = WolfState.patrol;
                    afraidInit = false;
                    SMR.material.color = Color.white;

                }
            }
        }
    }

    public override void StartWalk()
    {
        anim.SetTrigger("run");
    }

    public void ControlRotation(Vector2Int walkDir)
    {
        if (walkDir == new Vector2Int(-1, 0))
        {
            transform.eulerAngles = new Vector3(0, -90f, 0);
            return;
        }

        if (walkDir == new Vector2Int(1, 0))
        {
            transform.eulerAngles = new Vector3(0, 90f, 0);
            return;
        }

        if (walkDir == new Vector2Int(0, 1))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            return;
        }

        if (walkDir == new Vector2Int(0, -1))
        {
            transform.eulerAngles = new Vector3(0, 180f, 0);
            return;
        }
    }
}

public enum WolfState
{
    afraid, patrol, chase, attack, dead
}
