using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Denotes a space that a piece can move
public class Space : IPiece
{
    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }

    public Space(GameObject gameObject)
    {
        GameObject = gameObject;
        Type = PieceType.Space;
        Movement = 0;
    }
}
