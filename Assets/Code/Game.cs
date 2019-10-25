using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Game : MonoBehaviour
{
    public Chessboard cb;
    public Pawn player;
    public Pawn item;
    public InputField IF;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            player.WalkStep(new Vector2Int(0,1));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            player.WalkStep(new Vector2Int(0, -1));

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.WalkStep(new Vector2Int(-1, 0));

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            player.WalkStep(new Vector2Int(1, 0));

        }
    }
    //
    public void __TestLoadMap()
    {
        cb.LoadMap(1);
    }

    public void __TestCreatePawn()
    {
        var go = GameObject.Instantiate(Resources.Load("Player")) as GameObject;
        go.GetComponent<MeshRenderer>().material.color = Color.blue;
        player = go.GetComponent<Player>();
        cb.RegisterPawn(player);
        player.SetToGrid(Vector2Int.zero);
        //
        go = GameObject.Instantiate(Resources.Load("Item")) as GameObject;
        item = go.GetComponent<Item>();
        cb.RegisterPawn(item);
        item.SetToGrid(new Vector2Int(3,3));
        //

    }

    public void __TestPlayerWalk()
    {
        string[] sArray = IF.text.Split(',');
        var x = int.Parse(sArray[0]);
        var y = int.Parse(sArray[1]);
        player.WalkTo(new Vector2Int(x, y));
    }
    

}
