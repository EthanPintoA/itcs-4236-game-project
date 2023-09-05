using UnityEngine;

public class Soldier : IPiece
{
    public GameObject GameObject { get; }

    public Soldier(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
