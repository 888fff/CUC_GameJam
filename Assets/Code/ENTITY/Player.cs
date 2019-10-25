using UnityEngine;
using System.Collections;

public class Player : Pawn
{
    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartWalk()
    {
        Debug.Log("Player StartWalk");
    }
    public override void EndWalk()
    {
        Debug.Log("Player EndWalk");
    }
}
