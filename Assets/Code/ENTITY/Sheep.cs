using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Pawn
{
    public SheepState state;
    public int randomWalkRate;//随机行走的几率,0-100之间,<=该值即可随机行走;
    public Vector2Int curPosition;
    public float randomWalkTimeRate;
    public float t;

    public Animator anim;
    public bool isDead;
    private void Start()
    {
        state = SheepState.idle;
        SightLength = 2;
        Speed = 0.3f;
        randomWalkRate = 20;
        WalkDir = new Vector2Int(1, 0);
        randomWalkTimeRate = 2.5f;

        anim = GetComponent<Animator>();
        isDead = false;
    }

    private void Update()
    {
        ControlRotation(WalkDir);
        MoveControl();
    }

    void MoveControl()
    {
        if (state == SheepState.dead)
            return;

        //检测是否有食物;
        Vector2Int foodPos = Vector2Int.zero;
        Vector2Int moveDir = Vector2Int.zero;
        var pawn = DetectFood(WalkDir, ref foodPos,ref moveDir);
        if(pawn!=null)
        {
            Food f = pawn.transform.gameObject.GetComponent<Food>();
            if(f!=null)
            {
                //向食物移动;
                Speed = f.speed;
                //WalkTo(foodPos);
                WalkStep(moveDir);
                state = SheepState.chaseFood;
            }
        }
        else
        {
            t += Time.deltaTime;
            if(t> randomWalkTimeRate)
            {
                t = 0;
                //符合时间间隔;


                //如果没有检测到食物,随机判定是否要行走;
                int rate = Random.Range(0, 100);
                if (rate <= randomWalkRate)//在随机行走范围内，则再进行一次随机来判定向哪个方向行走;
                {
                    Vector2Int randomMoveDir = FindRandomDir();
                    WalkStep(randomMoveDir);
                    state = SheepState.walk;
                }
                else
                {
                    //否则就呆在原地;
                    //Do Nothing
                    state = SheepState.idle;
                }
            }
        }
    } 
    
    //返回一个有效随机运动方向;
    Vector2Int FindRandomDir()
    {
        Vector2Int retVal=Vector2Int.zero;
        bool findIt=false;

        do
        {
            int randomDirection = Random.Range(0, 400);
            int dir = randomDirection / 100;
            switch (dir)
            {
                case 0:
                    retVal = new Vector2Int(0, 1);
                    findIt = DectectPos(retVal);
                    break;
                case 1:
                    retVal = new Vector2Int(1, 0);
                    findIt = DectectPos(retVal);
                    break;
                case 2:
                    retVal = new Vector2Int(0, -1);
                    findIt = DectectPos(retVal);
                    break;
                case 3:
                    retVal = new Vector2Int(-1, 0);
                    findIt = DectectPos(retVal);
                    break;
            }
        }
        while (!findIt);

        return retVal;
    }

    //
    bool DectectPos(Vector2Int dir)
    {
        bool retVal = false;
        curPosition= GetGridPos();
        int targetX = curPosition.x + dir.x;
        int targetY = curPosition.y + dir.y;
        int mapIndex = targetY * Board.Row + targetX;//5 + targetX;//即将移动位置的总索引值;
        if (targetX < 0 || targetX >= Board.Col || targetY < 0 || targetY >= Board.Row)//!!!此处需修改边界值;
        {
            retVal = false;
        }
        else
        {
            if (Board.BoardData[mapIndex] != 0)
                retVal = false;
            else
                retVal = true;
        }

        return retVal;
    }

    private void OnTriggerEnter(Collider other)
    {
        Wolf wolf = other.gameObject.GetComponent<Wolf>();
        if (wolf!= null)
        {
            wolf.state = WolfState.attack;//当攻击动画播放完毕,Wolf恢复巡逻状态;
            isDead = true;
            anim.SetTrigger("die");

            Invoke("DestroySelf", 3f);
            //Destroy(this.gameObject);
        }
    }

    public void DestroySelf()
    {
        GameManager.GetInstance().MissionFailed();
        Destroy(this.gameObject);
    }

    public override void StartWalk()
    {
        anim.SetTrigger("run");
    }

    public override void EndWalk()
    {
        anim.SetTrigger("idle");
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

public enum SheepState
{
    //walk对应随机运动;
    chaseFood,idle,walk,dead
}