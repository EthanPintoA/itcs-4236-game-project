using UnityEngine;

public class Tank : IPiece
{
    public int GetDamage()
    {
        //int randomNumber = Random.Range(1, 101);
        //return (randomNumber <= 50) ? 3 : 4;
        return 1;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public Tank(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 1;
        Range = 2;
        Health = 5; //5
    }
}
