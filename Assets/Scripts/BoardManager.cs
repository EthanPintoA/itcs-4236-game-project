using System.Linq;

using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    [Tooltip("The GameObject that contains all the walls in the scene.")]
    private GameObject wallsParent;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefab;

    // 10 x 10 board
    private readonly BoardState boardState = new();

    void Awake()
    {
        var wallsArray = wallsParent.transform.GetComponentsInChildren<Transform>();

        // Skip the first child, which is the parent itself.
        foreach (var child in wallsArray.Skip(1))
        {
            var piecePos = GlobalPositionToPiecePosition(child.position);
            boardState.SetPiece(new Wall(child.gameObject), piecePos);
        }

        foreach (var (piece, i) in boardState.Pieces.Select((p, i) => (p, i)))
        {
            if (piece != null)
            {
                continue;
            }

            var piecePos = new Vector2Int(i % 10, i / 10);
            var globalPos = PiecePositionToGlobalPosition(piecePos);

            var _ = Instantiate(SoldierPrefab, globalPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// Converts a global position to a piece's position.
    /// </summary>
    /// <returns></returns>
    public Vector2Int GlobalPositionToPiecePosition(Vector2 pos)
    {
        // (-4.5, 4.5) is the position of the top left corner of the board and a Wall is 1 unit wide.
        // Therefore, if a child is at (-4.5, 4.5), it should be at pieces[0].
        // If a child is at (-3.5, 4.5), it should be at pieces[1].
        // If a child is at (-4.5, 3.5), it should be at pieces[10].
        var x = Mathf.RoundToInt(pos.x + 4.5f);
        var y = Mathf.RoundToInt(-pos.y + 4.5f);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Converts a piece's position to a global position.
    /// </summary>
    public Vector2 PiecePositionToGlobalPosition(Vector2Int pos)
    {
        var x = pos.x - 4.5f;
        var y = -pos.y + 4.5f;

        return new Vector2(x, y);
    }
}
