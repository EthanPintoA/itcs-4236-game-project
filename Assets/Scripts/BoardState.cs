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
        var currentPiece = GetPiece(pos);

        if (currentPiece != null)
        {
            Object.Destroy(currentPiece.GameObject);
        }

        Pieces[pos.x + (pos.y * 10)] = piece;
    }

    /// <summary>
    /// Moves a piece from one position to another.
    /// </summary>
    public void MovePiece(Vector2Int from, Vector2Int to)
    {
        var piece = GetPiece(from);
        var currentPiece = GetPiece(to);

        if (currentPiece != null)
        {
            Object.Destroy(currentPiece.GameObject);
        }

        //Set gameobject world position
        var offset = new Vector2(4.5f, -4.5f);
        var worldPos = new Vector2(to.x, -to.y) - offset;
        piece.GameObject.transform.position = worldPos;

        SetPiece(piece, to);
        // I'm not using SetPiece here because I don't want to destroy
        // the piece's game object.
        Pieces[from.x + (from.y * 10)] = null;
    }
}
