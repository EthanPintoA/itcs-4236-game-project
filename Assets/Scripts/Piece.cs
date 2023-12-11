using UnityEngine;

public interface IPiece
{
    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; }
    public int Health { get; set; }

    public int GetDamage();

    /// <summary>
    /// What to display in the UI for damage.
    /// </summary>
    public string DamageAsStat { get; }
}

public enum PieceType
{
    Wall,
    Player1,
    Player2,
    Space
}
