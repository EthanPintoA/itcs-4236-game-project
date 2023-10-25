using UnityEngine;

public class Tank : IPiece
{
    public int GetDamage()
    {
        int randomNumber = Random.Range(1, 101);
        return (randomNumber <= 50) ? 3 : 4;
    }
    public static int health = 5;
    public static int range = 2;

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }

    public Tank(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
    }
}
