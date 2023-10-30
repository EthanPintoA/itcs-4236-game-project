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

    public readonly BoardState boardState = new();

    void Awake()
    {
        var wallsArray = wallsParent.transform.GetComponentsInChildren<Transform>();

        // Skip the first child, which is the parent itself.
        foreach (var child in wallsArray.Skip(1))
        {
            var piecePos = WorldPosToGridPos(child.position);
            if (!piecePos.HasValue)
            {
                Debug.LogError($"Wall {child.name} is out of bounds");
                continue;
            }
            boardState.SetPiece(new Wall(child.gameObject), piecePos.Value);
        }
    }

    /// <summary>
    /// Converts a world position to a grid position.
    /// </summary>
    public Vector2Int? WorldPosToGridPos(Vector2 worldPos)
    {
        // Offset is the positional offset to make
        // world's (x, y) == grid's (x, -y).
        var offset = new Vector2(4.5f, -4.5f) - (Vector2)transform.position;

        var gridPos = Vector2Int.RoundToInt(worldPos + offset);

        // Inverted y because the grid's y has a inverse relationship with
        // the world's y.
        gridPos.y = -gridPos.y;

        // Check if the grid position is out of bounds.
        if (gridPos.x < 0 || 9 < gridPos.x || gridPos.y < 0 || 9 < gridPos.y)
        {
            return null;
        }

        return gridPos;
    }

    /// <summary>
    /// Converts a grid position to a world position.
    /// </summary>
    public Vector2 GridPosToWorldPos(Vector2Int gridPos)
    {
        // Offset is the positional offset to make
        // world's (x, y) == grid's (x, -y).
        var offset = new Vector2(4.5f, -4.5f) - (Vector2)transform.position;

        // Inverted y because the grid's y has a inverse relationship with
        // the world's y.
        return new Vector2(gridPos.x, -gridPos.y) - offset;
    }

    public bool DidPlayerWin(PieceType player)
    {
        var enemy = (player == PieceType.Player1) ? PieceType.Player2 : PieceType.Player1;
        var enemyHasPieces = boardState.Pieces.Any(piece => piece?.Type == enemy);

        return !enemyHasPieces;
    }
}
