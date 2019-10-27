using System.Collections;
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

    private void Start()
    {
        barkRange = 4;
        fruitNum = 1;
        barkT = 0;
        barkRate = 0.1f;
        canBark = true;
    }

    //按键控制移动;
    void WalkControl()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            WalkStep(new Vector2Int(0, 1));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            WalkStep(new Vector2Int(0, -1));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            WalkStep(new Vector2Int(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            WalkStep(new Vector2Int(1, 0));
        }
    }
    //放置Grass或Fruit;
    void PutDownFood()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //如果还没有放置Grass,则允许放置一个Grass;
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
        WalkControl();

        PutDownFood();

        BarkControl();
    }

    private void OnTriggerEnter(Collider other)
    {
        Wolf wolf = other.gameObject.GetComponent<Wolf>();
        if (wolf != null)
        {
            wolf.state = WolfState.attack;//当攻击动画播放完毕,Wolf恢复巡逻状态;
            Destroy(this.gameObject);
        }
    }
}