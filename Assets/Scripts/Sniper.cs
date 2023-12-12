using UnityEngine;

public class Sniper : IPiece
{
    public int GetDamage()
    {
        int randomNumber = Random.Range(1, 101);
        return (randomNumber <= 30) ? 1 :
            (randomNumber <= 60) ? 2 :
            (randomNumber <= 85) ? 3 :
            4;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public string DamageAsStat { get; }

    public Sniper(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
        Range = 4;
        Health = 1;

        DamageAsStat = "1~4";
    }
}
