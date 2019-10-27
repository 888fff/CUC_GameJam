using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyTrap : Pawn
{
    void Start()
    {
        //unmoveable!!!
        Speed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Pawn life = other.gameObject.GetComponent<Pawn>();
        if (life != null)
        {
            Destroy(other.gameObject);
        }
    }
}
