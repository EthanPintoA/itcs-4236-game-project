using UnityEngine;

public class Soldier : IPiece
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

    public string DamageAsStat { get; }

    public Soldier(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
        Range = 1;
        Health = 1;

        DamageAsStat = "1";
    }
}
