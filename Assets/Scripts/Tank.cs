using UnityEngine;

public class Tank : IPiece
{
    public int GetDamage()
    {
        int randomNumber = Random.Range(1, 101);
        return (randomNumber <= 50) ? 3 : 4;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public string DamageAsStat { get; }

    public Tank(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 2;
        Range = 2;
        Health = 5;

        DamageAsStat = "3~4";
    }
}
