using UnityEngine;

public class Soldier : IPiece
{
    public int getDamage()
    {
        return 1;
    }
    public static int health = 1;
    public static int range = 1;

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }

    public Soldier(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 3;
    }
}
