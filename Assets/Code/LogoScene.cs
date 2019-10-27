using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class LogoScene : MonoBehaviour
{
    public Text label;
    // Use this for initialization
    void Start()
    {
        //label.transform.DOShakePosition(20, new Vector3(10, 10, 0));
        Invoke("Goto_Game",1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.anyKeyDown)
        {
            GameManager.GetInstance().StartMenu();
        }
        */
    }

    public void Goto_Game()
    {
        GameManager.GetInstance().StartMenu();
    }
}
