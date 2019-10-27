using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Pawn
{
    public FoodType foodType;
    public float speed;
    public Dog dog;
    public float lifeTime;//存活时间;
    float t;
    private void Awake()
    {
        switch(foodType)
        {
            case FoodType.grass:
                speed = 2f;
                lifeTime = 3f;
                break;
            case FoodType.fruit:
                speed = 3f;
                lifeTime = 6f;
                break;
        }

        Speed = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        if(t>lifeTime)
        {
            switch (foodType)
            {
                case FoodType.fruit:
                    dog.hasPutDownFruit = false;
                    break;
                case FoodType.grass:
                    dog.hasPutDownGrass = false;
                    break;
            }

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var sheep = other.gameObject.GetComponent<Sheep>();
        if (sheep != null)
        {
            sheep.Speed = 1f;//恢复正常速度;

            switch(foodType)
            {
                case FoodType.fruit:
                    dog.hasPutDownFruit = false;
                    break;
                case FoodType.grass:
                    dog.hasPutDownGrass = false;
                    break;
            }
            Destroy(this.gameObject);
        }
    }
}
public enum FoodType
{
    grass,fruit
}