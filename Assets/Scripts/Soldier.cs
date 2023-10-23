using UnityEngine;

public class Soldier : IPiece
{
    public static int movement = 1;
    public int getDamage()
    {
        return 1;
    }
    public static int health = 1;
    public static int range = 1;
    public GameObject GameObject { get; }

    public Soldier(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
