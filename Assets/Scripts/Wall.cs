using UnityEngine;

public class Wall : IPiece
{
    // public static int movement = 0;
    // public getDamage()
    // {
    //     return 0;
    // }
    public static int health = 10;
    // public static int range = 1;
    public GameObject GameObject { get; }

    public Wall(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
