using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefab;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var piecePos = MousePositionToPiecePosition();
            if (!piecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var (pieceGridPos, pieceGlobalPos) = piecePos.Value;

            Debug.Log($"Creating piece at {pieceGridPos}");

            var soldierObj = Instantiate(SoldierPrefab, pieceGlobalPos, Quaternion.identity);
            boardManager.boardState.SetPiece(new Soldier(soldierObj), pieceGridPos);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var piecePos = MousePositionToPiecePosition();
            if (!piecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var (pieceGridPos, _) = piecePos.Value;

            Debug.Log($"Deleting piece at {pieceGridPos}");

            boardManager.boardState.SetPiece(null, pieceGridPos);
        }
    }

    /// <summary>
    /// Converts the mouse's position to a piece's position.
    /// </summary>
    /// <returns> The piece's grid position and global position. </returns>
    public (Vector2Int, Vector2)? MousePositionToPiecePosition()
    {
        var mouseScreenPos = Mouse.current.position.ReadValue();
        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

        var gridPos = boardManager.WorldPosToGridPos(mouseWorldPos);

        if (!gridPos.HasValue)
        {
            return null;
        }

        var worldPos = boardManager.GridPosToWorldPos(gridPos.Value);

        return (gridPos.Value, worldPos);
    }
}
