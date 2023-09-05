using UnityEngine;

public class Wall : IPiece
{
    public GameObject GameObject { get; }

    public Wall(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
