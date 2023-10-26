using UnityEngine;

public interface IPiece
{
    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
}

public enum PieceType
{
    Wall,
    Player1,
    Player2,
    Space
}
