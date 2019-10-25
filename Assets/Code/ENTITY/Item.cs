using UnityEngine;
using System.Collections;

public class Item : Pawn
{

    void Start()
    {
        //unmoveable!!!
        Speed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        var script = other.gameObject.GetComponent<Player>();
        if (script != null)
        {
            Destroy(this.gameObject);
        }
    }
}
