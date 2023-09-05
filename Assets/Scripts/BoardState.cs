using UnityEngine;

public class BoardState
{
    public IPiece[] Pieces { get; set; }

    public BoardState()
    {
        Pieces = new IPiece[100];
    }

    /// <summary>
    /// Get the piece at the given position.
    /// </summary>
    public IPiece GetPiece(Vector2Int pos)
    {
        return Pieces[pos.x + (pos.y * 10)];
    }

    /// <summary>
    /// Set the piece at the given position.
    /// </summary>
    public void SetPiece(IPiece piece, Vector2Int pos)
    {
        Pieces[pos.x + (pos.y * 10)] = piece;
    }
}
