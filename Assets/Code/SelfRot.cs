using UnityEngine;
using System.Collections;

public class SelfRot : MonoBehaviour
{
    public float speed = 20;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
