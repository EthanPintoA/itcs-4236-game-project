using UnityEngine;

public class King : IPiece
{
    public int GetDamage()
    {
        return 1;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public King(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
        Range = 1;
        Health = 15;
    }
}
