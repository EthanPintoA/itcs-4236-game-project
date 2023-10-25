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
    public static int health = 1;
    public static int range = 1;

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }

    public Sniper(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
    }
}
