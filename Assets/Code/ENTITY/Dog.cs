﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Pawn
{
    public Vector2Int curPosition;
    public bool hasPutDownGrass;//true标志着已经放置grass;
    public bool hasPutDownFruit;//true标志着已经放置fruit;

    public int barkRange;//吼叫范围有几格(向正前方);
    public int fruitNum;//水果的数量;

    public bool canBark;//能否吼叫;
    public float barkRate;//吼叫冷却时长;
    public int CD_percentage;//CD百分比值(转圈程度,100即可释放技能);
    public float barkT;//吼叫计时器;

    //动画相关;
    public Animator anim;
    public bool isDead;

    public bool readyPutDownGrass=true;

    private void Start()
    {
        barkRange = 4;
        fruitNum = 1;
        barkT = 0;
        barkRate = 0.1f;
        canBark = true;

        anim = GetComponent<Animator>();
        isDead = false;
    }

    //按键控制移动;
    void WalkControl()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Vector2Int targetIndex = GetGridPos() + new Vector2Int(0, 1);
            if (targetIndex.x < 0 || targetIndex.y < 0 || targetIndex.x >= Board.Col || targetIndex.y >= Board.Row)
            {              
                //无法移动
            }
            else
            {
                if (Board.GetGridType(targetIndex) == 0 || Board.GetGridType(targetIndex) == 9)
                {
                    WalkStep(new Vector2Int(0, 1));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector2Int targetIndex = GetGridPos() + new Vector2Int(0, -1);
            if (targetIndex.x < 0 || targetIndex.y < 0 || targetIndex.x >= Board.Col || targetIndex.y >= Board.Row)
            {
                //无法移动
            }
            else
            {
                if (Board.GetGridType(targetIndex) == 0 || Board.GetGridType(targetIndex) == 9)
                {
                    WalkStep(new Vector2Int(0, -1));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector2Int targetIndex = GetGridPos() + new Vector2Int(-1, 0);
            if (targetIndex.x < 0 || targetIndex.y < 0 || targetIndex.x >= Board.Col || targetIndex.y >= Board.Row)
            {
                //无法移动
            }
            else
            {
                if (Board.GetGridType(targetIndex) == 0 || Board.GetGridType(targetIndex) == 9)
                {
                    WalkStep(new Vector2Int(-1, 0));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector2Int targetIndex = GetGridPos() + new Vector2Int(1, 0);
            if (targetIndex.x < 0 || targetIndex.y < 0 || targetIndex.x >= Board.Col || targetIndex.y >= Board.Row)
            {
                //无法移动
            }
            else
            {
                if (Board.GetGridType(targetIndex) == 0 || Board.GetGridType(targetIndex) == 9)
                {
                    WalkStep(new Vector2Int(1, 0));
                }
            }
        }
    }
    //放置Grass或Fruit;
    void PutDownFood()
    {
        if (Input.GetKeyDown(KeyCode.J)&&readyPutDownGrass)
        {
            //readyPutDownGrass = true;

            //如果还没有放置Grass,则允许放置一个Grass;
            if (!hasPutDownGrass)
            {
                //readyPutDownGrass = true;

                hasPutDownGrass = true;
                Vector2Int putDownPosition = GetGridPos();

                //放置一个Grass(实例化一个Grass)
                var go = GameObject.Instantiate(Resources.Load("Grass")) as GameObject;
                go.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                Food item = go.GetComponent<Food>();
                Board.RegisterPawn(item);
                item.SetToGrid(putDownPosition);
                item.dog = this;
                readyPutDownGrass = false;
                Debug.Log("FoodPos:" + putDownPosition.ToString());
            }
            else
            {
                Debug.Log("Already put down a grass!");
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            //如果还没有放置Grass,则允许放置一个Grass;
            if (!hasPutDownFruit&&fruitNum>0)
            {
                hasPutDownFruit = true;
                Vector2Int putDownPosition = GetGridPos();

                //放置一个Grass(实例化一个Fruit)
                var go = GameObject.Instantiate(Resources.Load("Fruit")) as GameObject;
                go.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
                Food item = go.GetComponent<Food>();
                Board.RegisterPawn(item);
                item.SetToGrid(putDownPosition);
                item.dog = this;

                fruitNum--;
            }
            else
            {
                Debug.Log("Already put down a fruit!");
            }
        }
    }
    //管理Grass和Fruit;

    //控制吼叫;
    void BarkControl()
    {
        if(!canBark)
        {
            barkT += Time.deltaTime;
            CD_percentage = (int)(barkT / barkRate*100);
            if(barkT > barkRate)
            {
                barkT = 0;
                canBark = true;
                CD_percentage = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                DetectWolf(WalkDir, barkRange);
                canBark = false;
            }
        }
    }

    private void Update()
    {
        ControlRotation(WalkDir);

        PutDownFood();

        WalkControl();

        BarkControl();
    }

    private void OnTriggerEnter(Collider other)
    {
        Wolf wolf = other.gameObject.GetComponent<Wolf>();
        if (wolf != null)
        {
            wolf.state = WolfState.attack;//当攻击动画播放完毕,Wolf恢复巡逻状态;
            isDead = true;
            anim.SetTrigger("die");

            Invoke("DestroySelf", 3f);
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
        readyPutDownGrass = false;
    }

    public override void EndWalk()
    {
        anim.SetTrigger("idle");
        readyPutDownGrass = true;

        /*if(readyPutDownGrass)
        {
            if (!hasPutDownGrass)
            {
                hasPutDownGrass = true;
                Vector2Int putDownPosition = GetGridPos();

                //放置一个Grass(实例化一个Grass)
                var go = GameObject.Instantiate(Resources.Load("Grass")) as GameObject;
                go.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                Food item = go.GetComponent<Food>();
                Board.RegisterPawn(item);
                item.SetToGrid(putDownPosition);
                item.dog = this;
            }
            else
            {
                Debug.Log("Already put down a grass!");
            }

            readyPutDownGrass = false;
        }*/
    }

    public void ControlRotation(Vector2Int walkDir)
    {
        if(walkDir==new Vector2Int(-1,0))
        {
            transform.eulerAngles = new Vector3(0,- 90f, 0);
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