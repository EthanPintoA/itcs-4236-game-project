using UnityEngine;

public class Helicopter : IPiece
{
    public int GetDamage()
    {
        /*int randomNumber = Random.Range(1, 101);
        return (randomNumber <= 30) ? 1 :
            (randomNumber <= 60) ? 2 :
            (randomNumber <= 85) ? 3 :
            4;
        */
        return 1;
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public bool CarryingAnotherPiece { get; set; }

    public Helicopter(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 2;
        Range = 2;
        Health = 1;
        CarryingAnotherPiece = false;
    }
}
