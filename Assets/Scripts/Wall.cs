using UnityEngine;

public class Wall : IPiece
{
    public int GetDamage()
    {
        return 0;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public string DamageAsStat => "0";

    public Wall(GameObject gameObject)
    {
        GameObject = gameObject;
        Type = PieceType.Wall;
        Movement = 0;
        Range = 0;
        Health = 1; //10
    }
}
