using UnityEngine;

public interface IPiece
{
    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public int GetDamage();

}

public enum PieceType
{
    Wall,
    Player1,
    Player2,
    Space
}
