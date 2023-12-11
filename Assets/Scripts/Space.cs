using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Denotes a space that a piece can move
public class Space : IPiece
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

    public Space(GameObject gameObject)
    {
        GameObject = gameObject;
        Type = PieceType.Space;
        Movement = 0;
        Range = 0;
        Health = 0;
    }
}
